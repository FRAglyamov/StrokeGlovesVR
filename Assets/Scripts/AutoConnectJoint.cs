using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoConnectJoint : MonoBehaviour
{
    void Awake()
    {
        var parentRB = transform.parent.GetComponent<Rigidbody>();
        if (parentRB == null)
        {
            transform.parent.parent.GetComponent<Rigidbody>();
        }
        GetComponent<Joint>().connectedBody = parentRB;
    }
}
