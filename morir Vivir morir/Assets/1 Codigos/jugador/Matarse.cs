using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matarse : MonoBehaviour
{
    [Header("Referencias")]
    public CambioForma cambioForma;
    public Fabrica fabrica;
    public GameObject jugador;

    [Header("Forma que puede usar esta habilidad")]
    public int formaPermitida = 1; // 0 Base - 1 Salto - 2 Metal

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (cambioForma == null || fabrica == null || jugador == null)
                return;

            // Solo funciona con la forma indicada
            if (cambioForma.ObtenerFormaActual() != formaPermitida)
                return;

            fabrica.RespawnearJugador(jugador);
        }
    }
}
