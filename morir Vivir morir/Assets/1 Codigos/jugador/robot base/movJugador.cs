using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movJugador : MonoBehaviour
{
    [Header("Estado")]
    public bool puedeMoverse = true;

    [Header("Movimiento")]
    public float velocidad = 7f;
    public float velocidadRotacion = 18f;
    public float aceleracion = 25f;
    public float desaceleracion = 30f;

    [Header("Precisión del movimiento")]

    [Tooltip("Frenado utilizado cuando el jugador cambia hacia la dirección contraria.")]
    public float frenadoCambioDireccion = 45f;

    [Tooltip("Rotación más rápida cuando el jugador cambia bruscamente de dirección.")]
    public float velocidadRotacionCambio = 24f;

    [Tooltip("Multiplicador de aceleración utilizado al comenzar a moverse.")]
    public float multiplicadorAceleracionInicial = 1.35f;

    [Tooltip("Porcentaje de velocidad hasta el cual se aplica la aceleración inicial.")]
    [Range(0f, 1f)]
    public float porcentajeAceleracionInicial = 0.4f;

    [Tooltip("Velocidad mínima antes de detener completamente al jugador.")]
    public float velocidadMinima = 0.05f;

    [Header("Referencias")]
    public Transform camara;

    [Header("Plataformas móviles")]
    [Tooltip("Normal mínima para considerar que el jugador está parado encima.")]
    [Range(0f, 1f)]
    public float normalMinimaPlataforma = 0.5f;

    private Rigidbody rb;

    private Vector3 direccionMovimiento;
    private Vector3 velocidadActual;

    private CadaverPlataforma plataformaActual;
    private Rigidbody rbPlataforma;

    private Vector3 posicionAnteriorPlataforma;
    private Vector3 velocidadPlataforma;

    private readonly HashSet<Collider> contactosPlataforma =
        new HashSet<Collider>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError(
                "movJugador necesita un Rigidbody en el mismo objeto.",
                gameObject
            );

            enabled = false;
            return;
        }

        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!puedeMoverse)
        {
            direccionMovimiento = Vector3.zero;
            return;
        }

        if (camara == null)
        {
            direccionMovimiento = Vector3.zero;
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 adelante = camara.forward;
        Vector3 derecha = camara.right;

        adelante.y = 0f;
        derecha.y = 0f;

        adelante.Normalize();
        derecha.Normalize();

        direccionMovimiento =
            (adelante * vertical + derecha * horizontal).normalized;
    }

    private void FixedUpdate()
    {
        CalcularVelocidadPlataforma();

        if (!puedeMoverse)
        {
            DetenerFisica();
            return;
        }

        MoverJugador();
        RotarJugador();
    }

    private void MoverJugador()
    {
        bool tieneEntrada =
            direccionMovimiento.sqrMagnitude > 0.01f;

        Vector3 velocidadObjetivo =
            direccionMovimiento * velocidad;

        bool cambiandoDireccion = EstaCambiandoDireccion();

        float suavizado;

        if (cambiandoDireccion)
        {
            /*
             * Cuando el jugador intenta moverse hacia la
             * dirección contraria, frenamos más rápido.
             */
            suavizado = frenadoCambioDireccion;
        }
        else if (tieneEntrada)
        {
            /*
             * Al comenzar a moverse se utiliza una aceleración
             * más fuerte para que el personaje responda rápido.
             */
            float porcentajeVelocidad = 0f;

            if (velocidad > 0f)
            {
                porcentajeVelocidad =
                    velocidadActual.magnitude / velocidad;
            }

            if (porcentajeVelocidad <
                porcentajeAceleracionInicial)
            {
                suavizado =
                    aceleracion *
                    multiplicadorAceleracionInicial;
            }
            else
            {
                suavizado = aceleracion;
            }
        }
        else
        {
            /*
             * Al soltar las teclas se utiliza la desaceleración.
             */
            suavizado = desaceleracion;
        }

        velocidadActual = Vector3.MoveTowards(
            velocidadActual,
            velocidadObjetivo,
            suavizado * Time.fixedDeltaTime
        );

        /*
         * Elimina velocidades demasiado pequeñas para evitar
         * que el jugador continúe deslizándose lentamente.
         */
        if (!tieneEntrada &&
            velocidadActual.magnitude <= velocidadMinima)
        {
            velocidadActual = Vector3.zero;
        }

        /*
         * La velocidad propia del jugador se combina con
         * la velocidad de la plataforma móvil.
         */
        Vector3 nuevaVelocidad = new Vector3(
            velocidadActual.x + velocidadPlataforma.x,
            rb.velocity.y,
            velocidadActual.z + velocidadPlataforma.z
        );

        rb.velocity = nuevaVelocidad;
    }

    private bool EstaCambiandoDireccion()
    {
        if (direccionMovimiento.sqrMagnitude <= 0.01f)
            return false;

        if (velocidadActual.sqrMagnitude <= 0.01f)
            return false;

        float direccionEntreVectores = Vector3.Dot(
            velocidadActual.normalized,
            direccionMovimiento.normalized
        );

        /*
         * Un resultado menor que cero significa que la nueva
         * dirección apunta en contra del movimiento actual.
         */
        return direccionEntreVectores < 0f;
    }

    private void RotarJugador()
    {
        if (direccionMovimiento.sqrMagnitude < 0.01f)
            return;

        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(
                direccionMovimiento,
                Vector3.up
            );

        float angulo = Vector3.Angle(
            transform.forward,
            direccionMovimiento
        );

        float rotacionUsada =
            angulo > 90f
            ? velocidadRotacionCambio
            : velocidadRotacion;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            rotacionUsada * Time.fixedDeltaTime
        );
    }

    private void CalcularVelocidadPlataforma()
    {
        if (plataformaActual == null)
        {
            velocidadPlataforma = Vector3.zero;
            rbPlataforma = null;
            return;
        }

        /*
         * Si la plataforma tiene Rigidbody y Unity registra
         * su velocidad, usamos directamente esa velocidad.
         */
        if (rbPlataforma != null &&
            rbPlataforma.velocity.sqrMagnitude > 0.000001f)
        {
            velocidadPlataforma =
                rbPlataforma.velocity;

            posicionAnteriorPlataforma =
                rbPlataforma.position;

            return;
        }

        /*
         * Si el Rigidbody es kinematic y no informa velocidad,
         * calculamos la velocidad mediante el desplazamiento.
         */
        Vector3 posicionActual =
            plataformaActual.transform.position;

        Vector3 desplazamiento =
            posicionActual -
            posicionAnteriorPlataforma;

        velocidadPlataforma =
            desplazamiento / Time.fixedDeltaTime;

        posicionAnteriorPlataforma =
            posicionActual;
    }

    private void OnCollisionEnter(Collision collision)
    {
        DetectarPlataforma(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        DetectarPlataforma(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        CadaverPlataforma plataforma =
            collision.collider
                .GetComponentInParent<CadaverPlataforma>();

        if (plataforma == null)
            return;

        contactosPlataforma.Remove(
            collision.collider
        );

        if (contactosPlataforma.Count == 0 &&
            plataforma == plataformaActual)
        {
            SalirDePlataforma();
        }
    }

    private void DetectarPlataforma(
        Collision collision)
    {
        CadaverPlataforma plataforma =
            collision.collider
                .GetComponentInParent<CadaverPlataforma>();

        if (plataforma == null)
            return;

        bool estaEncima = false;

        /*
         * Desde el jugador, una normal hacia arriba indica
         * que está apoyado sobre la plataforma.
         */
        foreach (ContactPoint contacto in collision.contacts)
        {
            if (contacto.normal.y >= normalMinimaPlataforma)
            {
                estaEncima = true;
                break;
            }
        }

        if (!estaEncima)
            return;

        contactosPlataforma.Add(
            collision.collider
        );

        if (plataformaActual == plataforma)
            return;

        plataformaActual = plataforma;

        rbPlataforma =
            plataformaActual.GetComponent<Rigidbody>();

        if (rbPlataforma == null)
        {
            rbPlataforma =
                plataformaActual
                    .GetComponentInParent<Rigidbody>();
        }

        if (rbPlataforma != null)
        {
            posicionAnteriorPlataforma =
                rbPlataforma.position;
        }
        else
        {
            posicionAnteriorPlataforma =
                plataformaActual.transform.position;
        }

        velocidadPlataforma = Vector3.zero;
    }

    private void SalirDePlataforma()
    {
        plataformaActual = null;
        rbPlataforma = null;

        velocidadPlataforma = Vector3.zero;
        posicionAnteriorPlataforma = Vector3.zero;

        contactosPlataforma.Clear();
    }

    public void BloquearMovimiento()
    {
        puedeMoverse = false;

        direccionMovimiento = Vector3.zero;
        velocidadActual = Vector3.zero;

        DetenerFisica();
    }

    public void DesbloquearMovimiento()
    {
        puedeMoverse = true;
    }

    private void DetenerFisica()
    {
        if (rb == null)
            return;

        
        rb.velocity = new Vector3(
            velocidadPlataforma.x,
            rb.velocity.y,
            velocidadPlataforma.z
        );

        rb.angularVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        SalirDePlataforma();
    }
}
