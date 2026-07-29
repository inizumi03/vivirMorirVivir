using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SombraJugador : MonoBehaviour
{
    [Header("Jugador")]
    public Transform jugador;
    public Rigidbody rigidbodyJugador;

    [Header("Detección del suelo")]
    public LayerMask capasSuelo;
    public float distanciaMaxima = 30f;
    public float alturaInicioRaycast = 0.5f;
    public float separacionDelSuelo = 0.02f;

    [Header("Tamaño de la sombra")]
    public Vector3 escalaNormal = new Vector3(1f, 1f, 1f);
    public Vector3 escalaSubiendo = new Vector3(1.5f, 1.5f, 1.5f);
    public Vector3 escalaCayendo = new Vector3(0.6f, 0.6f, 0.6f);

    [Header("Movimiento")]
    public float velocidadPosicion = 20f;
    public float velocidadEscala = 8f;
    public float velocidadRotacion = 15f;

    [Header("Detección del salto")]
    public float velocidadMinimaVertical = 0.1f;

    [Header("Opciones")]
    public bool ocultarSiNoHaySuelo = true;

    private Renderer rendererSombra;
    private Vector3 ultimaPosicionJugador;
    private float velocidadVerticalCalculada;

    private void Start()
    {
        rendererSombra = GetComponentInChildren<Renderer>();

        if (jugador != null)
        {
            ultimaPosicionJugador = jugador.position;
        }

        transform.localScale = escalaNormal;
    }

    private void LateUpdate()
    {
        if (jugador == null)
            return;

        CalcularVelocidadVertical();
        BuscarSuperficie();
        ActualizarEscala();
    }

    private void CalcularVelocidadVertical()
    {
        if (rigidbodyJugador != null)
        {
            velocidadVerticalCalculada = rigidbodyJugador.velocity.y;
        }
        else
        {
            velocidadVerticalCalculada =
                (jugador.position.y - ultimaPosicionJugador.y) /
                Mathf.Max(Time.deltaTime, 0.0001f);
        }

        ultimaPosicionJugador = jugador.position;
    }

    private void BuscarSuperficie()
    {
        Vector3 origenRaycast =
            jugador.position + Vector3.up * alturaInicioRaycast;

        RaycastHit hit;

        bool encontroSuelo = Physics.Raycast(
            origenRaycast,
            Vector3.down,
            out hit,
            distanciaMaxima,
            capasSuelo,
            QueryTriggerInteraction.Ignore
        );

        if (!encontroSuelo)
        {
            MostrarSombra(false);
            return;
        }

        MostrarSombra(true);

        Vector3 posicionDestino =
            hit.point + hit.normal * separacionDelSuelo;

        transform.position = Vector3.Lerp(
            transform.position,
            posicionDestino,
            velocidadPosicion * Time.deltaTime
        );

        Quaternion rotacionDestino =
            Quaternion.FromToRotation(Vector3.up, hit.normal);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionDestino,
            velocidadRotacion * Time.deltaTime
        );
    }

    private void ActualizarEscala()
    {
        Vector3 escalaDestino;

        if (velocidadVerticalCalculada > velocidadMinimaVertical)
        {
            // El jugador está subiendo.
            escalaDestino = escalaSubiendo;
        }
        else if (velocidadVerticalCalculada < -velocidadMinimaVertical)
        {
            // El jugador está cayendo.
            escalaDestino = escalaCayendo;
        }
        else
        {
            // El jugador está quieto verticalmente.
            escalaDestino = escalaNormal;
        }

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaDestino,
            velocidadEscala * Time.deltaTime
        );
    }

    private void MostrarSombra(bool mostrar)
    {
        if (!ocultarSiNoHaySuelo)
            mostrar = true;

        if (rendererSombra != null)
        {
            rendererSombra.enabled = mostrar;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (jugador == null)
            return;

        Vector3 origen =
            jugador.position + Vector3.up * alturaInicioRaycast;

        Gizmos.DrawLine(
            origen,
            origen + Vector3.down * distanciaMaxima
        );
    }
}
