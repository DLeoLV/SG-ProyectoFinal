using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoRotation : MonoBehaviour
{
    public Transform objeto;
    void Update()
    {
        transform.rotation = objeto.rotation;
    }
}