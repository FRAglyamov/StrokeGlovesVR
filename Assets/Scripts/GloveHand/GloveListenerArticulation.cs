using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class responsible for fingers' flex and updating fingers' target flex based on values from SerialController (Glove).
/// (or on controller.selectAction in case of testing with standart controller)
/// </summary>
public class GloveListenerArticulation : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _controller;

    [SerializeField, Tooltip("From thumb to pinky")]
    private GameObject[] fingers1 = new GameObject[5]; // phalanges with 1 at the end of the name in a hierarchy, closest to the palm

    private ArticulationBody[,] _articulations = new ArticulationBody[5, 3];

    [SerializeField, Tooltip("Works only without gloves, with controllers")]
    private float targetFlex = 20f;

    private SerialController _serialController;

    private void Start()
    {
        AssignAriculationBodyReferences();

        if (GloveDevice.current == null)
        {
            SetFlexByControllers();
        }

        _serialController = AssistantSystem.Instance.serialController;
        _serialController.messageListener = this.gameObject;
    }

    /// <summary>
    /// Assign ArticulationBody components references of phalanges to the list.
    /// </summary>
    private void AssignAriculationBodyReferences()
    {
        for (int i = 0; i < fingers1.Length; i++)
        {
            ArticulationBody[] tmpArticulations = new ArticulationBody[3];
            tmpArticulations[0] = fingers1[i].GetComponent<ArticulationBody>();
            var tmpJointsChildren = fingers1[i].GetComponentsInChildren<ArticulationBody>();
            tmpArticulations[1] = tmpJointsChildren[0];
            tmpArticulations[2] = tmpJointsChildren[1];
            for (int j = 0; j < _articulations.GetLength(1); j++)
            {
                _articulations[i, j] = tmpArticulations[j];
            }
        }
    }

    private void SetFlexByControllers()
    {
        _controller.selectAction.action.performed += FlexFingers;
        _controller.selectAction.action.canceled += UnFlexFingers;
    }

    private void UnsetFlexByControllers()
    {
        _controller.selectAction.action.performed -= FlexFingers;
        _controller.selectAction.action.canceled -= UnFlexFingers;
    }

    private void FlexFingers(InputAction.CallbackContext obj) => FlexFingers(targetFlex);
    private void UnFlexFingers(InputAction.CallbackContext obj) => FlexFingers(-10f);

    private void FlexFingers(float flexAngle)
    {
        for (int i = 0; i < _articulations.GetLength(0); i++)
        {
            for (int j = 0; j < _articulations.GetLength(1); j++)
            {
                var tmpXDrive = _articulations[i, j].xDrive;
                tmpXDrive.target = flexAngle;
                _articulations[i, j].xDrive = tmpXDrive;
            }
        }
    }

    /// <summary>
    /// True - if at least one of the fingers is flexing for the required angle.
    /// Needed as a threshold for grabbing.
    /// </summary>
    /// <param name="requiredFlex"></param>
    /// <returns></returns>
    public bool IsHaveRequiredFlex(float requiredFlex)
    {
        for (int i = 0; i < _articulations.GetLength(0); i++)
        {
            if(_articulations[i, 0].xDrive.target >= requiredFlex)
            {
                return true;
            }
        }
        return false;
    }

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        InitGloveDeviceOnFirstMessage();
        UnsetFlexByControllers();

        var state = new GloveDeviceState();

        Debug.Log("Arrived message: " + msg);
        string[] tmp = msg.Split(',');
        string[] fingersFlexing = tmp[0].Split(' ');
        //string[] rotation = tmp[1].Split(' ');

        // Set the target (rotation) for articulation bodies of all phalanges
        for (int i = 0; i < _articulations.GetLength(0); i++)
        {
            for (int j = 0; j < _articulations.GetLength(1); j++)
            {
                var tmpXDrive = _articulations[i, j].xDrive;
                // Remark: Maybe Remap not to contant -10, 80 values? Get min, max from ArticulationBody.
                tmpXDrive.target = Remap(float.Parse(fingersFlexing[i]), 0f, 100f, -10f, 80f);
                _articulations[i, j].xDrive = tmpXDrive;
            }
        }

        // Remark: Code was used for IMU sensor. Now it isn't used. Remove?
        //state.deviceRotation = Quaternion.Euler(
        //   float.Parse(rotation[1], CultureInfo.InvariantCulture),
        //   float.Parse(rotation[0], CultureInfo.InvariantCulture),
        //   float.Parse(rotation[2], CultureInfo.InvariantCulture));

        state.thumb = float.Parse(fingersFlexing[0]) / 100;
        state.index = float.Parse(fingersFlexing[1]) / 100;
        state.middle = float.Parse(fingersFlexing[2]) / 100;
        state.ring = float.Parse(fingersFlexing[3]) / 100;
        state.pinky = float.Parse(fingersFlexing[4]) / 100;

        InputSystem.QueueStateEvent(GloveDevice.current, state);
    }

    /// <summary>
    /// Add GloveDevice to InputSystem if we don't have it and set isConnected as true.
    /// Remark: This version is for one glove.
    /// </summary>
    private void InitGloveDeviceOnFirstMessage()
    {
        if (!_serialController.isConnected && InputSystem.GetDevice("GloveDevice") == null)
        {
            InputSystem.AddDevice(new InputDeviceDescription
            {
                interfaceName = "Glove",
                product = "Glove Product"
            });
            _serialController.isConnected = true;
        }
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