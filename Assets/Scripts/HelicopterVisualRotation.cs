using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterVisualRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float maxRotationX = 20f;
    public float maxRotationZ = 20f;

    private float currentRotationX = 0f;
    private float currentRotationZ = 0f;

    void Update()
    {
        // Rotación en el eje X
        if (Input.GetKey(KeyCode.W))
        {
            currentRotationX = Mathf.Clamp(currentRotationX + rotationSpeed * Time.deltaTime, -maxRotationX, maxRotationX);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentRotationX = Mathf.Clamp(currentRotationX - rotationSpeed * Time.deltaTime, -maxRotationX, maxRotationX);
        }
        else
        {
            // Gradualmente volver a 0 grados cuando no se presiona W o S
            currentRotationX = Mathf.Lerp(currentRotationX, 0, rotationSpeed * Time.deltaTime / maxRotationX);
        }

        // Rotación en el eje Z
        if (Input.GetKey(KeyCode.A))
        {
            currentRotationZ = Mathf.Clamp(currentRotationZ + rotationSpeed * Time.deltaTime, -maxRotationZ, maxRotationZ);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentRotationZ = Mathf.Clamp(currentRotationZ - rotationSpeed * Time.deltaTime, -maxRotationZ, maxRotationZ);
        }
        else
        {
            // Gradualmente volver a 0 grados cuando no se presiona A o D
            currentRotationZ = Mathf.Lerp(currentRotationZ, 0, rotationSpeed * Time.deltaTime / maxRotationZ);
        }

        // Aplicar la rotación al objeto
        transform.localRotation = Quaternion.Euler(currentRotationX, 0, currentRotationZ);
    }
}