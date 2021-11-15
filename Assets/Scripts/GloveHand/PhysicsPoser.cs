using UnityEngine;

/// <summary>
/// Class which moves the physical representation of the hand to the target position (controller).
/// </summary>
[RequireComponent(typeof(ArticulationBody))]
public class PhysicsPoser : MonoBehaviour
{
    [SerializeField]
    private float physicsRange = 0.2f;
    [SerializeField]
    private LayerMask physicsMask = 0;

    [SerializeField, Range(0, 1)]
    private float slowDownVelocity = 0.75f;
    [SerializeField, Range(0, 1)] 
    private float slowDownAngularVelocity = 0.75f;

    [SerializeField, Range(0, 100)]
    private float maxPositionChange = 75f;
    [SerializeField, Range(0, 100)]
    private float maxRotationChange = 75f;

    [SerializeField]
    private Transform controllerTransform;

    [SerializeField]
    private GameObject handGhostModel;
    [SerializeField, Tooltip("On which distance show ghost hand")]
    private float showDistance = 0.1f;

    private ArticulationBody _articulationBody;
    private GrabManagerAveragePoint _grabManager;
    private float _grabbedMass = 1f;

    private Vector3 _targetPosition = Vector3.zero;
    private Quaternion _targetRotation = Quaternion.identity;

    private void Awake()
    {
        _articulationBody = GetComponent<ArticulationBody>();
        _grabManager = GetComponent<GrabManagerAveragePoint>();
    }

    private void Start()
    {
        UpdateTracking();
        TeleportViaArticulationBody();
    }

    private void Update()
    {
        UpdateTracking();
        CheckShowingGhostHand();
    }

    private void UpdateTracking()
    {
        // Get the rotation and position from the device (from ActionBasedController)
        //targetPosition = controller.positionAction.action.ReadValue<Vector3>();
        //targetRotation = controller.rotationAction.action.ReadValue<Quaternion>();

        // Or we can just get pos, rot of controller game object
        _targetPosition = controllerTransform.position;
        _targetRotation = controllerTransform.rotation;
    }

    private void CheckShowingGhostHand()
    {
        var distance = Vector3.Distance(_targetPosition, transform.position);
        bool isShow = distance > showDistance ? true : false;
        handGhostModel.SetActive(isShow);
    }

    private void FixedUpdate()
    {
        if (WithinPhysicsRange() && IsCloseToController(1f))
        {
            GrabbingAdjustment();
            MoveUsingPhysics();
            RotateUsingPhysics();
        }
        else
        {
            TeleportViaArticulationBody();
        }
    }

    private void GrabbingAdjustment()
    {
        _grabbedMass = 1f;
        if(_grabManager == null)
        {
            Debug.LogWarning("_grabManager is null. Can't adjust grabbed mass");
            return;
        }
        if (_grabManager.grabbedObject != null)
        {
            _grabbedMass = _grabManager.grabbedObject.GetComponent<Rigidbody>().mass;
            if (_grabbedMass < 1f)
            {
                _grabbedMass = 1f;
            }
        }
    }

    public bool WithinPhysicsRange()
    {
        return Physics.CheckSphere(transform.position, physicsRange, physicsMask, QueryTriggerInteraction.Ignore);
    }

    private bool IsCloseToController(float closeDistance)
    {
        return Vector3.Distance(_targetPosition, transform.position) < closeDistance ? true : false;
    }

    private void MoveUsingPhysics()
    {
        // Prevents overshooting
        _articulationBody.velocity *= slowDownVelocity;

        Vector3 velocity = FindNewVelocity();
        if (IsValidVelocity(velocity.x))
        {
            // Figure out the max we can move, then move via velocity
            float maxChange = maxPositionChange * Time.deltaTime;
            _articulationBody.velocity = Vector3.MoveTowards(_articulationBody.velocity, velocity, maxChange);
        }
    }

    private Vector3 FindNewVelocity()
    {
        Vector3 difference = _targetPosition - transform.position;
        return difference / Time.deltaTime / _grabbedMass;
    }

    private void RotateUsingPhysics()
    {
        // Prevents overshooting
        _articulationBody.angularVelocity *= slowDownAngularVelocity;

        Vector3 angularVelocity = FindNewAngularVelocity();
        if (IsValidVelocity(angularVelocity.x))
        {
            // Figure out the max we can rotate, then move via velocity
            float maxChange = maxRotationChange * Time.deltaTime;
            _articulationBody.angularVelocity = Vector3.MoveTowards(_articulationBody.angularVelocity, angularVelocity, maxChange);
        }

    }

    /// <summary>
    /// Get target velocity  (with estimated time).
    /// </summary>
    /// <returns></returns>
    private Vector3 FindNewAngularVelocity()
    {
        Quaternion difference = _targetRotation * Quaternion.Inverse(transform.rotation);
        difference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        // Do the weird thing to account for have a range of -180 to 180
        if (angleInDegrees > 180)
        {
            angleInDegrees -= 360;
        }

        return (rotationAxis * angleInDegrees * Mathf.Deg2Rad) / Time.deltaTime / _grabbedMass;
    }

    private void TeleportViaArticulationBody()
    {
        _articulationBody.velocity = Vector3.zero;
        _articulationBody.angularVelocity = Vector3.zero;
        _articulationBody.TeleportRoot(_targetPosition, _targetRotation);
    }

    /// <summary>
    /// Is it an actual number, or is a broken number?
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool IsValidVelocity(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private void OnDrawGizmos()
    {
        // Show the range at which the physics takeover
        Gizmos.DrawWireSphere(transform.position, physicsRange);
    }
}