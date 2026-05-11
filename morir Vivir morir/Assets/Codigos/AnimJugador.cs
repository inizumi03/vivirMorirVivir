using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimJugador : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Rigidbody rbJugador;

    [Header("Suelo")]
    public Transform puntoSuelo;
    public float distanciaSuelo = 0.3f;
    public LayerMask capaSuelo;

    [Header("Movimiento")]
    public float velocidadMinima = 0.1f;

    private bool enSuelo;

    private void Update()
    {
        RevisarSuelo();
        ActualizarMovimiento();
    }

    private void RevisarSuelo()
    {
        Vector3 origen = puntoSuelo != null ? puntoSuelo.position : transform.position;

        enSuelo = Physics.Raycast(
            origen,
            Vector3.down,
            distanciaSuelo,
            capaSuelo
        );

        animator.SetBool("enSuelo", enSuelo);
    }

    private void ActualizarMovimiento()
    {
        if (rbJugador == null) return;

        Vector3 velocidadHorizontal = new Vector3(
            rbJugador.velocity.x,
            0f,
            rbJugador.velocity.z
        );

        animator.SetFloat("velocidad", velocidadHorizontal.magnitude);
    }

    public void ActivarSalto()
    {
        animator.SetTrigger("salto");
    }

    public void ActivarAgarre()
    {
        animator.SetTrigger("agarrar");
    }
}
