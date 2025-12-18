using UnityEngine;
using Photon.Pun;
using Utility;

class PhysicsWagonMovementSync : MonoBehaviourPun, IPunObservable
{
    private PhysicsWagon physicsWagon;
    private Vector3 _correctPosition = Vector3.zero;
    private Quaternion _correctRotation = Quaternion.identity;
    public Vector3 _correctVelocity = Vector3.zero;
    private bool _syncVelocity = false;
    private float SmoothingDelay => 10f;
    private float MaxPredictionTime = 0.5f;
    private Transform _transform;
    private Rigidbody _rigidbody;
    private PhotonView _photonView;
    private float _timeSinceLastMessage = 0f;

    private void Awake()
    {
        physicsWagon = GetComponent<PhysicsWagon>();
        _photonView = physicsWagon.gameObject.GetPhotonView();
        _transform = physicsWagon.transform;
        _rigidbody = physicsWagon.wagonRigidbody;

        _correctPosition = _transform.position;
        _correctRotation = transform.rotation;
        if (_rigidbody != null)
        {
            _syncVelocity = true;
            _correctVelocity = _rigidbody.velocity;
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_transform.position);
            var rotation = _transform.rotation;
            stream.SendNext(QuaternionCompression.CompressQuaternion(ref rotation));
            if (_syncVelocity)
                stream.SendNext(_rigidbody.velocity);
        }
        else
        {
            _correctPosition = (Vector3)stream.ReceiveNext();
            QuaternionCompression.DecompressQuaternion(ref _correctRotation, (int)stream.ReceiveNext());
            if (_syncVelocity)
                _correctVelocity = (Vector3)stream.ReceiveNext();
            _timeSinceLastMessage = 0f;
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            _transform.position = Vector3.Lerp(_transform.position, _correctPosition, Time.deltaTime * SmoothingDelay);
            _transform.rotation = Quaternion.Lerp(_transform.rotation, _correctRotation, Time.deltaTime * SmoothingDelay);
            if (_syncVelocity && _timeSinceLastMessage < MaxPredictionTime)
            {
                _correctPosition += _correctVelocity * Time.deltaTime;
                _timeSinceLastMessage += Time.deltaTime;
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        OnPhotonSerializeView(stream, info);
    }
}