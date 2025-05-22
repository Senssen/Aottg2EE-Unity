using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsWagon : MonoBehaviour
{
    [SerializeField] private Transform Body;
    [SerializeField] private Transform WheelsFront;
    [SerializeField] private Transform WheelsBack;
    [SerializeField] public HingeJoint HorseHinge;

    private Rigidbody wagonRigidbody;
    private float wheelCircumference;

    // Start is called before the first frame update
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
        // Calculate the speed of the wagon body
        float speed = wagonRigidbody.velocity.magnitude;

        // Calculate the rotation angle for the wheels
        float rotationAngle = (speed / wheelCircumference) * 360f * Time.fixedDeltaTime;

        // Apply the rotation to the wheels
        WheelsFront.Rotate(Vector3.right, rotationAngle);
        WheelsBack.Rotate(Vector3.right, rotationAngle);
    }
}
