using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionContinuaZ : MonoBehaviour
{
    public float velocidadRotacion = 50f;

    void Update()
    {
        transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);
    }
}
