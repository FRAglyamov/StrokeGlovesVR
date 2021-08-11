using UnityEngine;

public class ArticulationPhysicsHand : MonoBehaviour
{

    [SerializeField]
    private GameObject handGhostModel;

    [SerializeField]
    private float showDistance = 0.05f;

    [SerializeField]
    private ArticulationBody body;

    [SerializeField]
    private Transform trackedTransform;

    [SerializeField]
    private float positionStrength = 20f;

    [SerializeField]
    private float maxVelocity = 3f;

    private float _distance;
    private GrabManagerAveragePoint _grabManager;

    private void Awake()
    {
        body = GetComponent<ArticulationBody>();
        _grabManager = GetComponent<GrabManagerAveragePoint>();
    }

    private void Start()
    {
        transform.position = trackedTransform.position;
        transform.rotation = trackedTransform.rotation;
    }

    private void Update()
    {

        _distance = Vector3.Distance(trackedTransform.position, transform.position);
        if (_distance > showDistance)
        {
            handGhostModel.SetActive(true);
        }
        else
        {
            handGhostModel.SetActive(false);
        }

        if(_distance > 1f)
        {
            body.TeleportRoot(trackedTransform.position, trackedTransform.rotation);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        //body.TeleportRoot(transform.position, trackedTransform.rotation * Quaternion.AngleAxis(-90f, Vector3.forward));
        //body.TeleportRoot(transform.position, trackedTransform.rotation);

        Quaternion rotation = trackedTransform.rotation * Quaternion.Inverse(transform.rotation);
        Vector3 angularVelocity = Vector3.ClampMagnitude((new Vector3(
          Mathf.DeltaAngle(0, rotation.eulerAngles.x),
          Mathf.DeltaAngle(0, rotation.eulerAngles.y),
          Mathf.DeltaAngle(0, rotation.eulerAngles.z))
            / Time.fixedDeltaTime) * Mathf.Deg2Rad, 10f);
        body.angularVelocity = angularVelocity;
        body.angularDamping = 5f;

        if (_grabManager == null) // TODO: remove later. Now this need for left hand, because left hand can't grab for the moment
        {
            body.velocity = Vector3.ClampMagnitude((trackedTransform.position - transform.position).normalized * positionStrength * _distance, maxVelocity);
            return;
        }

        float grabbedMass = 1f;
        if (_grabManager.grabbedObject != null)
        {
            grabbedMass = _grabManager.grabbedObject.GetComponent<Rigidbody>().mass;
            if (grabbedMass < 1f)
            {
                grabbedMass = 1f;
            }
        }
        var velocity = Vector3.ClampMagnitude((trackedTransform.position - transform.position).normalized * positionStrength * _distance / grabbedMass, maxVelocity);
        //if (_grabManager.grabbedObject != null && body.velocity.magnitude > 3f / grabbedMass)
        //{
        //    _grabManager.UnGrab();
        //}

        body.velocity = velocity;
    }
}
