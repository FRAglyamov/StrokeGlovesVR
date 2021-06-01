using System.Collections;
using System.Collections.Generic;
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
    private float positionStrength = 20;

    [SerializeField]
    private float maxVelocity = 3f;

    private float distance;

    private void Awake()
    {
        body = GetComponent<ArticulationBody>();
    }
    private void Start()
    {
        transform.position = trackedTransform.position;
        transform.rotation = trackedTransform.rotation;
    }

    private void Update()
    {
        //palmTransform.rotation = Quaternion.Lerp(transform.rotation, trackedTransform.rotation, 0.9f);

        distance = Vector3.Distance(trackedTransform.position, transform.position);
        if (distance > showDistance)
        {
            handGhostModel.SetActive(true);
        }
        else
        {
            handGhostModel.SetActive(false);
        }
        if(distance > 1f)
        {
            //transform.position = trackedTransform.position;
            //body.immovable = true;
            body.TeleportRoot(trackedTransform.position, trackedTransform.rotation);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {

        //body.TeleportRoot(transform.position, trackedTransform.rotation * Quaternion.AngleAxis(-90f, Vector3.forward));
        //body.TeleportRoot(transform.position, trackedTransform.rotation);

        //ArticulationBody palmBody = palmTransform.GetComponent<ArticulationBody>();
        //Vector3 palmDelta = (transform.position +
        //  (trackedTransform.rotation * Vector3.back * 0.0225f) +
        //  (trackedTransform.rotation * Vector3.up * 0.0115f)) - body.worldCenterOfMass;
        //float alpha = 0.05f; // Blend between existing velocity and all new velocity
        //body.velocity *= alpha;
        //body.AddForce(Vector3.ClampMagnitude((((palmDelta / Time.fixedDeltaTime) / Time.fixedDeltaTime) * (body.mass + 3f)) * (1f - alpha), 1000f));


        Quaternion rotation = trackedTransform.rotation * Quaternion.Inverse(transform.rotation);
        Vector3 angularVelocity = Vector3.ClampMagnitude((new Vector3(
          Mathf.DeltaAngle(0, rotation.eulerAngles.x),
          Mathf.DeltaAngle(0, rotation.eulerAngles.y),
          Mathf.DeltaAngle(0, rotation.eulerAngles.z)) / Time.fixedDeltaTime) * Mathf.Deg2Rad, 10f);
        //palmBody.angularVelocity = Vector3.zero;
        //palmBody.AddTorque(angularVelocity);
        body.angularVelocity = angularVelocity;
        body.angularDamping = 5f;

        //var vel = (trackedTransform.position - body.transform.position).normalized * positionStrength * distance;
        var vel = Vector3.ClampMagnitude((trackedTransform.position - transform.position).normalized * positionStrength * distance, maxVelocity);
        body.velocity = vel;

    }
}
