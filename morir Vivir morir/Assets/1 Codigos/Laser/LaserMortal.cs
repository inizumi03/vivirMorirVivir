using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMortal : MonoBehaviour
{
    [Header("Referencias")]
    public Transform salidaLaser;
    public Transform finalLaser;
    public Transform cuboLaser;

    [Header("Bloqueo")]
    public LayerMask layersQueBloquean;
    public float tiempoParaLiberar = 0.15f;

    private Vector3 posicionFinalOriginal;

    private Collider bloqueadorActual;
    private float tiempoSinBloqueo = 0f;

    private void Start()
    {
        posicionFinalOriginal = finalLaser.position;
    }

    private void Update()
    {
        if (bloqueadorActual != null)
        {
            finalLaser.position =
                bloqueadorActual.ClosestPoint(salidaLaser.position);

            tiempoSinBloqueo = 0f;
        }
        else
        {
            tiempoSinBloqueo += Time.deltaTime;

            if (tiempoSinBloqueo >= tiempoParaLiberar)
            {
                finalLaser.position = posicionFinalOriginal;
            }
        }

        ActualizarLaser();
    }

    private void ActualizarLaser()
    {
        Vector3 direccion = finalLaser.position - salidaLaser.position;
        float distancia = direccion.magnitude;

        cuboLaser.position = salidaLaser.position + direccion / 2f;
        cuboLaser.rotation = Quaternion.LookRotation(direccion);

        Vector3 escala = cuboLaser.localScale;
        escala.z = distancia;
        cuboLaser.localScale = escala;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!EstaEnLayer(other.gameObject.layer)) return;

        bloqueadorActual = other;
        tiempoSinBloqueo = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!EstaEnLayer(other.gameObject.layer)) return;

        bloqueadorActual = other;
        tiempoSinBloqueo = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!EstaEnLayer(other.gameObject.layer)) return;

        if (other == bloqueadorActual)
        {
            bloqueadorActual = null;
            tiempoSinBloqueo = 0f;
        }
    }

    private bool EstaEnLayer(int layer)
    {
        return (layersQueBloquean.value & (1 << layer)) != 0;
    }
}