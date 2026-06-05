using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioForma : MonoBehaviour
{
    [Header("Modelos")]
    public GameObject robotBase;
    public GameObject robotSalto;
    public GameObject robotMetal;

    private int formaActual = 0;
    private int formaPendiente = 0;

    private void Start()
    {
        ActivarForma(formaActual);
    }

    public void ElegirBase()
    {
        formaPendiente = 0;
        Debug.Log("Forma pendiente: BASE");
    }

    public void ElegirSalto()
    {
        formaPendiente = 1;
        Debug.Log("Forma pendiente: SALTO");
    }

    public void ElegirMetal()
    {
        formaPendiente = 2;
        Debug.Log("Forma pendiente: METAL");
    }

    public void AplicarFormaPendiente()
    {
        formaActual = formaPendiente;
        ActivarForma(formaActual);
    }

    private void ActivarForma(int forma)
    {
        if (robotBase != null)
            robotBase.SetActive(false);

        if (robotSalto != null)
            robotSalto.SetActive(false);

        if (robotMetal != null)
            robotMetal.SetActive(false);

        if (forma == 0 && robotBase != null)
            robotBase.SetActive(true);

        if (forma == 1 && robotSalto != null)
            robotSalto.SetActive(true);

        if (forma == 2 && robotMetal != null)
            robotMetal.SetActive(true);
    }
    public int ObtenerFormaActual()
    {
        return formaActual;
    }
    public bool EstaEnFormaSalto()
    {
        return formaActual == 1;
    }
}
