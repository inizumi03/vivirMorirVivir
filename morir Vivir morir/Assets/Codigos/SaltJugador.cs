using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltJugador : MonoBehaviour
{
    [Header("Salto")]
    public float fuerzaSalto = 7f;

    [Header("Suelo")]
    public Transform puntoSuelo;
    public float distanciaSuelo = 0.3f;
    public LayerMask capaSuelo;

    [Header("Animaciones")]
    public AnimJugador animJugador;

    private Rigidbody rb;
    private bool enSuelo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        VerificarSuelo();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Saltar();
        }
    }

    private void Saltar()
    {
        if (!enSuelo)
            return;

        // ANIMACION
        if (animJugador != null)
        {
            animJugador.ActivarSalto();
        }

        rb.velocity = new Vector3(
            rb.velocity.x,
            0f,
            rb.velocity.z
        );

        rb.AddForce(
            Vector3.up * fuerzaSalto,
            ForceMode.Impulse
        );

        enSuelo = false;
    }

    private void VerificarSuelo()
    {
        Vector3 origen =
            puntoSuelo != null
            ? puntoSuelo.position
            : transform.position;

        enSuelo = Physics.Raycast(
            origen,
            Vector3.down,
            distanciaSuelo,
            capaSuelo
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origen =
            puntoSuelo != null
            ? puntoSuelo.position
            : transform.position;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            origen,
            origen + Vector3.down * distanciaSuelo
        );

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(
            origen + Vector3.down * distanciaSuelo,
            0.05f
        );
    }
}
