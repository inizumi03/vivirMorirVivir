using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nocaerse : MonoBehaviour
{
    private Transform plataformaActual;
    private Quaternion rotacionFija;

    private void OnCollisionEnter(Collision collision)
    {
        TrampaMovil plataforma =
            collision.collider.GetComponentInParent<TrampaMovil>();

        if (plataforma != null)
        {
            plataformaActual = plataforma.transform;

            // Guarda la rotación actual del clon
            rotacionFija = transform.rotation;

            // Se hace hijo de la plataforma para seguir su movimiento
            transform.SetParent(plataformaActual, true);
        }
    }

    private void LateUpdate()
    {
        if (plataformaActual != null)
        {
            // Mantiene siempre la misma rotación
            transform.rotation = rotacionFija;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        TrampaMovil plataforma =
            collision.collider.GetComponentInParent<TrampaMovil>();

        if (plataforma != null &&
            plataforma.transform == plataformaActual)
        {
            transform.SetParent(null, true);
            plataformaActual = null;
        }
    }
}