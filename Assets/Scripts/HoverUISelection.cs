using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class for selection/"clicking" UI elements when the view/ray from a camera hovering them for a certain time
/// </summary>
public class HoverUISelection : MonoBehaviour
{
    [SerializeField]
    private LayerMask UIMask;

    [SerializeField]
    private GameObject loadingPrefab;
    private Image _loadingImage;

    private float _requiredTime = 1f; // Seconds, required for selection.
    private float _hitTime = -1f; // How long we already hitting the same GO.
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

        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _maxHitDistance, UIMask))
        if (Physics.Raycast(ray, out RaycastHit hit, _maxHitDistance, UIMask))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue);

            CheckHoverChange(ref hit);
            _loadingImage.fillAmount = (Time.time - _hitTime) / _requiredTime;

            if ((_hitTime + _requiredTime < Time.time ) && hit.transform.TryGetComponent(out Button button))
            {
                IPointerClickHandler clickHandler = button.gameObject.GetComponent<IPointerClickHandler>();
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                clickHandler.OnPointerClick(pointerEventData);
                ResetHit();
            }
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _maxHitDistance, Color.white);
            Debug.DrawRay(ray.origin, ray.direction * _maxHitDistance, Color.white);

            ResetHit();
        }
    }

    private void ResetHit()
    {
        _hittedObject = null;
        _hitTime = -1f;

        DestroyLoadingImage();
    }

    /// <summary>
    /// Reset the time and loading image if the hovering object was changed.
    /// </summary>
    /// <param name="hit"></param>
    private void CheckHoverChange(ref RaycastHit hit)
    {
        if (_hittedObject == null || _hittedObject != hit.transform.gameObject)
        {
            _hittedObject = hit.transform.gameObject;
            _hitTime = Time.time;

            DestroyLoadingImage();
            _loadingImage = Instantiate(loadingPrefab, hit.transform.position, hit.transform.rotation, hit.transform).GetComponent<Image>();
        }
    }

    private void DestroyLoadingImage()
    {
        if (_loadingImage != null)
        {
            Destroy(_loadingImage.gameObject);
            _loadingImage = null;
        }
    }
}
