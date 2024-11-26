using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionContinuaY : MonoBehaviour
{
    public float velocidadRotacion = 50f; 

    void Update()
    {
        transform.Rotate(0, velocidadRotacion * Time.deltaTime, 0);
    }
}