using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaPorBotones : MonoBehaviour
{
    [Header("Botones necesarios")]
    public int botonesNecesarios = 2;

    [Header("Objeto que baja")]
    public Transform objetoABajar;
    public float distanciaBajada = 5f;
    public float velocidad = 2f;

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;

    private int botonesActivados = 0;

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

        bool abrir = botonesActivados >= botonesNecesarios;

        Vector3 destino = abrir ? posicionFinal : posicionInicial;

        objetoABajar.position = Vector3.MoveTowards(
            objetoABajar.position,
            destino,
            velocidad * Time.deltaTime
        );
    }

    public void SumarBoton()
    {
        botonesActivados++;
    }

    public void RestarBoton()
    {
        botonesActivados--;

        if (botonesActivados < 0)
        {
            botonesActivados = 0;
        }
    }
}
