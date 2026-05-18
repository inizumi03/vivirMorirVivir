using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMortal : MonoBehaviour
{
    [Header("Laser")]
    public Transform puntoSalida;
    public float distanciaMaxima = 20f;
    public LineRenderer lineaLaser;

    [Header("Grosor")]
    public float grosorLaser = 0.3f;

    [Header("Capas")]
    public LayerMask capasDetectables;

    private void Start()
    {
        if (lineaLaser != null)
        {
            lineaLaser.positionCount = 2;

            // GROSOR
            lineaLaser.startWidth = grosorLaser;
            lineaLaser.endWidth = grosorLaser;
        }
    }

    private void Update()
    {
        
        ActualizarLaser();

    }

    private void ActualizarLaser()
    {
        if (puntoSalida == null || lineaLaser == null) return;

        Vector3 origen = puntoSalida.position;
        Vector3 direccion = puntoSalida.forward;

        Vector3 puntoFinal = origen + direccion * distanciaMaxima;

        if (Physics.Raycast(
            origen,
            direccion,
            out RaycastHit hit,
            distanciaMaxima,
            capasDetectables))
        {
            puntoFinal = hit.point;

            // FABRICA
            Fabrica fabrica =
                hit.collider.GetComponentInParent<Fabrica>();

            if (fabrica != null)
            {
                fabrica.VolverAPosicionSegura();

                ActualizarLinea(origen, puntoFinal);
                return;
            }

            // JUGADOR
            Vida vida =
                hit.collider.GetComponentInParent<Vida>();

            if (vida != null)
            {
                vida.Morir();
            }
        }

        ActualizarLinea(origen, puntoFinal);
    }

    private void ActualizarLinea(Vector3 inicio, Vector3 final)
    {
        lineaLaser.SetPosition(0, inicio);
        lineaLaser.SetPosition(1, final);
    }

}
