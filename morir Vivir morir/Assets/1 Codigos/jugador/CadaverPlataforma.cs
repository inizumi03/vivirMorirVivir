using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CadaverPlataforma : MonoBehaviour
{
    [Header("Configuración")]
    public bool mantenerRotacion = true;

    [Tooltip("Tag utilizado por el jugador.")]
    public string tagJugador = "Player";

    [Tooltip("Normal mínima para considerar que el jugador está encima.")]
    [Range(0f, 1f)]
    public float normalMinima = 0.45f;

    [Tooltip("Distancia máxima permitida para seguir considerándolo encima.")]
    public float distanciaMaximaEncima = 1.5f;

    private Rigidbody rb;

    private bool dentroDelLaser;

    private bool gravedadOriginal;
    private bool kinematicOriginal;
    private RigidbodyConstraints restriccionesOriginales;

    private Quaternion rotacionBloqueada;

    private Transform jugadorEncima;
    private Rigidbody rbJugador;
    private Transform padreOriginalJugador;

    private readonly HashSet<Collider> collidersJugador =
        new HashSet<Collider>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody>();
        }

        if (rb != null)
        {
            gravedadOriginal = rb.useGravity;
            kinematicOriginal = rb.isKinematic;
            restriccionesOriginales = rb.constraints;
        }

        rotacionBloqueada = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (!dentroDelLaser)
            return;

        MantenerCadaverEstable();
        ComprobarJugadorEncima();
    }

    public void ActivarComoPlataforma()
    {
        if (dentroDelLaser)
            return;

        dentroDelLaser = true;
        rotacionBloqueada = transform.rotation;

        if (rb == null)
            return;

        gravedadOriginal = rb.useGravity;
        kinematicOriginal = rb.isKinematic;
        restriccionesOriginales = rb.constraints;

       
        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        rb.useGravity = false;
        rb.isKinematic = true;

        if (mantenerRotacion)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void DesactivarComoPlataforma()
    {
        dentroDelLaser = false;

        SoltarJugador();

        if (rb == null)
            return;

        rb.isKinematic = kinematicOriginal;
        rb.useGravity = gravedadOriginal;
        rb.constraints = restriccionesOriginales;

        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void MoverPlataforma(
        Vector3 nuevaPosicion,
        Vector3 desplazamiento)
    {
        if (!dentroDelLaser)
            return;

        if (rb != null)
        {
            rb.MovePosition(nuevaPosicion);

            if (mantenerRotacion)
            {
                rb.MoveRotation(rotacionBloqueada);
            }
        }
        else
        {
            transform.position = nuevaPosicion;

            if (mantenerRotacion)
            {
                transform.rotation = rotacionBloqueada;
            }
        }

        /*
         * Ya no movemos manualmente al jugador.
         * Al ser hijo temporal de la plataforma,
         * acompaña exactamente su movimiento.
         */
    }

    private void MantenerCadaverEstable()
    {
        if (!mantenerRotacion)
            return;

        if (rb != null)
        {
            rb.MoveRotation(rotacionBloqueada);
        }
        else
        {
            transform.rotation = rotacionBloqueada;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DetectarJugadorEncima(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        DetectarJugadorEncima(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        Transform jugador = ObtenerJugador(
            collision.collider
        );

        if (jugador == null)
            return;

        collidersJugador.Remove(
            collision.collider
        );

        if (collidersJugador.Count == 0)
        {
            SoltarJugador();
        }
    }

    private void DetectarJugadorEncima(
        Collision collision)
    {
        if (!dentroDelLaser)
            return;

        Transform jugador = ObtenerJugador(
            collision.collider
        );

        if (jugador == null)
            return;

        bool estaEncima = false;

        foreach (ContactPoint contacto in collision.contacts)
        {
            
            if (contacto.normal.y <= -normalMinima)
            {
                estaEncima = true;
                break;
            }
        }

        if (!estaEncima)
            return;

        collidersJugador.Add(
            collision.collider
        );

        if (jugadorEncima == jugador)
            return;

        SubirJugadorAPlataforma(jugador);
    }

    private void SubirJugadorAPlataforma(
        Transform jugador)
    {
        SoltarJugador();

        jugadorEncima = jugador;

        rbJugador =
            jugadorEncima.GetComponent<Rigidbody>();

        if (rbJugador == null)
        {
            rbJugador =
                jugadorEncima.GetComponentInParent<Rigidbody>();
        }

       
        if (rbJugador != null)
        {
            jugadorEncima = rbJugador.transform;

           
            rbJugador.interpolation =
                RigidbodyInterpolation.Interpolate;
        }

        padreOriginalJugador =
            jugadorEncima.parent;

        
        jugadorEncima.SetParent(
            transform,
            true
        );
    }

    private void ComprobarJugadorEncima()
    {
        if (jugadorEncima == null)
            return;

        if (collidersJugador.Count == 0)
        {
            SoltarJugador();
            return;
        }

        Vector3 diferencia =
            jugadorEncima.position -
            transform.position;

       
        if (diferencia.magnitude >
            distanciaMaximaEncima)
        {
            SoltarJugador();
        }
    }

    private Transform ObtenerJugador(Collider col)
    {
        if (col == null)
            return null;

        if (col.CompareTag(tagJugador))
        {
            Rigidbody rigidbodyJugador =
                col.GetComponentInParent<Rigidbody>();

            if (rigidbodyJugador != null)
            {
                return rigidbodyJugador.transform;
            }

            return col.transform;
        }

        Transform actual = col.transform;

        while (actual != null)
        {
            if (actual.CompareTag(tagJugador))
            {
                Rigidbody rigidbodyJugador =
                    actual.GetComponent<Rigidbody>();

                if (rigidbodyJugador != null)
                {
                    return rigidbodyJugador.transform;
                }

                return actual;
            }

            actual = actual.parent;
        }

        return null;
    }

    private void SoltarJugador()
    {
        if (jugadorEncima != null)
        {
            
            jugadorEncima.SetParent(
                padreOriginalJugador,
                true
            );
        }

        jugadorEncima = null;
        rbJugador = null;
        padreOriginalJugador = null;

        collidersJugador.Clear();
    }

    public bool EstaDentroDelLaser()
    {
        return dentroDelLaser;
    }

    private void OnDisable()
    {
        SoltarJugador();
    }

    private void OnDestroy()
    {
        SoltarJugador();
    }
}
