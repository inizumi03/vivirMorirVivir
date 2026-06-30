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

    [Header("Visual")]
    public Renderer rendererBoton;

    public Material materialNormal;
    public Material materialActivado;

    private bool activado = false;

    private void Start()
    {
        // MATERIAL INICIAL
        if (rendererBoton != null && materialNormal != null)
        {
            rendererBoton.material = materialNormal;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PuedeActivar(other.gameObject)) return;

        // SI YA SE ACTIVÓ Y ES DE UN SOLO USO
        if (activado && usarSoloUnaVez) return;

        activado = true;

        // CAMBIAR MATERIAL
        if (rendererBoton != null && materialActivado != null)
        {
            rendererBoton.material = materialActivado;
        }

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
