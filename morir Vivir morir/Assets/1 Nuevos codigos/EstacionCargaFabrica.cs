using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EstacionCargaFabrica : MonoBehaviour
{
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

    private void OnCollisionStay(Collision collision)
    {
        if (energiaActual <= 0f) return;

        Fabrica fabrica =
         collision.gameObject.GetComponent<Fabrica>();

        if (fabrica != null)
        {
            float energiaACargar = energiaPorSegundo * Time.deltaTime;

            energiaACargar = Mathf.Min(energiaACargar, energiaActual);

            fabrica.CargarEnergia(energiaACargar);

            energiaActual -= energiaACargar;
            energiaActual = Mathf.Clamp(energiaActual, 0f, energiaMaxima);

            ActualizarBarra();

            if (energiaActual <= 0f && destruirAlVaciarse)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ActualizarBarra()
    {
        if (barraEnergiaEstacion != null)
        {
            barraEnergiaEstacion.fillAmount = energiaActual / energiaMaxima;
        }
    }
}
