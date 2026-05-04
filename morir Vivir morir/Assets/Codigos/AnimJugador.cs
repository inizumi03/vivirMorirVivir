using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimJugador : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Rigidbody rb;

    [Header("Suelo")]
    public float distanciaSuelo = 0.3f;
    public LayerMask capaSuelo;

    private bool enSuelo;

    private void Update()
    {
        VerificarSuelo();
        ManejarMovimiento();
        ManejarSalto();
    }

    void VerificarSuelo()
    {
        enSuelo = Physics.Raycast(
            transform.position,
            Vector3.down,
            distanciaSuelo,
            capaSuelo
        );

        animator.SetBool("enSuelo", enSuelo);
    }

    void ManejarMovimiento()
    {
        Vector3 velocidadHorizontal = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float velocidad = velocidadHorizontal.magnitude;

        animator.SetFloat("velocidad", velocidad);
    }

    void ManejarSalto()
    {
        float velocidadY = rb.velocity.y;

        // Detecta inicio de salto
        if (!enSuelo && velocidadY > 0.1f)
        {
            animator.SetBool("caida", false);
        }

        // Detecta caída
        if (!enSuelo && velocidadY < -0.1f)
        {
            animator.SetBool("caida", true);
        }

        // Reset cuando toca suelo
        if (enSuelo)
        {
            animator.SetBool("caida", false);
        }
    }

    // Esto lo llamás desde tu código de salto
    public void ActivarSalto()
    {
        animator.SetTrigger("salto");
    }
}
