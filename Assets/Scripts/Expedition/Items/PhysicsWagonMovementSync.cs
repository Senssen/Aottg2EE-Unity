using UnityEngine;
using Photon.Pun;
using Utility;

class PhysicsWagonMovementSync : MonoBehaviourPun, IPunObservable
{
    private PhysicsWagon physicsWagon;
    private float _timeSinceLastMessage = 0f;

    private Vector3 _correctWagonPosition = Vector3.zero;
    private Quaternion _correctWagonRotation = Quaternion.identity;
    public Vector3 _correctWagonVelocity = Vector3.zero;

    private Vector3 _correctBeamsPosition = Vector3.zero;
    private Quaternion _correctBeamsRotation = Quaternion.identity;
    public Vector3 _correctBeamsVelocity = Vector3.zero;

    private Transform _wagonTransform;
    private Rigidbody _wagonRigidbody;

    private Transform _beamsTransform;
    private Rigidbody _beamsRigidbody;

    private float SmoothingDelay => 10f;
    private float MaxPredictionTime = 0.5f;

    private void Awake()
    {
        physicsWagon = GetComponent<PhysicsWagon>();

        _wagonTransform = physicsWagon.transform;
        _wagonRigidbody = physicsWagon.wagonRigidbody;

        _beamsTransform = physicsWagon.Beams.transform;
        _beamsRigidbody = physicsWagon.Beams.GetComponent<Rigidbody>();

        if (!photonView.IsMine)
        {
            var configurableJointoint = physicsWagon.GetComponent<ConfigurableJoint>();
            Destroy(configurableJointoint);
            _wagonRigidbody.isKinematic = true;
            _beamsRigidbody.isKinematic = true;
        }

        _correctWagonPosition = _wagonTransform.position;
        _correctWagonRotation = _wagonTransform.rotation;

        _correctBeamsPosition = _beamsTransform.position;
        _correctBeamsRotation = _beamsTransform.rotation;

        _correctWagonVelocity = _wagonRigidbody.velocity;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            var wagonRotation = _wagonTransform.rotation;
            stream.SendNext(_wagonTransform.position);
            stream.SendNext(QuaternionCompression.CompressQuaternion(ref wagonRotation));
            
            var localBeamsRotation = Quaternion.Inverse(_wagonTransform.rotation) * _beamsTransform.rotation;
            stream.SendNext(_wagonTransform.InverseTransformPoint(_beamsTransform.position));
            stream.SendNext(QuaternionCompression.CompressQuaternion(ref localBeamsRotation));

            stream.SendNext(_wagonRigidbody.velocity);
        }
        else
        {
            _correctWagonPosition = (Vector3)stream.ReceiveNext();
            QuaternionCompression.DecompressQuaternion(ref _correctWagonRotation, (int)stream.ReceiveNext());

            _correctBeamsPosition = (Vector3)stream.ReceiveNext();
            QuaternionCompression.DecompressQuaternion(ref _correctBeamsRotation, (int)stream.ReceiveNext());

            _correctWagonVelocity = (Vector3)stream.ReceiveNext();
            
            _timeSinceLastMessage = 0f;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            if (_timeSinceLastMessage < MaxPredictionTime)
            {
                _correctWagonPosition += _correctWagonVelocity * Time.deltaTime;
                _timeSinceLastMessage += Time.deltaTime;
            }
            
            _wagonTransform.position = Vector3.Lerp(
                _wagonTransform.position, 
                _correctWagonPosition, 
                Time.deltaTime * SmoothingDelay
            );
            _wagonTransform.rotation = Quaternion.Lerp(
                _wagonTransform.rotation, 
                _correctWagonRotation, 
                Time.deltaTime * SmoothingDelay
            );

            _beamsTransform.position = Vector3.Lerp(
                _beamsTransform.position, 
                _wagonTransform.TransformPoint(_correctBeamsPosition), 
                Time.deltaTime * SmoothingDelay
            );
            _beamsTransform.rotation = Quaternion.Lerp(
                _beamsTransform.rotation, 
                _wagonTransform.rotation * _correctBeamsRotation, 
                Time.deltaTime * SmoothingDelay
            );
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        OnPhotonSerializeView(stream, info);
    }
}