using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuerteJugador : MonoBehaviour
{
    [Header("Respawn")]
    public Transform puntoRespawn;

    [Header("Cuerpo muerto")]
    public GameObject prefabCuerpoMuerto;
    public bool copiarRotacion = true;

    [Header("Detección de daño")]
    public string tagDaño = "Daño";

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDaño))
        {
            Morir();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(tagDaño))
        {
            Morir();
        }
    }

    public void Morir()
    {
        CrearCuerpoMuerto();
        Reaparecer();
    }

    private void CrearCuerpoMuerto()
    {
        if (prefabCuerpoMuerto == null) return;

        Quaternion rotacion = copiarRotacion ? transform.rotation : Quaternion.identity;

        Instantiate(
            prefabCuerpoMuerto,
            transform.position,
            rotacion
        );
    }

    private void Reaparecer()
    {
        if (puntoRespawn == null) return;

        transform.position = puntoRespawn.position;
        transform.rotation = puntoRespawn.rotation;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
