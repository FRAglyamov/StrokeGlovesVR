using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GloveListener : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _handTransform;
    int _index, _middle, _pinky, _ring, _thumb;
    private ActionBasedController _controller;
    private void Start()
    {
        _index = _animator.GetLayerIndex("Index");
        _middle = _animator.GetLayerIndex("Middle");
        _pinky = _animator.GetLayerIndex("Pinky");
        _ring = _animator.GetLayerIndex("Ring");
        _thumb = _animator.GetLayerIndex("Thumb");
    }
    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        var state = new GloveDeviceState();

        Debug.Log("Arrived: " + msg);
        string[] tmp = msg.Split(',');
        string[] fingersFlexing = tmp[0].Split(' ');
        string[] rotation = tmp[1].Split(' ');

        _animator.SetLayerWeight(_index, float.Parse(fingersFlexing[1]) / 100);
        _animator.SetLayerWeight(_middle, float.Parse(fingersFlexing[2]) / 100);
        _animator.SetLayerWeight(_pinky, float.Parse(fingersFlexing[4]) / 100);
        _animator.SetLayerWeight(_ring, float.Parse(fingersFlexing[3]) / 100);
        _animator.SetLayerWeight(_thumb, float.Parse(fingersFlexing[0]) / 100);

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

        _handTransform.eulerAngles = new Vector3(
            float.Parse(rotation[1], CultureInfo.InvariantCulture),
            float.Parse(rotation[0], CultureInfo.InvariantCulture),
            float.Parse(rotation[2], CultureInfo.InvariantCulture));
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
}