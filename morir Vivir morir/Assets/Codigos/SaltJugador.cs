using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltJugador : MonoBehaviour
{
    [Header("Salto")]
    public float fuerzaSalto = 7f;
    public int cantidadSaltosMax = 2;

    [Header("Suelo")]
    public float distanciaSuelo = 0.3f;
    public LayerMask capaSuelo;

    [Header("Animaciones")]
    public AnimJugador animJugador;

    private Rigidbody rb;

    private int saltosRestantes;
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
            IntentarSaltar();
        }
    }

    void IntentarSaltar()
    {
        if (saltosRestantes <= 0) return;

        saltosRestantes--;

        // Reinicia la velocidad vertical para que el salto sea consistente
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);

        if (animJugador != null)
        {
            animJugador.ActivarSalto();
        }
    }

    void VerificarSuelo()
    {
        enSuelo = Physics.Raycast(
            transform.position,
            Vector3.down,
            distanciaSuelo,
            capaSuelo
        );

        if (enSuelo && rb.velocity.y <= 0f)
        {
            saltosRestantes = cantidadSaltosMax;
        }
    }
}
