using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAnimationController : MonoBehaviour
{
    private ActionBasedController _controller;

    [SerializeField]
    private Animator _animator;

    void Start()
    {
        _controller = GetComponent<ActionBasedController>();
        if (_animator == null)
            _animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(GloveDevice.current==null)
            UpdateHandAnimation();
    }

    void UpdateHandAnimation()
    {
        float triggerValue = _controller.activateAction.action.ReadValue<float>();
        _animator.SetFloat("Trigger", triggerValue);
        float gripValue = _controller.selectAction.action.ReadValue<float>();
        _animator.SetFloat("Grip", gripValue);
    }
}
