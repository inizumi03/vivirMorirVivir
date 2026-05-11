using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boton : MonoBehaviour
{
    [Header("Detección")]
    public string tagJugador = "Player";
    public string tagObjeto = "Agarrable";

    [Header("Controlador")]
    public PuertaPorBotones puerta;

    [Header("Estado")]
    public bool usarSoloUnaVez = true;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!PuedeActivar(other.gameObject)) return;

        // SI YA SE ACTIVÓ Y ES DE UN SOLO USO
        if (activado && usarSoloUnaVez) return;

        activado = true;

        if (puerta != null)
        {
            puerta.SumarBoton();
        }

        Debug.Log("BOTON ACTIVADO");
    }

    private bool PuedeActivar(GameObject obj)
    {
        return obj.CompareTag(tagJugador) || obj.CompareTag(tagObjeto);
    }
}
