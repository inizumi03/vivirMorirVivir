using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lanzarFabrica : MonoBehaviour
{
   [Header("Puntos")]
    public Transform lugarDeCarga;

    [Header("Lanzamiento")]
    public Transform direccionLanzamiento;
    public float fuerzaLanzamiento = 12f;
    public float fuerzaArriba = 2f;

    [Header("Trayectoria")]
    public LineRenderer lineaTrayectoria;
    public int puntosLinea = 25;
    public float tiempoEntrePuntos = 0.08f;

    private GameObject objetoCercano;
    private GameObject objetoAgarrado;
    private Rigidbody rbObjeto;

    private void Start()
    {
        if (lineaTrayectoria != null)
        {
            lineaTrayectoria.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objetoAgarrado == null)
            {
                Agarrar();
            }
        }

        if (Input.GetMouseButtonDown(0) && objetoAgarrado != null)
        {
            Lanzar();
        }

        ActualizarTrayectoria();
    }

    private void FixedUpdate()
    {
        if (objetoAgarrado != null && rbObjeto != null)
        {
            rbObjeto.MovePosition(lugarDeCarga.position);
            rbObjeto.MoveRotation(lugarDeCarga.rotation);
        }
    }

    private void Agarrar()
    {
        if (objetoCercano == null) return;

        objetoAgarrado = objetoCercano;
        rbObjeto = objetoAgarrado.GetComponent<Rigidbody>();

        if (rbObjeto == null)
        {
            objetoAgarrado = null;
            return;
        }

        ObjetoRespawn respawn = objetoAgarrado.GetComponent<ObjetoRespawn>();
        if (respawn != null)
        {
            respawn.ActivarFisicas();
        }

        rbObjeto.velocity = Vector3.zero;
        rbObjeto.angularVelocity = Vector3.zero;
        rbObjeto.useGravity = false;
        rbObjeto.freezeRotation = true;

        objetoAgarrado.transform.position = lugarDeCarga.position;
        objetoAgarrado.transform.rotation = lugarDeCarga.rotation;
    }

    private void Lanzar()
    {
        Rigidbody rb = rbObjeto;

        objetoAgarrado = null;
        rbObjeto = null;

        rb.useGravity = true;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 direccion = ObtenerDireccionLanzamiento();

        rb.AddForce(direccion * fuerzaLanzamiento, ForceMode.Impulse);

        if (lineaTrayectoria != null)
        {
            lineaTrayectoria.enabled = false;
        }
    }

    private Vector3 ObtenerDireccionLanzamiento()
    {
        Vector3 direccion = direccionLanzamiento != null
            ? direccionLanzamiento.forward
            : transform.forward;

        direccion = (direccion + Vector3.up * fuerzaArriba).normalized;

        return direccion;
    }

    private void ActualizarTrayectoria()
    {
        if (lineaTrayectoria == null) return;

        if (objetoAgarrado == null)
        {
            lineaTrayectoria.enabled = false;
            return;
        }

        lineaTrayectoria.enabled = true;
        lineaTrayectoria.positionCount = puntosLinea;

        Vector3 origen = lugarDeCarga.position;
        Vector3 velocidadInicial = ObtenerDireccionLanzamiento() * fuerzaLanzamiento;

        for (int i = 0; i < puntosLinea; i++)
        {
            float tiempo = i * tiempoEntrePuntos;

            Vector3 posicion = origen
                + velocidadInicial * tiempo
                + 0.5f * Physics.gravity * tiempo * tiempo;

            lineaTrayectoria.SetPosition(i, posicion);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            objetoCercano = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objetoCercano)
        {
            objetoCercano = null;
        }
    }
}
