using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vida : MonoBehaviour
{
    [Header("Vida")]
    public bool muerto = false;

    [Header("Daño")]
    public string tagDaño = "Daño";

    [Header("Fabrica")]
    public Fabrica fabrica;

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
        if (muerto) return;

        muerto = true;

        Debug.Log("JUGADOR MUERTO");

        if (fabrica != null)
        {
            fabrica.RespawnearJugador(gameObject);
        }
        else
        {
            Debug.Log("FABRICA ES NULL");
        }

        Invoke(nameof(ResetearMuerte), 0.2f);
    }

    private void ResetearMuerte()
    {
        muerto = false;
    }
}
