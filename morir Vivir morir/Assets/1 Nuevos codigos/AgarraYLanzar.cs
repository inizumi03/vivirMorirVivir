using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgarraYLanzar : MonoBehaviour
{
    [Header("Referencias")]
    public AnimJugador animJugador;
    public Transform puntoCarga;
    public MonoBehaviour scriptMovimiento;

    [Header("Lanzamiento")]
    public float fuerzaMinima = 6f;
    public float fuerzaMaxima = 18f;
    public float velocidadCarga = 12f;
    public float fuerzaArriba = 2f;

    [Header("Trayectoria")]
    public LineRenderer lineaTrayectoria;
    public int puntosLinea = 25;
    public float tiempoEntrePuntos = 0.08f;

    private GameObject objetoEnRango;
    private GameObject objetoAgarrado;
    private Rigidbody rbObjeto;
    private Rigidbody rbJugador;
    private Collider[] collidersJugador;

    private bool agarrandoAnimacion;
    private bool cargandoLanzamiento;
    private bool cargaSubiendo = true;
    private float fuerzaActual;

    private void Awake()
    {
        rbJugador = GetComponent<Rigidbody>();
        collidersJugador = GetComponentsInChildren<Collider>();
        fuerzaActual = fuerzaMinima;
    }

    private void Start()
    {
        if (lineaTrayectoria != null)
            lineaTrayectoria.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objetoAgarrado == null)
                IntentarAgarrar();
            else
                Soltar();
        }

        if (objetoAgarrado != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                cargandoLanzamiento = true;
                cargaSubiendo = true;
                fuerzaActual = fuerzaMinima;
            }

            if (Input.GetMouseButton(0) && cargandoLanzamiento)
            {
                CargarFuerzaOscilante();
            }

            if (Input.GetMouseButtonUp(0) && cargandoLanzamiento)
            {
                Lanzar();
            }
        }

        ActualizarTrayectoria();
    }

    private void FixedUpdate()
    {
        if (objetoAgarrado != null && rbObjeto != null && puntoCarga != null)
        {
            ObjetoEstable datos = objetoAgarrado.GetComponent<ObjetoEstable>();

            if (datos != null)
            {
                rbObjeto.MovePosition(datos.ObtenerPosicionCarga(puntoCarga));
                rbObjeto.MoveRotation(datos.ObtenerRotacionCarga(puntoCarga));
            }
            else
            {
                rbObjeto.MovePosition(puntoCarga.position);
                rbObjeto.MoveRotation(puntoCarga.rotation);
            }
        }
    }

    private void CargarFuerzaOscilante()
    {
        float cambio = velocidadCarga * Time.deltaTime;

        if (cargaSubiendo)
        {
            fuerzaActual += cambio;

            if (fuerzaActual >= fuerzaMaxima)
            {
                fuerzaActual = fuerzaMaxima;
                cargaSubiendo = false;
            }
        }
        else
        {
            fuerzaActual -= cambio;

            if (fuerzaActual <= fuerzaMinima)
            {
                fuerzaActual = fuerzaMinima;
                cargaSubiendo = true;
            }
        }
    }

    public void SetObjetoEnRango(GameObject obj)
    {
        objetoEnRango = obj;
    }

    public void QuitarObjetoEnRango(GameObject obj)
    {
        if (objetoEnRango == obj)
            objetoEnRango = null;
    }

    private void IntentarAgarrar()
    {
        if (objetoEnRango == null) return;
        if (agarrandoAnimacion) return;

        agarrandoAnimacion = true;

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = false;

        if (rbJugador != null)
        {
            rbJugador.velocity = Vector3.zero;
            rbJugador.angularVelocity = Vector3.zero;
        }

        if (animJugador != null)
            animJugador.ActivarAgarre();
    }

    public void EventoFinAgarre()
    {
        if (objetoEnRango == null)
        {
            TerminarAgarre();
            return;
        }

        objetoAgarrado = objetoEnRango;
        rbObjeto = objetoAgarrado.GetComponent<Rigidbody>();

        if (rbObjeto == null)
            rbObjeto = objetoAgarrado.GetComponentInParent<Rigidbody>();

        if (rbObjeto == null)
        {
            objetoAgarrado = null;
            TerminarAgarre();
            return;
        }

        objetoAgarrado = rbObjeto.gameObject;

        Fabrica fabrica = objetoAgarrado.GetComponent<Fabrica>();

        if (fabrica == null)
            fabrica = objetoAgarrado.GetComponentInParent<Fabrica>();

        if (fabrica == null)
            fabrica = objetoAgarrado.GetComponentInChildren<Fabrica>();

        if (fabrica != null)
        {
            fabrica.GuardarPosicionSegura();
            fabrica.MarcarCargada(true);

            Debug.Log("FABRICA AGARRADA");
        }
        ObjetoEstable datos = objetoAgarrado.GetComponent<ObjetoEstable>();

        if (datos != null)
            datos.IgnorarColisionConJugador(collidersJugador, true);

        rbObjeto.velocity = Vector3.zero;
        rbObjeto.angularVelocity = Vector3.zero;
        rbObjeto.useGravity = false;
        rbObjeto.freezeRotation = true;

        if (datos != null)
        {
            objetoAgarrado.transform.position = datos.ObtenerPosicionCarga(puntoCarga);
            objetoAgarrado.transform.rotation = datos.ObtenerRotacionCarga(puntoCarga);
        }
        else
        {
            objetoAgarrado.transform.position = puntoCarga.position;
            objetoAgarrado.transform.rotation = puntoCarga.rotation;
        }

        fuerzaActual = fuerzaMinima;
        cargandoLanzamiento = false;

        TerminarAgarre();
    }

    private void TerminarAgarre()
    {
        agarrandoAnimacion = false;

        if (scriptMovimiento != null)
            scriptMovimiento.enabled = true;
    }

    private void Soltar()
    {
        if (objetoAgarrado == null || rbObjeto == null) return;

        ObjetoEstable datos = objetoAgarrado.GetComponent<ObjetoEstable>();

        if (datos != null)
            datos.IgnorarColisionConJugador(collidersJugador, false);

        rbObjeto.useGravity = true;
        rbObjeto.freezeRotation = false;
        rbObjeto.velocity = Vector3.zero;
        rbObjeto.angularVelocity = Vector3.zero;
        Fabrica fabrica = objetoAgarrado.GetComponent<Fabrica>();

        if (fabrica == null)
            fabrica = objetoAgarrado.GetComponentInParent<Fabrica>();

        if (fabrica == null)
            fabrica = objetoAgarrado.GetComponentInChildren<Fabrica>();

        if (fabrica != null)
        {
            fabrica.MarcarCargada(false);

            Debug.Log("FABRICA SOLTADA");
        }
        objetoAgarrado = null;
        rbObjeto = null;

        cargandoLanzamiento = false;
        fuerzaActual = fuerzaMinima;

        if (lineaTrayectoria != null)
            lineaTrayectoria.enabled = false;
    }

    private void Lanzar()
    {
        if (objetoAgarrado == null || rbObjeto == null) return;

        GameObject objeto = objetoAgarrado;
        Rigidbody rb = rbObjeto;

        ObjetoEstable datos = objeto.GetComponent<ObjetoEstable>();

        if (datos != null)
            datos.IgnorarColisionConJugador(collidersJugador, false);
        Fabrica fabrica = objeto.GetComponent<Fabrica>();

        if (fabrica == null)
            fabrica = objeto.GetComponentInParent<Fabrica>();

        if (fabrica == null)
            fabrica = objeto.GetComponentInChildren<Fabrica>();

        if (fabrica != null)
        {
            fabrica.MarcarCargada(false);

            Debug.Log("FABRICA LANZADA");
        }
        objetoAgarrado = null;
        rbObjeto = null;

        rb.useGravity = true;
        rb.freezeRotation = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(ObtenerDireccionLanzamiento() * fuerzaActual, ForceMode.Impulse);

        cargandoLanzamiento = false;
        fuerzaActual = fuerzaMinima;

        if (lineaTrayectoria != null)
            lineaTrayectoria.enabled = false;
    }

    private Vector3 ObtenerDireccionLanzamiento()
    {
        return (puntoCarga.forward + Vector3.up * fuerzaArriba).normalized;
    }

    private void ActualizarTrayectoria()
    {
        if (lineaTrayectoria == null) return;

        if (objetoAgarrado == null)
        {
            lineaTrayectoria.enabled = false;
            return;
        }

        lineaTrayectoria.enabled = true;
        lineaTrayectoria.positionCount = puntosLinea;

        Vector3 origen = puntoCarga.position;
        Vector3 velocidadInicial = ObtenerDireccionLanzamiento() * fuerzaActual;

        for (int i = 0; i < puntosLinea; i++)
        {
            float tiempo = i * tiempoEntrePuntos;

            Vector3 posicion = origen
                + velocidadInicial * tiempo
                + 0.5f * Physics.gravity * tiempo * tiempo;

            lineaTrayectoria.SetPosition(i, posicion);
        }
    }
}
