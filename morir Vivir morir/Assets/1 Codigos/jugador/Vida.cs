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

    [Header("Movimiento")]
    public movJugador Movimiento;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag(tagDaño))
        {
            Morir();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(tagDaño))
        {
            Morir();
        }
    }

    public void Morir()
    {
        if (muerto) return;

        muerto = true;

        DetenerMovimiento();

        if (fabrica != null && pantallaNegra != null)
        {
            StartCoroutine(
                pantallaNegra.FadeRespawn(() =>
                {
                    fabrica.RespawnearJugador(gameObject);
                    ActivarMovimiento();
                })
            );
        }
        else if (fabrica != null)
        {
            fabrica.RespawnearJugador(gameObject);
            ActivarMovimiento();
        }

        Invoke(nameof(ResetearMuerte), 0.5f);
    }

    private void ResetearMuerte()
    {
        muerto = false;
    }

    private void DetenerMovimiento()
    {
        if (Movimiento != null)
        {
            Movimiento.BloquearMovimiento();
        }
    }

    private void ActivarMovimiento()
    {
        if (Movimiento != null)
        {
            Movimiento.DesbloquearMovimiento();
        }
    }
}
