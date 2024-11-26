using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float verticalSpeed = 5f;
    public float smoothTime = 2f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            targetPosition += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetPosition += transform.forward * -moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            targetPosition += transform.right * -moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetPosition += transform.right * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            targetPosition += transform.up * verticalSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            targetPosition += transform.up * -verticalSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            targetRotation *= Quaternion.Euler(Vector3.up * -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetRotation *= Quaternion.Euler(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothTime * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime * Time.deltaTime);
    }
}