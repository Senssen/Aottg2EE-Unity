using UnityEngine;

public class PhysicsWagon : MonoBehaviour
{
    [SerializeField] private Transform Body;
    [SerializeField] private Transform WheelsFront;
    [SerializeField] private Transform WheelsBack;
    [SerializeField] public HingeJoint HorseHinge;

    private Rigidbody wagonRigidbody;
    private float wheelCircumference;

    void Start()
    {
        wagonRigidbody = Body.GetComponent<Rigidbody>();
        float wheelRadius = 0.7f;
        wheelCircumference = 2 * Mathf.PI * wheelRadius;
    }

    void FixedUpdate()
    {
        RotateWheels();
    }

    void RotateWheels()
    {
        float speed = wagonRigidbody.velocity.magnitude;
        float rotationAngle = speed / wheelCircumference * 360f * Time.fixedDeltaTime;
        WheelsFront.Rotate(Vector3.right, rotationAngle);
        WheelsBack.Rotate(Vector3.right, rotationAngle);
    }
}
