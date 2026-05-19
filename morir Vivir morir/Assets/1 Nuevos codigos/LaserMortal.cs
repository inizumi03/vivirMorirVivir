using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMortal : MonoBehaviour
{
    [Header("Referencia")]
    public Transform puntoSalida;
    public Transform cuboLaser;

    [Header("Distancia")]
    public float distanciaMaxima = 20f;

    [Header("Detección")]
    public LayerMask capasDetectables;

    private Vector3 escalaOriginal;

    private void Start()
    {
        if (cuboLaser != null)
        {
            escalaOriginal = cuboLaser.localScale;
        }
    }

    private void Update()
    {
        ActualizarLaser();
    }

    private void ActualizarLaser()
    {
        if (puntoSalida == null || cuboLaser == null) return;

        Vector3 origen = puntoSalida.position;
        Vector3 direccion = puntoSalida.forward;

        float distanciaFinal = distanciaMaxima;

        if (Physics.Raycast(
            origen,
            direccion,
            out RaycastHit hit,
            distanciaMaxima,
            capasDetectables))
        {
            distanciaFinal = hit.distance;

            // FABRICA
            Fabrica fabrica =
                hit.collider.GetComponentInParent<Fabrica>();

            if (fabrica != null)
            {
                fabrica.VolverAPosicionSegura();
            }

            // JUGADOR
            Vida vida =
                hit.collider.GetComponentInParent<Vida>();

            if (vida != null)
            {
                vida.Morir();
            }
        }

        Vector3 centro =
            origen + direccion * (distanciaFinal / 2f);

        cuboLaser.position = centro;
        cuboLaser.rotation = puntoSalida.rotation;

        // SOLO CAMBIA EL LARGO Z
        cuboLaser.localScale = new Vector3(
            escalaOriginal.x,
            escalaOriginal.y,
            distanciaFinal
        );
    }
}
