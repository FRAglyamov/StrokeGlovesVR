using UnityEngine;

public class UpVelocityMultiplyer : MonoBehaviour
{
    [SerializeField]
    private float multiplyer = 4f;
    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_rb.velocity.y > 0 && _rb.velocity.y < 0.5f)
        {
            _rb.velocity *= multiplyer;
        }
    }
}
