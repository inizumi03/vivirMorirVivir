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

    [Header("Animación de la sierra")]
    public Animator animatorSierra;

    private Transform objetivoActual;
    private bool esperando = false;
    private bool bloqueadaPorCadaver = false;

    private int cantidadCadaveresTocando = 0;

    private void Start()
    {
        if (puntoA != null)
        {
            transform.position = puntoA.position;
        }

        objetivoActual = puntoB;

        if (animatorSierra != null)
        {
            animatorSierra.speed = 1f;
        }
    }

    private void Update()
    {
        if (puntoA == null || puntoB == null) return;
        if (esperando) return;
        if (bloqueadaPorCadaver) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivoActual.position,
            velocidad * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, objetivoActual.position) <= 0.05f)
        {
            CambiarObjetivo();
        }
    }

    private void CambiarObjetivo()
    {
        objetivoActual = objetivoActual == puntoA ? puntoB : puntoA;

        if (esperarEnPuntos)
        {
            StartCoroutine(Esperar());
        }
    }

    private IEnumerator Esperar()
    {
        esperando = true;

        yield return new WaitForSeconds(tiempoEspera);

        esperando = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (EsCadaver(collision.collider))
        {
            cantidadCadaveresTocando++;
            BloquearSierra();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (EsCadaver(collision.collider))
        {
            cantidadCadaveresTocando--;

            if (cantidadCadaveresTocando < 0)
                cantidadCadaveresTocando = 0;

            if (cantidadCadaveresTocando == 0)
            {
                DesbloquearSierra();
            }
        }
    }

    private bool EsCadaver(Collider col)
    {
        return col.GetComponentInParent<CadaverBloqueador>() != null;
    }

    private void BloquearSierra()
    {
        bloqueadaPorCadaver = true;

        if (animatorSierra != null)
        {
            animatorSierra.speed = 0f;
        }

        Debug.Log("SIERRA BLOQUEADA POR CADAVER");
    }

    private void DesbloquearSierra()
    {
        bloqueadaPorCadaver = false;

        if (animatorSierra != null)
        {
            animatorSierra.speed = 1f;
        }

        Debug.Log("SIERRA LIBERADA");
    }

    private void OnDrawGizmos()
    {
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(puntoA.position, puntoB.position);
            Gizmos.DrawSphere(puntoA.position, 0.2f);
            Gizmos.DrawSphere(puntoB.position, 0.2f);
        }
    }
}
