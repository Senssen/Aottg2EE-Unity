using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomoWagon : MonoBehaviour
{
    [SerializeField] private Transform Wagon_Body;
    [SerializeField] private Transform Wagon_Saddle;
    [SerializeField] private Transform Front_Wheels;
    [SerializeField] private Transform Back_Wheels;
    [SerializeField] public HingeJoint HarnessJoint; // remove this because it wont be used later

    private Rigidbody wagonRigidbody;
    private float wheelCircumference;

    // Start is called before the first frame update
    void Start()
    {
        wagonRigidbody = Wagon_Body.GetComponent<Rigidbody>();
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
        Front_Wheels.Rotate(Vector3.right, rotationAngle);
        Back_Wheels.Rotate(Vector3.right, rotationAngle);
    }
}
