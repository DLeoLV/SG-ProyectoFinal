using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class UDPSendTest : MonoBehaviour
{

    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();


    private string IP;  // define in init
    public int port;  // define in init
    public Text engineA;
    public Text engineAHex;
    public Slider sliderA;
    public Text engineB;
    public Text engineBHex;
    public Slider sliderB;
    public Text engineC;
    public Text engineCHex;
    public Slider sliderC;

    public Text Data;

    UdpClient client;

    public bool active = false;

    public float SmoothEngine = 0.5f;

    public float A = 0, B = 0, C = 0 ;

    public float longg;

    public float waitStart;

    public Transform vehicle;

    //Posiciones desde donde inician los motores
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

        A = 125;
        B = 125;
        C = 125;

        sliderA.value = A;
        sliderB.value = B;
        sliderC.value = C;

        string HexA = DecToHexMove(A);
        string HexB = DecToHexMove(B);
        string HexC = DecToHexMove(C);

        engineAHex.text = "Engine A: " + HexA;
        engineBHex.text = "Engine B: " + HexB;
        engineCHex.text = "Engine C: " + HexC;

        mUDPDATA.mAppDataField.PlayMotorC = HexC;
        mUDPDATA.mAppDataField.PlayMotorA = HexA;
        mUDPDATA.mAppDataField.PlayMotorB = HexB;


        engineA.text = ((int)sliderA.value).ToString();
        engineB.text = ((int)sliderB.value).ToString();
        engineC.text = ((int)sliderC.value).ToString();

        Data.text = "Data: " + mUDPDATA.GetToString();

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
    void CalcularAltitud()
    {

        //valueMotor = 125;

        //A = (float)(Mathf.Clamp(valueMotor, 0, 250));
        //B = (float)(Mathf.Clamp(valueMotor, 0, 250));
        //C = (float)(Mathf.Clamp(valueMotor, 0, 250));

    }
    void CalcularRotacion()
    {
        // Anular la rotación en los ejes X y Z, manteniendo solo la rotación en Y.
        // Esto se hace para trabajar solo con la rotación en torno al eje Y (giro alrededor del eje vertical).
        Quaternion rotY = vehicle.rotation;
        rotY.x = 0; // Eliminar la rotación en el eje X.
        rotY.z = 0; // Eliminar la rotación en el eje Z.

        // Calcular las posiciones desplazadas del vehículo en el espacio:
        // - Vb1: Posición del vehículo desplazada a lo largo del eje X original (con la rotación completa del vehículo).
        // - Vb2: Posición del vehículo desplazada a lo largo del eje X pero sin la rotación en X y Z (solo usando la rotación en Y).
        // - VbA1: Posición del vehículo desplazada a lo largo del eje Z original.
        // - VbA2: Posición del vehículo desplazada a lo largo del eje Z pero sin la rotación en X y Z.
        Vector3 Vb1 = vehicle.position + vehicle.rotation * Vector3.right * longg;
        Vector3 Vb2 = vehicle.position + rotY * Vector3.right * longg;
        Vector3 VbA1 = vehicle.position + vehicle.rotation * Vector3.forward * longg;
        Vector3 VbA2 = vehicle.position + rotY * Vector3.forward * longg;

        // Calcular las distancias entre las posiciones desplazadas, multiplicadas por 2:
        // - distBA: Distancia entre las posiciones Vb1 y Vb2, que está relacionada con el giro en Y (rotación alrededor del eje vertical).
        // - distC: Distancia entre las posiciones VbA1 y VbA2, que está relacionada con el giro en X (rotación en el eje horizontal).
        float distBA = (Vb1 - Vb2).magnitude * 2; // Distancia relacionada con la rotación en Y.
        float distC = (VbA1 - VbA2).magnitude * 2; // Distancia relacionada con la rotación en X.

        // Ajustar los motores A y B según el giro en Z (subir o bajar dependiendo del ángulo):
        // El ángulo de rotación en Z determina la dirección del giro, por lo que ajustamos las posiciones de los motores A y B.
        if (vehicle.eulerAngles.z < 180 && vehicle.eulerAngles.z > 0)
        {
            // Si el vehículo está girando en sentido horario (Z entre 0 y 180 grados),
            // aumentamos la velocidad del motor B y reducimos la del motor A.
            // Las distancias calculadas son multiplicadas por un factor (10 en este caso) para obtener el ajuste deseado.
            // Utilizamos Mathf.Lerp para una transición suave entre la velocidad actual y la nueva velocidad.
            B = (int)Mathf.Lerp(B, Mathf.Clamp(positionMotorB + distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
            A = (int)Mathf.Lerp(A, Mathf.Clamp(positionMotorA - distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }
        else if (vehicle.eulerAngles.z > 180 && vehicle.eulerAngles.z < 360)
        {
            // Si el vehículo está girando en sentido antihorario (Z entre 180 y 360 grados),
            // reducimos la velocidad del motor B y aumentamos la del motor A.
            B = (int)Mathf.Lerp(B, Mathf.Clamp(positionMotorB - distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
            A = (int)Mathf.Lerp(A, Mathf.Clamp(positionMotorA + distBA * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }

        // Ajustar el motor C dependiendo del giro en X (movimiento hacia arriba o hacia abajo):
        // El ángulo de rotación en X determina si el vehículo está subiendo o bajando,
        // por lo que ajustamos la velocidad del motor C para contrarrestar este movimiento.
        if (vehicle.eulerAngles.x < 180 && vehicle.eulerAngles.x > 0)
        {
            // Si el vehículo está subiendo (X entre 0 y 180 grados), aumentamos la velocidad del motor C.
            C = (int)Mathf.Lerp(C, Mathf.Clamp(positionMotorC + distC * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }
        else if (vehicle.eulerAngles.x > 180 && vehicle.eulerAngles.x < 360)
        {
            // Si el vehículo está bajando (X entre 180 y 360 grados), reducimos la velocidad del motor C.
            C = (int)Mathf.Lerp(C, Mathf.Clamp(positionMotorC - distC * 10, 0, 250), Time.deltaTime * SmoothEngine);
        }
    }

    void FixedUpdate()
    {
        if (active)
        {

            CalcularRotacion();

            sliderA.value = A;
            sliderB.value = B;
            sliderC.value = C;
 
            string HexA = DecToHexMove(A);
            string HexB = DecToHexMove(B);
            string HexC = DecToHexMove(C);

            engineAHex.text = "Engine A: " + HexA;
            engineBHex.text = "Engine B: " + HexB;
            engineCHex.text = "Engine C: " + HexC;

            mUDPDATA.mAppDataField.PlayMotorC = HexC;
            mUDPDATA.mAppDataField.PlayMotorA = HexA;
            mUDPDATA.mAppDataField.PlayMotorB = HexB;


            engineA.text = ((int)sliderA.value).ToString();
            engineB.text = ((int)sliderB.value).ToString();
            engineC.text = ((int)sliderC.value).ToString();

            Data.text = "Data: " + mUDPDATA.GetToString();

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
            // Bytes empfangen.
            if (message != "")
            {

                //byte[] data = StringToByteArray(message);
                print(message);
                // Den message zum Remote-Client senden.
                //client.Send(data, data.Length, remoteEndPoint);

            }


        }
        catch (Exception err)
        {
            print(err.ToString());
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

        // rotate left or Right
        Vector3 Vb1 = vehicle.position + vehicle.rotation * Vector3.right * longg;
        Vector3 Vb2 = vehicle.position + rotY * Vector3.right * longg;

        // rotate forward or back
        Vector3 VbA1 = vehicle.position + vehicle.rotation * Vector3.forward * longg;
        Vector3 VbA2 = vehicle.position + rotY * Vector3.forward * longg;


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vb1, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, Vb1);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Vb2, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, Vb2);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(VbA1, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, VbA1);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(VbA2, 0.5f);
        Gizmos.DrawLine(vehicle.transform.position, VbA2);

    }

}