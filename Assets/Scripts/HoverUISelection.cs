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

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _maxHitDistance, UIMask) 
            && hit.transform.TryGetComponent(out Button button))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

            if (_hittedObject == null || _hittedObject != hit.transform.gameObject)
            {
                _hittedObject = hit.transform.gameObject;
                _hitTime = Time.time;

                _loadingImage = Instantiate(loadingPrefab, hit.transform.position, hit.transform.rotation, hit.transform).GetComponent<Image>();
            }
            if(_hitTime + _requiredTime >= Time.time)
            {
                // Select/Activate UI element
                button.onClick.Invoke();
            }
            // Draw line (or not?) and progress of timer
            _loadingImage.fillAmount = (Time.time - _hitTime) / _requiredTime;

        }
        else
        {

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _maxHitDistance, Color.white);
            Debug.Log("Did not Hit");

            _hittedObject = null;
            _hitTime = 0f;

            Destroy(_loadingImage.gameObject);
            _loadingImage = null;
        }
    }
}
