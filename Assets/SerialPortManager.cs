using System.IO.Ports;
using UnityEngine;

public class SerialPortManager : MonoBehaviour
{
    [SerializeField]
    private SerialController rightHandSerialController;
    [SerializeField]
    private SerialController leftHandSerialController;

    private void Start()
    {
        string[] ports = SerialPort.GetPortNames();
        Debug.Log("The following serial ports were found:");
        foreach (string port in ports)
        {
            Debug.Log(port);
        }

    }
    private void TryConnectToCOMs()
    {
        /// ���� ��� �����.
        /// ���������� � ������ ��� ������� �����.
        /// ��� ������. ���� �����, ��������, GloveRight, �� ��������� ��� ���� ���������/Serial controller'� ������ ��������.
        /// ����� ����� ��� ��������, ��������� ��������� ������.
        /// (��� ��������� ����� ���� ������, ��������� ������ ������ � �������� ������)
        /// ���� ����� ������� ������ ���� ������ � ���������, ����� ������������ ��� �������� ������
        string[] ports = SerialPort.GetPortNames();
        foreach (var port in ports)
        {
            //Debug.Log($"Create SerialController with port = {port}");
            //CreateSerialController(port);
        }
    }
    //private void CreateSerialController(string port)
    //{
    //    var controller = gameObject.AddComponent<SerialController>();
    //    controller.portName = port;
    //}
    //private void OnMessageArrived(string message)
    //{
    //    if (message == "GloveRight")
    //    {

    //    }
    //}
    public void StartCalibration()
    {
        leftHandSerialController.SendSerialMessage("Calibation");
        rightHandSerialController.SendSerialMessage("Calibation");
    }
}
