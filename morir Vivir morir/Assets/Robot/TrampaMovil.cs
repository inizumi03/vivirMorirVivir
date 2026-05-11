using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampaMovil : MonoBehaviour
{
    [Header("Puntos")]
    public Transform puntoA;
    public Transform puntoB;

    [Header("Movimiento")]
    public float velocidad = 3f;
    public bool esperarEnPuntos = true;
    public float tiempoEspera = 1f;

    private Transform objetivoActual;
    private bool esperando = false;

    private void Start()
    {
        if (puntoA != null)
        {
            transform.position = puntoA.position;
        }

        objetivoActual = puntoB;
    }

    private void Update()
    {
        if (puntoA == null || puntoB == null) return;
        if (esperando) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivoActual.position,
            velocidad * Time.deltaTime
        );

        float distancia = Vector3.Distance(
            transform.position,
            objetivoActual.position
        );

        if (distancia <= 0.05f)
        {
            CambiarObjetivo();
        }
    }

    private void CambiarObjetivo()
    {
        if (objetivoActual == puntoA)
        {
            objetivoActual = puntoB;
        }
        else
        {
            objetivoActual = puntoA;
        }

        if (esperarEnPuntos)
        {
            StartCoroutine(Esperar());
        }
    }

    private System.Collections.IEnumerator Esperar()
    {
        esperando = true;

        yield return new WaitForSeconds(tiempoEspera);

        esperando = false;
    }

    private void OnDrawGizmos()
    {
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                puntoA.position,
                puntoB.position
            );

            Gizmos.DrawSphere(puntoA.position, 0.2f);
            Gizmos.DrawSphere(puntoB.position, 0.2f);
        }
    }
}
