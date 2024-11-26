using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterVisualRotation : MonoBehaviour
{
    public float rotationSpeed = 20f;
    public float rotationLimit = 20f;

    private float currentRotationX = 0f;
    private float currentRotationZ = 0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            currentRotationX = Mathf.Clamp(currentRotationX + rotationSpeed * Time.deltaTime, -rotationLimit, rotationLimit);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentRotationX = Mathf.Clamp(currentRotationX - rotationSpeed * Time.deltaTime, -rotationLimit, rotationLimit);
        }
        else
        {
            currentRotationX = Mathf.Lerp(currentRotationX, 0f, rotationSpeed * Time.deltaTime / rotationLimit);
        }
        if (Input.GetKey(KeyCode.A))
        {
            currentRotationZ = Mathf.Clamp(currentRotationZ + rotationSpeed * Time.deltaTime, -rotationLimit, rotationLimit);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentRotationZ = Mathf.Clamp(currentRotationZ - rotationSpeed * Time.deltaTime, -rotationLimit, rotationLimit);
        }
        else
        {
            currentRotationZ = Mathf.Lerp(currentRotationZ, 0f, rotationSpeed * Time.deltaTime / rotationLimit);
        }
        transform.localRotation = Quaternion.Euler(currentRotationX, 0, currentRotationZ);
    }
}