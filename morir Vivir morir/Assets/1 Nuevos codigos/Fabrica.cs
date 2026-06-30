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

    [Header("Daño")]
    public string tagDaño = "Daño";

    private Vector3 ultimaPosicionSegura;
    private Quaternion ultimaRotacionSegura;

    private bool siendoTransportada = false;

    private Rigidbody rb;

    [Header("Energía")]
    public float energiaMaxima = 100f;
    public float energiaActual = 100f;
    public float energiaPerdidaPorRespawn = 25f;
    public CambioForma cambioForma;
    public Image barraEnergia;
    [Header("Clones")]
    public int maximoClones = 5;

    private Queue<GameObject> clones = new Queue<GameObject>();
    [Header("Derrota")]
    public GameObject canvasDerrota;

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
            Transform puntoFinal =
                puntoControlActual != null
                ? puntoControlActual
                : null;

            if (puntoFinal != null)
            {
                MoverFabrica(
                    puntoFinal.position,
                    puntoFinal.rotation
                );

                jugador.transform.position =
                    puntoFinal.position;

                jugador.transform.rotation =
                    puntoFinal.rotation;
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

        movJugador mov =
            jugador.GetComponent<movJugador>();

        if (mov != null)
        {
            mov.enabled = true;
        }

        AplicarCambioForma(jugador);
    }

    private void CrearCuerpo(GameObject jugador)
    {
        GameObject prefabElegido = ObtenerPrefabCuerpo(jugador);

        if (prefabElegido == null)
            return;

        GameObject cuerpo = Instantiate(
            prefabElegido,
            jugador.transform.position,
            jugador.transform.rotation
        );

        // Guarda el nuevo clon
        clones.Enqueue(cuerpo);

        // Si supera el límite, destruye el más viejo
        if (clones.Count > maximoClones)
        {
            GameObject clonViejo = clones.Dequeue();

            if (clonViejo != null)
            {
                Destroy(clonViejo);
            }
        }

        Animator anim = cuerpo.GetComponent<Animator>();

        if (anim != null)
        {
            anim.enabled = false;
        }
    }

    private GameObject ObtenerPrefabCuerpo(GameObject jugador)
    {
        CambioForma cambioForma =
            jugador.GetComponent<CambioForma>();

        if (cambioForma == null)
            return cuerpoBasePrefab;

        int formaActual =
            cambioForma.ObtenerFormaActual();

        if (formaActual == 0)
            return cuerpoBasePrefab;

        if (formaActual == 1)
            return cuerpoSaltoPrefab;

        if (formaActual == 2)
            return cuerpoMetalPrefab;

        return cuerpoBasePrefab;
    }

    private void AplicarCambioForma(GameObject jugador)
    {
        CambioForma cambioForma =
            jugador.GetComponent<CambioForma>();

        if (cambioForma != null)
        {
            cambioForma.AplicarFormaPendiente();
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
            return 0f;

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
