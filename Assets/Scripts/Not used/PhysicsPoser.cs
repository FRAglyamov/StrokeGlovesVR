using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ArticulationBody))]
//[RequireComponent(typeof(ActionBasedController))]
public class PhysicsPoser : MonoBehaviour
{
    public float physicsRange = 0.1f;
    public LayerMask physicsMask = 0;

    [Range(0, 1)] public float slowDownVelocity = 0.75f;
    [Range(0, 1)] public float slowDownAngularVelocity = 0.75f;

    [Range(0, 100)] public float maxPositionChange = 75f;
    [Range(0, 100)] public float maxRotationChange = 75f;

    private ArticulationBody ab;
    //[SerializeField]
    //private ActionBasedController controller;
    [SerializeField]
    private Transform controllerTransfrom;

    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;
    private void Awake()
    {
        ab = GetComponent<ArticulationBody>();
        //controller = GetComponent<ActionBasedController>();
    }

    private void Start()
    {
        // As soon as we start, move to the hand
        UpdateTracking();
        MoveUsingTransform();
        RotateUsingTransform();
    }

    private void Update()
    {
        // Update our target location
        UpdateTracking();
    }

    private void UpdateTracking()
    {
        // Get the rotation and position from the device
        //targetPosition = controller.positionAction.action.ReadValue<Vector3>();
        //targetRotation = controller.rotationAction.action.ReadValue<Quaternion>();

        // Or we can just get pos, rot of controller game object.
        targetPosition = controllerTransfrom.position;
        targetRotation = controllerTransfrom.rotation;
    }

    private void FixedUpdate()
    {
        if (WithinPhysicsRange())
        {
            MoveUsingPhysics();
            RotateUsingPhysics();
        }
        else
        {
            MoveUsingTransform();
            RotateUsingTransform();
        }
    }

    public bool WithinPhysicsRange()
    {
        return Physics.CheckSphere(transform.position, physicsRange, physicsMask, QueryTriggerInteraction.Ignore);
    }

    private void MoveUsingPhysics()
    {
        // Prevents overshooting
        ab.velocity *= slowDownVelocity;

        Vector3 velocity = FindNewVelocity();
        if (IsValidVelocity(velocity.x))
        {
            // Figure out the max we can move, then move via velocity
            float maxChange = maxPositionChange * Time.deltaTime;
            ab.velocity = Vector3.MoveTowards(ab.velocity, velocity, maxChange);
        }
    }

    // Get target velocity (with estimated time).
    private Vector3 FindNewVelocity()
    {
        Vector3 difference = targetPosition - ab.transform.position;
        return difference / Time.deltaTime;
    }

    private void RotateUsingPhysics()
    {
        // Prevents overshooting
        ab.angularVelocity *= slowDownAngularVelocity;

        Vector3 angularVelocity = FindNewAngularVelocity();
        if (IsValidVelocity(angularVelocity.x))
        {
            // Figure out the max we can rotate, then move via velocity
            float maxChange = maxRotationChange * Time.deltaTime;
            ab.angularVelocity = Vector3.MoveTowards(ab.angularVelocity, angularVelocity, maxChange);
        }

    }

    // Get target velocity  (with estimated time).
    private Vector3 FindNewAngularVelocity()
    {
        Quaternion difference = targetRotation * Quaternion.Inverse(ab.transform.rotation);
        difference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        // Do the weird thing to account for have a range of -180 to 180
        if (angleInDegrees > 180)
            angleInDegrees -= 360;

        return (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.deltaTime;
    }

    // Is it an actual number, or is a broken number?
    private bool IsValidVelocity(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private void MoveUsingTransform()
    {
        // Prevents jitter
        ab.velocity = Vector3.zero;
        transform.localPosition = targetPosition;
    }

    private void RotateUsingTransform()
    {
        // Prevents jitter
        ab.angularVelocity = Vector3.zero;
        transform.localRotation = targetRotation;
    }

    private void OnDrawGizmos()
    {
        // Show the range at which the physics takeover
        Gizmos.DrawWireSphere(transform.position, physicsRange);
    }

    private void OnValidate()
    {
        // Just in case
        if (TryGetComponent(out Rigidbody rigidbody))
            rigidbody.useGravity = false;
    }
}