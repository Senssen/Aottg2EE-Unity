using UnityEngine;
using Characters;
using Photon.Pun;

public class PhysicsWagon : MonoBehaviour
{
    [SerializeField] public Transform HorseSpot;
    [SerializeField] private Transform WheelsFront;
    [SerializeField] private Transform WheelsBack;
    [SerializeField] public GameObject Beams;
    public Rigidbody wagonRigidbody;
    private FixedJoint Joint;
    private float wheelCircumference;
    private bool isMounted = false;

    void Start()
    {
        float wheelRadius = 0.7f;
        wheelCircumference = 2 * Mathf.PI * wheelRadius;
    }

    void FixedUpdate()
    {
        RotateWheels();
        //FollowHorse();
    }

    void RotateWheels()
    {
        float speed = wagonRigidbody.velocity.magnitude;
        if (speed < 1f) return;

        float rotationAngle = speed / wheelCircumference * 360f * Time.fixedDeltaTime;
        WheelsFront.Rotate(Vector3.right, rotationAngle);
        WheelsBack.Rotate(Vector3.right, rotationAngle);
    }

    void FollowHorse()
    {
        if (isMounted == false) return;

        Horse horse = Joint?.connectedBody?.gameObject.GetComponent<Horse>();
        if (horse == null) return;

        HorseSpot.position = horse.Cache.Transform.position/*  - horse.Cache.Transform.forward * 2.3f + Vector3.up * 0.6f */;
    }

    public void CreateJoint(Rigidbody rb)
    {
        Joint = Beams.AddComponent<FixedJoint>();
        Joint.connectedBody = rb;
    }

    public void DestroyJoint()
    {
        Destroy(Joint);
    }

    public FixedJoint GetJoint()
    {
        return Joint;
    }

    public float GetDistance(Transform entity)
    {
        return Vector3.Distance(entity.position, wagonRigidbody.transform.position);
    }

    public void SetIsMounted(bool _isMounted)
    {
        isMounted = _isMounted;
        SetKinematics(!_isMounted);
    }

    public bool GetIsMounted()
    {
        return isMounted;
    }

    public void SetKinematics(bool _isKinematic)
    {
        Rigidbody hingeRb = Joint.gameObject.GetComponent<Rigidbody>();
        if (hingeRb != null) hingeRb.isKinematic = _isKinematic;
        wagonRigidbody.isKinematic = _isKinematic;
    }
}