using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

public struct GloveDeviceState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('C', 'U', 'S', 'T');

    //[InputControl(name = "index", layout = "Axis")]
    //[InputControl(name = "middle", layout = "Axis")]
    //[InputControl(name = "pinky", layout = "Axis")]
    //[InputControl(name = "ring", layout = "Axis")]
    //[InputControl(name = "thumb", layout = "Axis")]
    //public short fingersAxis;
    [InputControl(layout = "Axis")]
    public float index;
    [InputControl(layout = "Axis")]
    public float middle;
    [InputControl(layout = "Axis")]
    public float pinky;
    [InputControl(layout = "Axis")]
    public float ring;
    [InputControl(layout = "Axis")]
    public float thumb;
    //[InputControl(name = "quatX", layout = "Axis")]
    //[InputControl(name = "quatY", layout = "Axis")]
    //[InputControl(name = "quatZ", layout = "Axis")]
    //[InputControl(name = "quatW", layout = "Axis")]
    //public Quaternion rotation;
    //[InputControl(layout = "Axis")]
    //public Quaternion quatX;
    //[InputControl(layout = "Axis")]
    //public float quatY;
    //[InputControl(layout = "Axis")]
    //public float quatZ;
    //[InputControl(layout = "Axis")]
    //public float quatW;
}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(stateType = typeof(GloveDeviceState))]
public class GloveDevice : InputDevice, IInputUpdateCallbackReceiver
{
#if UNITY_EDITOR
    static GloveDevice()
    {
        Initialize();
    }

#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        InputSystem.RegisterLayout<GloveDevice>(
            matches: new InputDeviceMatcher()
                .WithInterface("Glove"));
    }
    public AxisControl index { get; private set; }
    public AxisControl middle { get; private set; }
    public AxisControl pinky { get; private set; }
    public AxisControl ring { get; private set; }
    public AxisControl thumb { get; private set; }
    //public QuaternionControl quatX { get; private set; }
    //public QuaternionControl quatY { get; private set; }
    //public QuaternionControl quatZ { get; private set; }
    //public QuaternionControl quatW { get; private set; }
    //public QuaternionControl rotation { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        index = GetChildControl<AxisControl>("index");
        middle = GetChildControl<AxisControl>("middle");
        pinky = GetChildControl<AxisControl>("pinky");
        ring = GetChildControl<AxisControl>("ring");
        thumb = GetChildControl<AxisControl>("thumb");
        //quatX = GetChildControl<QuaternionControl>("quatX");
        //quatY = GetChildControl<QuaternionControl>("quatY");
        //quatZ = GetChildControl<QuaternionControl>("quatZ");
        //quatW = GetChildControl<QuaternionControl>("quatW");
        //rotation = GetChildControl<QuaternionControl>("rotation");
    }

    public static GloveDevice current { get; private set; }
    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this)
            current = null;
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Glove Device/Create Device")]
    private static void CreateDevice()
    {
        InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "Glove",
            product = "Glove Product"
        });
    }

    [MenuItem("Tools/Glove Device/Remove Device")]
    private static void RemoveDevice()
    {
        var gloveDevice = InputSystem.devices.FirstOrDefault(x => x is GloveDevice);
        if (gloveDevice != null)
            InputSystem.RemoveDevice(gloveDevice);
    }

#endif

    public void OnUpdate()
    {
        //var state = new GloveDeviceState();
        //Debug.Log(ring.ReadValue());
        //InputSystem.QueueStateEvent(this, state);
    }
}
