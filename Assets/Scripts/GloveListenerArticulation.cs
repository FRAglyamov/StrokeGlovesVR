using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GloveListenerArticulation : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _controller;

    [SerializeField, Tooltip("From thumb to pinky")]
    private GameObject[] fingers1 = new GameObject[5];

    private ArticulationBody[,] _articulations = new ArticulationBody[5, 3];

    [SerializeField, Tooltip("Works only without gloves, with controllers")]
    private float targetFlex = 40f; 

    private void Start()
    {
        for (int i = 0; i < fingers1.Length; i++)
        {
            //Debug.Log(fingers1[i].name);
            ArticulationBody[] tmpArticulations = new ArticulationBody[3];
            tmpArticulations[0] = fingers1[i].GetComponent<ArticulationBody>();
            var tmpJointsChildren = fingers1[i].GetComponentsInChildren<ArticulationBody>();
            tmpArticulations[1] = tmpJointsChildren[0];
            tmpArticulations[2] = tmpJointsChildren[1];
            for (int j = 0; j < _articulations.GetLength(1); j++)
            {
                _articulations[i, j] = tmpArticulations[j];
            }

            if (GloveDevice.current == null)
            {
                _controller.selectAction.action.performed += FlexFingers;
                _controller.selectAction.action.canceled += UnFlexFingers;
            }
        }

    }

    private void FlexFingers(InputAction.CallbackContext obj)
    {
        for (int i = 0; i < _articulations.GetLength(0); i++)
        {
            for (int j = 0; j < _articulations.GetLength(1); j++)
            {
                //if (articulations[i, j].GetComponent<GrabDetector>().isTouching)
                //{
                //    continue;
                //}
                var tmpXDrive = _articulations[i, j].xDrive;
                tmpXDrive.target = Mathf.Lerp(tmpXDrive.target, targetFlex, 0.1f);
                _articulations[i, j].xDrive = tmpXDrive;
            }
        }
    }
    public bool IsHaveRequiredFlex(float requiredFlex)
    {
        if (_articulations[0, 0].xDrive.target >= requiredFlex
            || _articulations[0, 0].xDrive.target >= requiredFlex
            || _articulations[0, 0].xDrive.target >= requiredFlex
            || _articulations[0, 0].xDrive.target >= requiredFlex
            || _articulations[0, 0].xDrive.target >= requiredFlex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UnFlexFingers(InputAction.CallbackContext obj)
    {
        for (int i = 0; i < _articulations.GetLength(0); i++)
        {
            for (int j = 0; j < _articulations.GetLength(1); j++)
            {
                //articulations[i, j].GetComponent<GrabDetector>().isTouching = false;
                var tmpXDrive = _articulations[i, j].xDrive;
                tmpXDrive.target = -10f;
                _articulations[i, j].xDrive = tmpXDrive;
            }
        }
    }

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        var state = new GloveDeviceState();

        Debug.Log("Arrived: " + msg);
        string[] tmp = msg.Split(',');
        string[] fingersFlexing = tmp[0].Split(' ');
        string[] rotation = tmp[1].Split(' ');

        // Set target (rotation) for articulation bodies of all phalanges
        for (int i = 0; i < _articulations.GetLength(0); i++)
        {
            for (int j = 0; j < _articulations.GetLength(1); j++)
            {
                var tmpXDrive = _articulations[i, j].xDrive;
                tmpXDrive.target = Remap(float.Parse(fingersFlexing[i]), 0f, 100f, -10f, 80f);
                _articulations[i, j].xDrive = tmpXDrive;
            }
        }

        state.deviceRotation = Quaternion.Euler(
           float.Parse(rotation[1], CultureInfo.InvariantCulture),
           float.Parse(rotation[0], CultureInfo.InvariantCulture),
           float.Parse(rotation[2], CultureInfo.InvariantCulture));

        state.thumb = float.Parse(fingersFlexing[0]) / 100;
        state.index = float.Parse(fingersFlexing[1]) / 100;
        state.middle = float.Parse(fingersFlexing[2]) / 100;
        state.ring = float.Parse(fingersFlexing[3]) / 100;
        state.pinky = float.Parse(fingersFlexing[4]) / 100;

        InputSystem.QueueStateEvent(GloveDevice.current, state);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        //Debug.Log(success ? "Device connected" : "Device disconnected");
    }
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}