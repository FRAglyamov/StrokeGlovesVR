using UnityEngine;

public class BoxAndBlocksTest : MonoBehaviour
{
    public int CubesAmount { get; private set; } = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            CubesAmount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            CubesAmount--;
        }
    }
}
