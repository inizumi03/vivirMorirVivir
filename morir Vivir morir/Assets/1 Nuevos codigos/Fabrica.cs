using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fabrica : MonoBehaviour
{
    [Header("Respawn normal")]
    public Transform puntoRespawnJugador;

    [Header("Respawn si muero cargando fábrica")]
    public Transform puntoControlActual;

    [Header("Prefab cuerpo muerto")]
    public GameObject cuerpoMuertoPrefab;

    [Header("Daño")]
    public string tagDaño = "Daño";

    private Vector3 ultimaPosicionSegura;
    private Quaternion ultimaRotacionSegura;

    private bool siendoCargada = false;
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GuardarPosicionSegura();
    }

    public void GuardarPosicionSegura()
    {
        ultimaPosicionSegura = transform.position;
        ultimaRotacionSegura = transform.rotation;
    }

    public void MarcarCargada(bool estado)
    {
        siendoCargada = estado;
    }

    public void ActualizarPuntoControl(Transform nuevoPunto)
    {
        puntoControlActual = nuevoPunto;
    }

    public void RespawnearJugador(GameObject jugador)
    {
        CrearCuerpo(jugador);

        Rigidbody rbJugador = jugador.GetComponent<Rigidbody>();

        if (rbJugador != null)
        {
            rbJugador.velocity = Vector3.zero;
            rbJugador.angularVelocity = Vector3.zero;
        }

        if (siendoCargada)
        {
            Transform puntoFinal = puntoControlActual != null ? puntoControlActual : null;

            if (puntoFinal != null)
            {
                MoverFabrica(puntoFinal.position, puntoFinal.rotation);
                jugador.transform.position = puntoFinal.position;
                jugador.transform.rotation = puntoFinal.rotation;
            }
            else
            {
                MoverFabrica(ultimaPosicionSegura, ultimaRotacionSegura);
                jugador.transform.position = ultimaPosicionSegura;
                jugador.transform.rotation = ultimaRotacionSegura;
            }

            siendoCargada = false;
            return;
        }

        if (puntoRespawnJugador != null)
        {
            jugador.transform.position = puntoRespawnJugador.position;
            jugador.transform.rotation = puntoRespawnJugador.rotation;
        }
        else
        {
            jugador.transform.position = transform.position;
            jugador.transform.rotation = transform.rotation;
        }
        movJugador mov =
       jugador.GetComponent<movJugador>();

        if (mov != null)
        {
            mov.enabled = true;
        }
    }

    private void CrearCuerpo(GameObject jugador)
    {
        if (cuerpoMuertoPrefab == null) return;

        GameObject cuerpo = Instantiate(
            cuerpoMuertoPrefab,
            jugador.transform.position,
            jugador.transform.rotation
        );

        Animator anim = cuerpo.GetComponent<Animator>();
        if (anim != null)
            anim.enabled = false;
    }

    private void MoverFabrica(Vector3 posicion, Quaternion rotacion)
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = posicion;
        transform.rotation = rotacion;
    }

    public void VolverAPosicionSegura()
    {
        MoverFabrica(ultimaPosicionSegura, ultimaRotacionSegura);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(tagDaño))
            VolverAPosicionSegura();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDaño))
            VolverAPosicionSegura();
    }
}
