using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Daño")]
    public string tagDaño = "Daño";

    [Header("Energía")]
    public float energiaMaxima = 100f;
    public float energiaActual = 100f;
    public float energiaPerdidaPorRespawn = 25f;
    public Image barraEnergia;

    [Header("Cambio de forma")]
    public CambioForma cambioForma;

    [Header("Derrota")]
    public GameObject canvasDerrota;

    private Vector3 ultimaPosicionSegura;
    private Quaternion ultimaRotacionSegura;

    private bool siendoTransportada = false;
    private Rigidbody rb;

    // Cada forma guarda sus propios clones.
    private Queue<GameObject> clonesBase =
        new Queue<GameObject>();

    private Queue<GameObject> clonesSalto =
        new Queue<GameObject>();

    private Queue<GameObject> clonesMetal =
        new Queue<GameObject>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GuardarPosicionSegura();

        energiaActual = energiaMaxima;
        ActualizarBarraEnergia();

        if (canvasDerrota != null)
        {
            canvasDerrota.SetActive(false);
        }
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
        // Crea el cuerpo usando la forma con la que murió.
        CrearCuerpo(jugador);

        PerderEnergia();

        Rigidbody rbJugador = jugador.GetComponent<Rigidbody>();

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
    }

    private void CrearCuerpo(GameObject jugador)
    {
        CambioForma cambio =
            jugador.GetComponent<CambioForma>();

        int formaAlMorir = 0;

        if (cambio != null)
        {
            formaAlMorir = cambio.ObtenerFormaActual();
        }

        GameObject prefabElegido =
            ObtenerPrefabSegunForma(formaAlMorir);

        int limite =
            ObtenerLimiteSegunForma(formaAlMorir);

        // Por ejemplo, la forma base tiene límite 0,
        // así que no genera ningún clon.
        if (prefabElegido == null || limite <= 0)
        {
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

    private void PerderEnergia()
    {
        energiaActual -= energiaPerdidaPorRespawn;

        energiaActual = Mathf.Clamp(
            energiaActual,
            0f,
            energiaMaxima
        );

        ActualizarBarraEnergia();

        if (energiaActual <= 0f)
        {
            ActivarDerrota();
        }
    }

    private void ActualizarBarraEnergia()
    {
        if (barraEnergia != null)
        {
            barraEnergia.fillAmount =
                energiaActual / energiaMaxima;
        }
    }

    private void ActivarDerrota()
    {
        if (canvasDerrota != null)
        {
            canvasDerrota.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public float CargarEnergia(float cantidad)
    {
        if (energiaActual >= energiaMaxima)
        {
            return 0f;
        }

        float energiaAntes = energiaActual;

        energiaActual += cantidad;

        energiaActual = Mathf.Clamp(
            energiaActual,
            0f,
            energiaMaxima
        );

        ActualizarBarraEnergia();

        return energiaActual - energiaAntes;
    }
}
