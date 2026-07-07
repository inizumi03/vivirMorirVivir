using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptorLuz : MonoBehaviour
{
    [Header("Controlador")]
    public ControlReceptoresLuz controlador;

    private bool yaRecibioLuz = false;

    public void RecibirLuz()
    {
        if (yaRecibioLuz) return;

        yaRecibioLuz = true;

        if (controlador != null)
        {
            controlador.SumarReceptor();
        }

        Debug.Log("RECEPTOR ACTIVADO");
    }
}
