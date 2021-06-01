using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GloveListenerJoints : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _controller;

    [SerializeField]
    private GameObject[] fingers1 = new GameObject[5]; // from thumb to pinky

    [SerializeField]
    private HingeJoint[,] joints = new HingeJoint[5, 3];

    private void Start()
    {
        for (int i = 0; i < fingers1.Length; i++)
        {
            Debug.Log(fingers1[i].name);
            HingeJoint[] tmpJoints = new HingeJoint[3];
            tmpJoints[0] = fingers1[i].GetComponent<HingeJoint>();
            var tmpJointsChildren = fingers1[i].GetComponentsInChildren<HingeJoint>();
            tmpJoints[1] = tmpJointsChildren[0];
            tmpJoints[2] = tmpJointsChildren[1];
            for (int j = 0; j < joints.GetLength(1); j++)
            {
                joints[i, j] = tmpJoints[j];
            }

            if (GloveDevice.current == null)
            {
                _controller.selectAction.action.performed += FlexFingers;
                _controller.selectAction.action.canceled += UnFlexFingers;

            }
        }

    }

    private void FlexFingers(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = 1; i < joints.GetLength(0); i++)
        {
            for (int j = 0; j < joints.GetLength(1); j++)
            {
                var tmpSpring = joints[i, j].spring;
                tmpSpring.targetPosition = 40f;
                joints[i, j].spring = tmpSpring;
            }
        }
    }
    private void UnFlexFingers(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = 0; i < joints.GetLength(0); i++)
        {
            for (int j = 0; j < joints.GetLength(1); j++)
            {
                var tmpSpring = joints[i, j].spring;
                tmpSpring.targetPosition = -10f;
                joints[i, j].spring = tmpSpring;
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

        for (int i = 0; i < joints.GetLength(0); i++)
        {
            for (int j = 0; j < joints.GetLength(1); j++)
            {
                var tmpSpring = joints[i, j].spring;
                tmpSpring.targetPosition = Remap(float.Parse(fingersFlexing[i]), 0f, 100f, -10f, 80f);
                joints[i, j].spring = tmpSpring;
            }
        }

        //_handTransform.rotation = new Quaternion(
        //    float.Parse(rotation[1], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[2], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[3], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[0], CultureInfo.InvariantCulture));
        //_handTransform.rotation.Set(
        //    float.Parse(rotation[1], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[2], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[3], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[0], CultureInfo.InvariantCulture));

        //_handTransform.eulerAngles = new Vector3(
        //    float.Parse(rotation[1], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[0], CultureInfo.InvariantCulture),
        //    float.Parse(rotation[2], CultureInfo.InvariantCulture));
        state.deviceRotation = Quaternion.Euler(
           float.Parse(rotation[1], CultureInfo.InvariantCulture),
           float.Parse(rotation[0], CultureInfo.InvariantCulture),
           float.Parse(rotation[2], CultureInfo.InvariantCulture));

        state.index = float.Parse(fingersFlexing[1]) / 100;
        state.middle = float.Parse(fingersFlexing[2]) / 100;
        state.pinky = float.Parse(fingersFlexing[4]) / 100;
        state.ring = float.Parse(fingersFlexing[3]) / 100;
        state.thumb = float.Parse(fingersFlexing[0]) / 100;
        InputSystem.QueueStateEvent(GloveDevice.current, state);
    }
    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }
    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}