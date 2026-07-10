using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptorLuz : MonoBehaviour
{
    [Header("Controlador")]
    public ControlReceptoresLuz controlador;

    [Header("Visual")]
    public Renderer objetoVisual;
    public Material materialActivado;

    private Material materialOriginal;

    private bool yaRecibioLuz = false;

    private void Start()
    {
        if (objetoVisual != null)
        {
            materialOriginal = objetoVisual.material;
        }
    }

    public void RecibirLuz()
    {
        if (yaRecibioLuz)
            return;

        yaRecibioLuz = true;

        if (objetoVisual != null && materialActivado != null)
        {
            objetoVisual.material = materialActivado;
        }

        if (controlador != null)
        {
            controlador.SumarReceptor();
        }

        Debug.Log("RECEPTOR ACTIVADO");
    }
}
