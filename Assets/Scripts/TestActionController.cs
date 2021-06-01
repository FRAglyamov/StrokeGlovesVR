using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestActionController : MonoBehaviour
{
    private ActionBasedController _controller;

    void Start()
    {
        _controller = GetComponent<ActionBasedController>();
        //bool isPressed = _controller.selectAction.action.ReadValue<bool>();

        _controller.selectAction.action.performed += PrintInDebugLog;
    }

    private void PrintInDebugLog(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Select button is pressed");
    }
}
