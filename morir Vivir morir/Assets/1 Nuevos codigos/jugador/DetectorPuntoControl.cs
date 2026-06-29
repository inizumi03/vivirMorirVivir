using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorPuntoControl : MonoBehaviour
{
    public Fabrica fabrica;

    private void OnTriggerEnter(Collider other)
    {
        PuntoControlInvisible punto = other.GetComponent<PuntoControlInvisible>();

        if (punto != null && fabrica != null)
        {
            fabrica.ActualizarPuntoControl(punto.transform);
        }
    }
}
