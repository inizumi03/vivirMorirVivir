using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boton : MonoBehaviour
{
    [Header("Detección")]
    public string tagJugador = "Player";
    public string tagObjeto = "Aparecer";

    [Header("Objeto que baja")]
    public Transform objetoABajar;
    public float distanciaBajada = 5f;
    public float velocidad = 2f;

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;

    private bool activado = false;

    private void Start()
    {
        if (objetoABajar != null)
        {
            posicionInicial = objetoABajar.position;
            posicionFinal = posicionInicial + Vector3.down * distanciaBajada;
        }
    }

    private void Update()
    {
        if (objetoABajar == null) return;

        Vector3 destino = activado ? posicionFinal : posicionInicial;

        objetoABajar.position = Vector3.MoveTowards(
            objetoABajar.position,
            destino,
            velocidad * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PuedeActivar(other))
        {
            activado = !activado;
        }
    }

    private bool PuedeActivar(Collider other)
    {
        return other.CompareTag(tagJugador) || other.CompareTag(tagObjeto);
    }
}
