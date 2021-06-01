using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysicsController : MonoBehaviour
{
    [SerializeField]
    public Transform controllerTransform;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        transform.position = controllerTransform.position;
        transform.rotation = controllerTransform.rotation;
    }

    private void FixedUpdate()
    {
        //rb.MovePosition(controllerTransform.position);
        //rb.MoveRotation(controllerTransform.rotation);

        rb.MoveRotation(controllerTransform.rotation * Quaternion.AngleAxis(-90f, Vector3.forward));

        Vector3 moveToHandVec = controllerTransform.position - transform.position;
        float neededSpeed = moveToHandVec.magnitude * (1.0f / Time.fixedDeltaTime);
        Vector3 neededSpeedVec = moveToHandVec.normalized * neededSpeed;
        Vector3 currentSpeedVec = rb.velocity;
        Vector3 newSpeedVec = neededSpeedVec - currentSpeedVec;
        rb.AddForce(newSpeedVec, ForceMode.VelocityChange);
    }
}
