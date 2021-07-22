using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finger"))
        {
            Destroy(gameObject);
        }
    }
}
