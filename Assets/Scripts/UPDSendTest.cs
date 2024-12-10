using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UDPSendTest : MonoBehaviour
{
    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();
    public string actualData;

    private string IP;
    public int port;

    // Text components for visual feedback (commented out)
    // public Text engineA;
    // public Text engineAHex;
    public float valueA = 125f;  // Variable para reemplazar el Slider A

    // public Text engineB;
    // public Text engineBHex;
    public float valueB = 125f;  // Variable para reemplazar el Slider B

    // public Text engineC;
    // public Text engineCHex;
    public float valueC = 125f;  // Variable para reemplazar el Slider C

    // public Text Data;

    UdpClient client;

    public bool active = false;

    public float SmoothEngine = 0.5f;

    public float A = 0, B = 0, C = 0;

    public float longg;

    public float waitStart;

    public Transform vehicle;

    // Posiciones desde donde inician los motores
    public float positionMotorA = 125f;
    public float positionMotorB = 125f;
    public float positionMotorC = 125f;

    // start from unity3d
    public void Start()
    {
        init();
    }

    public void init()
    {
        // define
        IP = "192.168.15.201";
        port = 7408;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(53342);

        // AppControlField
        mUDPDATA.mAppControlField.ConfirmCode = "55aa";
        mUDPDATA.mAppControlField.PassCode = "0000";
        mUDPDATA.mAppControlField.FunctionCode = "1301";
        // AppWhoField
        mUDPDATA.mAppWhoField.AcceptCode = "ffffffff";
        mUDPDATA.mAppWhoField.ReplyCode = "";//"00000001";
        // AppDataField
        mUDPDATA.mAppDataField.RelaTime = "00000064";
        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        mUDPDATA.mAppDataField.PortOut = "12345678";

        A = valueA;
        B = valueB;
        C = valueC;

        string HexA = DecToHexMove(A);
        string HexB = DecToHexMove(B);
        string HexC = DecToHexMove(C);

        // Update visual feedback (commented out)
        // engineAHex.text = "Engine A: " + HexA;
        // engineBHex.text = "Engine B: " + HexB;
        // engineCHex.text = "Engine C: " + HexC;

        mUDPDATA.mAppDataField.PlayMotorC = HexC;
        mUDPDATA.mAppDataField.PlayMotorA = HexA;
        mUDPDATA.mAppDataField.PlayMotorB = HexB;

        // Update visual feedback (commented out)
        // engineA.text = ((int)A).ToString();
        // engineB.text = ((int)B).ToString();
        // engineC.text = ((int)C).ToString();

        // Data.text = "Data: " + mUDPDATA.GetToString();

        sendString(mUDPDATA.GetToString());

        StartCoroutine(UpMovePlatform(waitStart));
    }

    public void ActiveSend()
    {
        active = true;
    }

    public void ResertPositionEngine()
    {
        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString(mUDPDATA.GetToString());

        mUDPDATA.mAppDataField.RelaTime = "00000064";
    }

    IEnumerator UpMovePlatform(float wait)
    {
        active = false;

        yield return new WaitForSeconds(3f);

        active = true;
    }

    void CalcularRotacion()
    {
        Quaternion rotY = vehicle.rotation;
        rotY.x = 0;
        rotY.z = 0;

        Vector3 Vb1 = vehicle.position + vehicle.rotation * Vector3.right * longg;
        Vector3 Vb2 = vehicle.position + rotY * Vector3.right * longg;
        Vector3 VbA1 = vehicle.position + vehicle.rotation * Vector3.forward * longg;
        Vector3 VbA2 = vehicle.position + rotY * Vector3.forward * longg;

        float distBA = (Vb1 - Vb2).magnitude * 2;
        float distC = (VbA1 - VbA2).magnitude * 2;

        if (vehicle.eulerAngles.z < 180 && vehicle.eulerAngles.z > 0)
        {
            B = (int)Mathf.Lerp(B, Mathf.Clamp(positionMotorB + distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
            A = (int)Mathf.Lerp(A, Mathf.Clamp(positionMotorA - distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }
        else if (vehicle.eulerAngles.z > 180 && vehicle.eulerAngles.z < 360)
        {
            B = (int)Mathf.Lerp(B, Mathf.Clamp(positionMotorB - distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
            A = (int)Mathf.Lerp(A, Mathf.Clamp(positionMotorA + distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }

        if (vehicle.eulerAngles.x < 180 && vehicle.eulerAngles.x > 0)
        {
            C = (int)Mathf.Lerp(C, Mathf.Clamp(positionMotorC + distC * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }
        else if (vehicle.eulerAngles.x > 180 && vehicle.eulerAngles.x < 360)
        {
            C = (int)Mathf.Lerp(C, Mathf.Clamp(positionMotorC - distC * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            CalcularRotacion();

            string HexA = DecToHexMove(A);
            string HexB = DecToHexMove(B);
            string HexC = DecToHexMove(C);

            // Update visual feedback (commented out)
            // engineAHex.text = "Engine A: " + HexA;
            // engineBHex.text = "Engine B: " + HexB;
            // engineCHex.text = "Engine C: " + HexC;

            mUDPDATA.mAppDataField.PlayMotorC = HexC;
            mUDPDATA.mAppDataField.PlayMotorA = HexA;
            mUDPDATA.mAppDataField.PlayMotorB = HexB;

            // Update visual feedback (commented out)
            // engineA.text = ((int)A).ToString();
            // engineB.text = ((int)B).ToString();
            // engineC.text = ((int)C).ToString();

            // Data.text = "Data: " + mUDPDATA.GetToString();

            sendString(mUDPDATA.GetToString());
        }
    }

    void OnApplicationQuit()
    {
        ResertPositionEngine();

        if (client != null)
            client.Close();
        Application.Quit();
    }

    byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    string DecToHexMove(float num)
    {
        int d = (int)((num / 5f) * 10000f);
        return "000" + d.ToString("X");
    }

    private void sendString(string message)
    {
        try
        {
            if (message != "")
            {
                print(message);
                actualData = message;
            }
        }
        catch (Exception err)
        {
            print(err.ToString());
            actualData = message;
        }
    }

    void OnDisable()
    {
        if (client != null)
            client.Close();
    }

    private void OnDrawGizmos()
    {
        Quaternion rotY = vehicle.rotation;
        rotY.x = 0;
        rotY.z = 0;

        Vector3 Vb1 = vehicle.position + vehicle.rotation * Vector3.right * longg;
        Vector3 Vb2 = vehicle.position + rotY * Vector3.right * longg;
        Vector3 VbA1 = vehicle.position + vehicle.rotation * Vector3.forward * longg;
        Vector3 VbA2 = vehicle.position + rotY * Vector3.forward * longg;

        float distBA = (Vb1 - Vb2).magnitude * 2;
        float distC = (VbA1 - VbA2).magnitude * 2;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(vehicle.position, Vb1);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(vehicle.position, VbA1);
    }
}
