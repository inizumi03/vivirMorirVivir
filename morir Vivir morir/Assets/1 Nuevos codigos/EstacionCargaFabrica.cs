using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EstacionCargaFabrica : MonoBehaviour
{
    [Header("Objeto que se carga")]
    public Fabrica fabrica;

    [Header("Carga")]
    public float energiaMaxima = 100f;
    public float energiaActual = 100f;
    public float energiaPorSegundo = 10f;

    [Header("UI")]
    public Image barraEnergiaEstacion;

    [Header("Destruir al vaciarse")]
    public bool destruirAlVaciarse = true;

    private void Start()
    {
        energiaActual = energiaMaxima;

        ActualizarBarra();
    }

    private void OnTriggerStay(Collider other)
    {
        if (energiaActual <= 0f) return;
        if (fabrica == null) return;

        // SOLO FUNCIONA CON LA FABRICA DEFINIDA
        if (other.gameObject != fabrica.gameObject)
            return;

        // SI EL JUGADOR LA ESTA TRANSPORTANDO
        // NO CARGA
        if (fabrica.EstaSiendoTransportada())
            return;

        float energiaACargar =
            energiaPorSegundo * Time.deltaTime;

        energiaACargar = Mathf.Min(
            energiaACargar,
            energiaActual
        );

        float energiaReal =
            fabrica.CargarEnergia(energiaACargar);

        // SOLO GASTA SI REALMENTE CARGO
        if (energiaReal > 0f)
        {
            energiaActual -= energiaReal;

            energiaActual = Mathf.Clamp(
                energiaActual,
                0f,
                energiaMaxima
            );

            ActualizarBarra();

            if (energiaActual <= 0f &&
                destruirAlVaciarse)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ActualizarBarra()
    {
        if (barraEnergiaEstacion != null)
        {
            barraEnergiaEstacion.fillAmount =
                energiaActual / energiaMaxima;
        }
    }
}
