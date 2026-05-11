using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InmuneJugador : MonoBehaviour
{
    public Vida vidaJugador;
    public string tagDaño = "Daño";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDaño))
        {
            vidaJugador.Morir();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(tagDaño))
        {
            vidaJugador.Morir();
        }
    }
}
