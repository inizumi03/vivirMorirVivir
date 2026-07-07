using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspejoLuz : MonoBehaviour
{
    [Header("Salida de luz")]
    public Transform puntoSalida;

    public Vector3 ObtenerDireccionSalida()
    {
        if (puntoSalida != null)
            return puntoSalida.forward;

        return transform.forward;
    }

    public Vector3 ObtenerPosicionSalida()
    {
        if (puntoSalida != null)
            return puntoSalida.position;

        return transform.position;
    }
}
