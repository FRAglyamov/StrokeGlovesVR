using UnityEngine;

/// <summary>
/// Class which move physical representation of hand to target position (controller).
/// </summary>
[RequireComponent(typeof(ArticulationBody))]
public class ArticulationPhysicsHand : MonoBehaviour
{

    [SerializeField]
    private GameObject handGhostModel;

    [SerializeField, Tooltip("On which distance show ghost hand")]
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
    private float _grabbedMass = 1f;

    private void Start()
    {
        body = GetComponent<ArticulationBody>();
        _grabManager = GetComponent<GrabManagerAveragePoint>();

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

        _grabbedMass = 1f;
        if (_grabManager != null && _grabManager.grabbedObject != null) // TODO: remove "_grabManager != null" later. Now this need, because left hand can't grab
        {
            _grabbedMass = _grabManager.grabbedObject.GetComponent<Rigidbody>().mass;
            if (_grabbedMass < 1f)
            {
                _grabbedMass = 1f;
            }
        }

        Quaternion rotation = trackedTransform.rotation * Quaternion.Inverse(transform.rotation);
        Vector3 angularVelocity = Vector3.ClampMagnitude((new Vector3(
          Mathf.DeltaAngle(0, rotation.eulerAngles.x),
          Mathf.DeltaAngle(0, rotation.eulerAngles.y),
          Mathf.DeltaAngle(0, rotation.eulerAngles.z))
            / Time.fixedDeltaTime / _grabbedMass) * Mathf.Deg2Rad, 10f);
        body.angularVelocity = angularVelocity;
        body.angularDamping = 5f;

        body.velocity = 
            Vector3.ClampMagnitude(
                (trackedTransform.position - transform.position).normalized * positionStrength * _distance / _grabbedMass, 
                maxVelocity);
    }
}
