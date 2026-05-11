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

    [Header("Pantalla negra")]
    public MuerteTrucha pantallaNegra;

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

        if (fabrica != null && pantallaNegra != null)
        {
            StartCoroutine(
                pantallaNegra.FadeRespawn(() =>
                {
                    fabrica.RespawnearJugador(gameObject);
                })
            );
        }
        else if (fabrica != null)
        {
            fabrica.RespawnearJugador(gameObject);
        }

        Invoke(nameof(ResetearMuerte), 0.5f);
    }

    private void ResetearMuerte()
    {
        muerto = false;
    }
}
