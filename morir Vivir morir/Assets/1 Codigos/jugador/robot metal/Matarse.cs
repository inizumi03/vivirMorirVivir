using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matarse : MonoBehaviour
{
    [Header("Referencias")]
    public CambioForma cambioForma;
    public Vida vida;

    [Header("Forma permitida")]
    public int formaPermitida = 1;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (cambioForma == null || vida == null)
            return;

        if (cambioForma.ObtenerFormaActual() != formaPermitida)
            return;

        vida.Morir();
    }
}
