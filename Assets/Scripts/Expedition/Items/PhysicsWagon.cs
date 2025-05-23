using UnityEngine;
using Characters;

public class PhysicsWagon : MonoBehaviour
{
    [SerializeField] private Transform Body;
    [SerializeField] private Transform WheelsFront;
    [SerializeField] private Transform WheelsBack;
    [SerializeField] public HingeJoint HorseHinge;
    [SerializeField] public Rigidbody TemporaryHinge;
    private Rigidbody wagonRigidbody;
    private float wheelCircumference;
    public bool isMounted = false;

    private float _setKinematicTimer = 3f;
    private bool _isInitialKinematicSet = false;

    void Start()
    {
        wagonRigidbody = Body.GetComponent<Rigidbody>();
        float wheelRadius = 0.7f;
        wheelCircumference = 2 * Mathf.PI * wheelRadius;
    }

    void Update()
    {
        if (_isInitialKinematicSet == true)
            return;

        if (_setKinematicTimer > 0) {
            _setKinematicTimer -= Time.deltaTime;
        } else {
            SetKinematic(true);
            _isInitialKinematicSet = true;
        }
    }

    void FixedUpdate()
    {
        RotateWheels();
        FollowHorse();
    }

    void RotateWheels()
    {
        float speed = wagonRigidbody.velocity.magnitude;
        float rotationAngle = speed / wheelCircumference * 360f * Time.fixedDeltaTime;
        WheelsFront.Rotate(Vector3.right, rotationAngle);
        WheelsBack.Rotate(Vector3.right, rotationAngle);
    }

    void FollowHorse()
    {
        if (isMounted == false)
            return;

        Horse horse = HorseHinge.connectedBody.gameObject.GetComponent<Horse>();
        if (horse == null)
            return;
        
        HorseHinge.gameObject.transform.position = horse.Cache.Transform.position - horse.Cache.Transform.forward * 2.3f + Vector3.up * 0.6f;
    }

    public void SetKinematic(bool _isKinematic)
    {
        Body.gameObject.GetComponent<Rigidbody>().isKinematic = _isKinematic;
        HorseHinge.gameObject.GetComponent<Rigidbody>().isKinematic = _isKinematic;
        TemporaryHinge.gameObject.GetComponent<Rigidbody>().isKinematic = _isKinematic;
    }
}
