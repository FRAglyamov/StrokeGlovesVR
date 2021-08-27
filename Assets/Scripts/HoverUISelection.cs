using UnityEngine;
using UnityEngine.UI;

public class HoverUISelection : MonoBehaviour
{
    [SerializeField]
    private LayerMask UIMask;

    [SerializeField]
    private GameObject loadingPrefab;
    private Image _loadingImage;

    private float _requiredTime = 3f;
    private float _hitTime = 0f;
    private GameObject _hittedObject;
    private float _maxHitDistance = 20f;

    private Camera _cam;

    private void Start()
    {
        _cam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _maxHitDistance, UIMask))
        if (Physics.Raycast(ray, out hit, _maxHitDistance, UIMask))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue);

            if (_hittedObject == null || _hittedObject != hit.transform.gameObject)
            {
                _hittedObject = hit.transform.gameObject;
                _hitTime = Time.time;

                _loadingImage = Instantiate(loadingPrefab, hit.transform.position, hit.transform.rotation, hit.transform).GetComponent<Image>();
            }

            if (hit.transform.TryGetComponent(out Button button) && _hitTime <= Time.time - _requiredTime)
            {
                button.onClick.Invoke();
            }
            _loadingImage.fillAmount = (Time.time - _hitTime) / _requiredTime;

        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _maxHitDistance, Color.white);
            Debug.DrawRay(ray.origin, ray.direction * _maxHitDistance, Color.white);

            _hittedObject = null;
            _hitTime = 0f;

            if (_loadingImage != null)
            {
                Destroy(_loadingImage.gameObject);
                _loadingImage = null;
            }
        }
    }
}
