using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimJugador : MonoBehaviour
{
    [Header("Animators por forma")]
    public Animator animatorBase;
    public Animator animatorSalto;
    public Animator animatorMetal;

    [Header("Referencias")]
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

    private Animator ObtenerAnimatorActivo()
    {
        if (animatorBase != null && animatorBase.gameObject.activeInHierarchy)
            return animatorBase;

        if (animatorSalto != null && animatorSalto.gameObject.activeInHierarchy)
            return animatorSalto;

        if (animatorMetal != null && animatorMetal.gameObject.activeInHierarchy)
            return animatorMetal;

        return null;
    }

    private void RevisarSuelo()
    {
        Animator animator = ObtenerAnimatorActivo();
        if (animator == null) return;

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
        Animator animator = ObtenerAnimatorActivo();
        if (animator == null) return;
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
        Animator animator = ObtenerAnimatorActivo();
        if (animator != null)
        {
            animator.SetTrigger("salto");
        }
    }

    public void ActivarAgarre()
    {
        Animator animator = ObtenerAnimatorActivo();
        if (animator != null)
        {
            animator.SetTrigger("agarrar");
        }
    }
}
