using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverUISelection : MonoBehaviour
{
    [SerializeField]
    private LayerMask UIMask;

    [SerializeField]
    private GameObject loadingPrefab;
    private Image _loadingImage;

    private float _requiredTime = 2f;
    private float _hitTime = -1f;
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

            CheckHoverChange(ref hit);
            _loadingImage.fillAmount = (Time.time - _hitTime) / _requiredTime;

            if ((_hitTime + _requiredTime < Time.time ) && hit.transform.TryGetComponent(out Button button))
            {
                Debug.Log("HoverUI clicked");
                //button.onClick.Invoke();
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
