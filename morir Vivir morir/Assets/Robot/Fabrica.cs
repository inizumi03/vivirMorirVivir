using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fabrica : MonoBehaviour
{
    [Header("Respawn")]
    public Transform puntoRespawn;

    [Header("Prefab cuerpo")]
    public GameObject cuerpoMuertoPrefab;

    public void RespawnearJugador(GameObject jugador)
    {
        CrearCuerpo(jugador);

        Rigidbody rb = jugador.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        jugador.transform.position = puntoRespawn.position;
        jugador.transform.rotation = puntoRespawn.rotation;
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
        {
            anim.enabled = false;
        }

        Rigidbody[] rigidbodies = cuerpo.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
