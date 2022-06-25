using UnityEngine;

public class GlowingPoint : MonoBehaviour
{
    public FingerType fingerType;
    public SpawnPoints spawnPoints;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finger") && other.GetComponent<FingerTypeHolder>().type == fingerType)
        {
            spawnPoints.currentAmount++;
            Destroy(gameObject);
        }
    }
}
