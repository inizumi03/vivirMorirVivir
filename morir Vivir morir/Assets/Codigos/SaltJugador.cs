using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltJugador : MonoBehaviour
{
    [Header("Salto")]
    public float fuerzaSalto = 7f;
    public int cantidadSaltosMax = 2;

    [Header("Suelo")]
    public Transform puntoSuelo;
    public float distanciaSuelo = 0.3f;
    public LayerMask capaSuelo;

    [Header("Animaciones")]
    public AnimJugador animJugador;

    private Rigidbody rb;
    private int saltosRestantes;
    private bool enSuelo;
    private bool esperandoEventoSalto;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        saltosRestantes = cantidadSaltosMax;
    }

    private void Update()
    {
        VerificarSuelo();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PedirSalto();
        }
    }

    private void PedirSalto()
    {
        if (saltosRestantes <= 0)
        {
            Debug.Log("NO HAY SALTOS");
            return;
        }

        if (esperandoEventoSalto)
        {
            Debug.Log("ESPERANDO EVENTO");
            return;
        }

        esperandoEventoSalto = true;

        Debug.Log("ACTIVANDO ANIMACION DE SALTO");

        if (animJugador != null)
        {
            animJugador.ActivarSalto();
        }
        else
        {
            Debug.Log("AnimJugador es NULL");
        }
    }

    public void EventoSaltar()
    {
        Debug.Log("SALTO REAL");

        if (!esperandoEventoSalto)
        {
            Debug.Log("NO ESTABA ESPERANDO EVENTO");
            return;
        }

        if (saltosRestantes <= 0)
        {
            Debug.Log("SIN SALTOS RESTANTES");
            return;
        }

        esperandoEventoSalto = false;
        saltosRestantes--;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);

        Debug.Log("FUERZA APLICADA");
    }

    private void VerificarSuelo()
    {
        Vector3 origen = puntoSuelo != null ? puntoSuelo.position : transform.position;

        enSuelo = Physics.Raycast(
            origen,
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
