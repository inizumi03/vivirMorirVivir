using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuntoControlInvisible : MonoBehaviour
{
    [Header("Fábrica")]
    public Fabrica fabrica;

    [Header("Dinero")]
    public bool darDinero = true;
    public int cantidadDinero = 100;
    public bool soloUnaVez = true;

    private bool usado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (fabrica != null)
        {
            fabrica.ActualizarPuntoControl(transform);

            if (darDinero)
            {
                if (!soloUnaVez || !usado)
                {
                    fabrica.AgregarDinero(cantidadDinero);
                    usado = true;
                }
            }
        }
    }
}
