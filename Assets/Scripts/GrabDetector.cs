using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDetector : MonoBehaviour
{
    public bool isTouching = false;

    [SerializeField]
    GrabPhysics grabPhysics;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == grabPhysics.grabbedObject)
        {
            isTouching = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == grabPhysics.grabbedObject)
        {
            isTouching = false;
        }
    }
}
