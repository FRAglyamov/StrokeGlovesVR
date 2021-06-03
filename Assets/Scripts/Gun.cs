using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _barrel;
    [SerializeField] private float _speed = 40f;

    public void Fire()
    {
        GameObject spawnedBullet = Instantiate(_bullet, _barrel.position, _barrel.rotation);
        spawnedBullet.GetComponent<Rigidbody>().velocity = _speed * _barrel.forward;
        Destroy(spawnedBullet, 2f);
    }
}
