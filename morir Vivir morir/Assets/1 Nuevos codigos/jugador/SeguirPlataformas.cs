using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguirPlataformas : MonoBehaviour
{
    private TrampaMovil plataformaActual;
    private Vector3 posicionAnteriorPlataforma;

    private void OnCollisionEnter(Collision collision)
    {
        TrampaMovil plataforma =
            collision.collider.GetComponentInParent<TrampaMovil>();

        if (plataforma != null)
        {
            plataformaActual = plataforma;
            posicionAnteriorPlataforma = plataforma.transform.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (plataformaActual == null) return;

        TrampaMovil plataforma =
            collision.collider.GetComponentInParent<TrampaMovil>();

        if (plataforma != plataformaActual) return;

        Vector3 movimientoPlataforma =
            plataformaActual.transform.position - posicionAnteriorPlataforma;

        transform.position += movimientoPlataforma;

        posicionAnteriorPlataforma =
            plataformaActual.transform.position;
    }

    private void OnCollisionExit(Collision collision)
    {
        TrampaMovil plataforma =
            collision.collider.GetComponentInParent<TrampaMovil>();

        if (plataforma == plataformaActual)
        {
            plataformaActual = null;
        }
    }
}
