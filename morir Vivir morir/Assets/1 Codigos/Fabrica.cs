using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fabrica : MonoBehaviour
{
    [Header("Respawn normal")]
    public Transform puntoRespawnJugador;

    [Header("Respawn si muero transportando fábrica")]
    public Transform puntoControlActual;

    [Header("Prefabs cuerpos muertos")]
    public GameObject cuerpoBasePrefab;
    public GameObject cuerpoSaltoPrefab;
    public GameObject cuerpoMetalPrefab;

    [Header("Límite de clones por forma")]
    public int maxClonesBase = 0;
    public int maxClonesSalto = 5;
    public int maxClonesMetal = 2;

    [Header("Interfaz de clones")]
    public TextMeshProUGUI textoClones;

    [Header("Daño")]
    public string tagDaño = "Daño";

    [Header("Dinero")]
    public int dineroInicial = 100;
    public int dineroActual = 100;
    public int dineroPerdidoPorMuerte = 10;
    public TextMeshProUGUI textoDinero;

    [Header("Cambio de forma")]
    public CambioForma cambioForma;

    private Vector3 ultimaPosicionSegura;
    private Quaternion ultimaRotacionSegura;

    private bool siendoTransportada = false;
    private Rigidbody rb;

    private Queue<GameObject> clonesBase =
        new Queue<GameObject>();

    private Queue<GameObject> clonesSalto =
        new Queue<GameObject>();

    private Queue<GameObject> clonesMetal =
        new Queue<GameObject>();

    private int ultimaFormaMostrada = -1;
    private int ultimaCantidadMostrada = -1;
    private int ultimoLimiteMostrado = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GuardarPosicionSegura();

        dineroActual = dineroInicial;

        ActualizarTextoDinero();
        ActualizarTextoClones(true);
    }

    private void Update()
    {
        ActualizarTextoClones(false);
    }

    public void GuardarPosicionSegura()
    {
        ultimaPosicionSegura = transform.position;
        ultimaRotacionSegura = transform.rotation;
    }

    public void MarcarTransportada(bool estado)
    {
        siendoTransportada = estado;
    }

    public bool EstaSiendoTransportada()
    {
        return siendoTransportada;
    }

    public void ActualizarPuntoControl(Transform nuevoPunto)
    {
        puntoControlActual = nuevoPunto;
    }

    public void RespawnearJugador(GameObject jugador)
    {
        CrearCuerpo(jugador);

        PerderDinero();

        Rigidbody rbJugador =
            jugador.GetComponent<Rigidbody>();

        if (rbJugador != null)
        {
            rbJugador.velocity = Vector3.zero;
            rbJugador.angularVelocity = Vector3.zero;
        }

        if (siendoTransportada)
        {
            if (puntoControlActual != null)
            {
                MoverFabrica(
                    puntoControlActual.position,
                    puntoControlActual.rotation
                );

                jugador.transform.position =
                    puntoControlActual.position;

                jugador.transform.rotation =
                    puntoControlActual.rotation;
            }
            else
            {
                MoverFabrica(
                    ultimaPosicionSegura,
                    ultimaRotacionSegura
                );

                jugador.transform.position =
                    ultimaPosicionSegura;

                jugador.transform.rotation =
                    ultimaRotacionSegura;
            }

            siendoTransportada = false;

            AplicarCambioForma(jugador);
            ActualizarTextoClones(true);

            return;
        }

        if (puntoRespawnJugador != null)
        {
            jugador.transform.position =
                puntoRespawnJugador.position;

            jugador.transform.rotation =
                puntoRespawnJugador.rotation;
        }
        else
        {
            jugador.transform.position =
                transform.position;

            jugador.transform.rotation =
                transform.rotation;
        }

        movJugador movimiento =
            jugador.GetComponent<movJugador>();

        if (movimiento != null)
        {
            movimiento.enabled = true;
        }

        AplicarCambioForma(jugador);
        ActualizarTextoClones(true);
    }

    private void CrearCuerpo(GameObject jugador)
    {
        CambioForma cambio =
            jugador.GetComponent<CambioForma>();

        int formaAlMorir = 0;

        if (cambio != null)
        {
            formaAlMorir =
                cambio.ObtenerFormaActual();
        }

        GameObject prefabElegido =
            ObtenerPrefabSegunForma(formaAlMorir);

        int limite =
            ObtenerLimiteSegunForma(formaAlMorir);

        if (prefabElegido == null || limite <= 0)
        {
            ActualizarTextoClones(true);
            return;
        }

        GameObject cuerpo = Instantiate(
            prefabElegido,
            jugador.transform.position,
            jugador.transform.rotation
        );

        Animator animacion =
            cuerpo.GetComponentInChildren<Animator>();

        if (animacion != null)
        {
            animacion.enabled = false;
        }

        RegistrarClon(
            cuerpo,
            formaAlMorir,
            limite
        );

        ActualizarTextoClones(true);
    }

    private GameObject ObtenerPrefabSegunForma(int forma)
    {
        if (forma == 0)
        {
            return cuerpoBasePrefab;
        }

        if (forma == 1)
        {
            return cuerpoSaltoPrefab;
        }

        if (forma == 2)
        {
            return cuerpoMetalPrefab;
        }

        return cuerpoBasePrefab;
    }

    private int ObtenerLimiteSegunForma(int forma)
    {
        if (forma == 0)
        {
            return maxClonesBase;
        }

        if (forma == 1)
        {
            return maxClonesSalto;
        }

        if (forma == 2)
        {
            return maxClonesMetal;
        }

        return 0;
    }

    private void RegistrarClon(
        GameObject nuevoClon,
        int forma,
        int limite)
    {
        Queue<GameObject> colaElegida =
            ObtenerColaSegunForma(forma);

        if (colaElegida == null)
        {
            Destroy(nuevoClon);
            return;
        }

        LimpiarReferenciasDestruidas(colaElegida);

        colaElegida.Enqueue(nuevoClon);

        while (colaElegida.Count > limite)
        {
            GameObject clonMasViejo =
                colaElegida.Dequeue();

            if (clonMasViejo != null)
            {
                Destroy(clonMasViejo);
            }
        }

        ActualizarTextoClones(true);
    }

    private Queue<GameObject> ObtenerColaSegunForma(int forma)
    {
        if (forma == 0)
        {
            return clonesBase;
        }

        if (forma == 1)
        {
            return clonesSalto;
        }

        if (forma == 2)
        {
            return clonesMetal;
        }

        return null;
    }

    private void LimpiarReferenciasDestruidas(
        Queue<GameObject> cola)
    {
        if (cola == null)
            return;

        int cantidad = cola.Count;

        for (int i = 0; i < cantidad; i++)
        {
            GameObject clon = cola.Dequeue();

            if (clon != null)
            {
                cola.Enqueue(clon);
            }
        }
    }

    private void AplicarCambioForma(GameObject jugador)
    {
        CambioForma cambio =
            jugador.GetComponent<CambioForma>();

        if (cambio != null)
        {
            cambio.AplicarFormaPendiente();
        }
    }

    private void MoverFabrica(
        Vector3 posicion,
        Quaternion rotacion)
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = posicion;
        transform.rotation = rotacion;
    }

    public void VolverAPosicionSegura()
    {
        MoverFabrica(
            ultimaPosicionSegura,
            ultimaRotacionSegura
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(tagDaño))
        {
            VolverAPosicionSegura();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDaño))
        {
            VolverAPosicionSegura();
        }
    }

    private void PerderDinero()
    {
        dineroActual -= dineroPerdidoPorMuerte;

        ActualizarTextoDinero();
    }

    private void ActualizarTextoDinero()
    {
        if (textoDinero != null)
        {
            textoDinero.text =
                "$" + dineroActual;
        }
    }

    private void ActualizarTextoClones(
        bool forzarActualizacion)
    {
        if (textoClones == null)
            return;

        int formaActual = 0;

        if (cambioForma != null)
        {
            formaActual =
                cambioForma.ObtenerFormaActual();
        }

        Queue<GameObject> colaActual =
            ObtenerColaSegunForma(formaActual);

        if (colaActual != null)
        {
            LimpiarReferenciasDestruidas(
                colaActual
            );
        }

        int cantidadActual =
            colaActual != null
                ? colaActual.Count
                : 0;

        int limiteActual =
            ObtenerLimiteSegunForma(
                formaActual
            );

        if (!forzarActualizacion &&
            formaActual == ultimaFormaMostrada &&
            cantidadActual == ultimaCantidadMostrada &&
            limiteActual == ultimoLimiteMostrado)
        {
            return;
        }

        textoClones.text =
            "OBVIS DISPONIBLES:\n" +
            cantidadActual +
            " / " +
            limiteActual;

        ultimaFormaMostrada =
            formaActual;

        ultimaCantidadMostrada =
            cantidadActual;

        ultimoLimiteMostrado =
            limiteActual;
    }

    public void AgregarDinero(int cantidad)
    {
        dineroActual += cantidad;

        ActualizarTextoDinero();
    }

    public int ObtenerDineroActual()
    {
        return dineroActual;
    }

    public int ObtenerCantidadClonesFormaActual()
    {
        int formaActual = 0;

        if (cambioForma != null)
        {
            formaActual =
                cambioForma.ObtenerFormaActual();
        }

        Queue<GameObject> colaActual =
            ObtenerColaSegunForma(formaActual);

        if (colaActual == null)
            return 0;

        LimpiarReferenciasDestruidas(
            colaActual
        );

        return colaActual.Count;
    }
}