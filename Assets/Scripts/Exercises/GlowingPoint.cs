using UnityEngine;

public class GlowingPoint : MonoBehaviour
{
    public FingerType fingerType;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finger") && other.GetComponent<FingerTypeHolder>().type == fingerType)
        {
            Destroy(gameObject);
        }
    }
}
