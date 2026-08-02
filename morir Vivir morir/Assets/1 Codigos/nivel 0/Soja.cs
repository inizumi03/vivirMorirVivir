using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soja : MonoBehaviour
{
    [Header("Referencias")]
    public LineRenderer lineRenderer;
    public Transform puntoSalida;

    [Header("Configuración")]
    public LayerMask capaCaja;
    public float distanciaMaxima = 20f;

    private void Update()
    {
        DibujarLaser();
    }

    private void DibujarLaser()
    {
        if (lineRenderer == null || puntoSalida == null)
            return;

        Vector3 origen = puntoSalida.position;
        Vector3 direccion = puntoSalida.forward;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origen);

        RaycastHit hit;

        if (Physics.Raycast(
            origen,
            direccion,
            out hit,
            distanciaMaxima,
            capaCaja))
        {
            // Se detiene al tocar una caja
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // Llega hasta la distancia máxima
            lineRenderer.SetPosition(
                1,
                origen + direccion * distanciaMaxima
            );
        }
    }
}
