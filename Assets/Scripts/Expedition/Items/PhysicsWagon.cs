using UnityEngine;
using Characters;
using Photon.Pun;

public class PhysicsWagon : MonoBehaviour
{
    [SerializeField] private Transform Body;
    [SerializeField] private Transform WheelsFront;
    [SerializeField] private Transform WheelsBack;
    [SerializeField] public GameObject WoodSaddleBeams;
    [SerializeField] public ConfigurableJoint HorseHinge;
    public Rigidbody wagonRigidbody;
    private float wheelCircumference;
    private bool isMounted = false;

    void Start()
    {
        wagonRigidbody = Body.GetComponent<Rigidbody>();
        float wheelRadius = 0.7f;
        wheelCircumference = 2 * Mathf.PI * wheelRadius;
    }

    void FixedUpdate()
    {
        RotateWheels();
        FollowHorse();
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
        if (isMounted == false)
            return;

        Horse horse = HorseHinge.connectedBody?.gameObject.GetComponent<Horse>();
        if (horse == null)
            return;

        HorseHinge.gameObject.transform.position = horse.Cache.Transform.position - horse.Cache.Transform.forward * 2.3f + Vector3.up * 0.6f;
    }

    public float GetDistance(Transform entity)
    {
        return Vector3.Distance(entity.position, Body.position);
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
        Rigidbody hingeRb = HorseHinge.gameObject.GetComponent<Rigidbody>();
        if (hingeRb != null) hingeRb.isKinematic = _isKinematic;
        //TemporaryHinge.isKinematic = _isKinematic;
        wagonRigidbody.isKinematic = _isKinematic;
    }
}