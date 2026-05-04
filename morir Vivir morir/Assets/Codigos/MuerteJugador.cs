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

    [Header("Cooldown")]
    public float tiempoCooldownMuerte = 1f;

    [Header("Detección de daño")]
    public string tagDaño = "Daño";

    private Rigidbody rb;
    private bool puedeMorir = true;

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
        if (!puedeMorir) return;

        puedeMorir = false;

        CrearCuerpoMuerto();
        Reaparecer();

        Invoke(nameof(ReactivarMuerte), tiempoCooldownMuerte);
    }

    private void CrearCuerpoMuerto()
    {
        if (prefabCuerpoMuerto == null) return;

        Quaternion rotacion = copiarRotacion ? transform.rotation : Quaternion.identity;

        GameObject cuerpo = Instantiate(
            prefabCuerpoMuerto,
            transform.position,
            rotacion
        );

        CuerpoMuerto cuerpoMuerto = cuerpo.GetComponent<CuerpoMuerto>();

        if (cuerpoMuerto != null)
        {
            cuerpoMuerto.ActivarMuerte();
        }
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

    private void ReactivarMuerte()
    {
        puedeMorir = true;
    }
}
