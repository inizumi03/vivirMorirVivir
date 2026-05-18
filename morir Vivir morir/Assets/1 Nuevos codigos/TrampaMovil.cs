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

    [Header("Animación")]
    public Animator animatorSierra;

    private Transform objetivoActual;
    private bool esperando = false;
    private int cadaveresTocando = 0;

    private void Start()
    {
        if (puntoA != null)
            transform.position = puntoA.position;

        objetivoActual = puntoB;

        ActualizarEstadoSierra();
    }

    private void Update()
    {
        if (cadaveresTocando < 0)
            cadaveresTocando = 0;

        ActualizarEstadoSierra();

        if (puntoA == null || puntoB == null) return;
        if (esperando) return;
        if (cadaveresTocando > 0) return;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (EsCadaver(collision.collider))
        {
            cadaveresTocando++;
            ActualizarEstadoSierra();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (EsCadaver(collision.collider))
        {
            cadaveresTocando--;
            if (cadaveresTocando < 0)
                cadaveresTocando = 0;

            ActualizarEstadoSierra();
        }
    }

    private bool EsCadaver(Collider col)
    {
        return col.GetComponentInParent<CadaverBloqueador>() != null;
    }

    private void ActualizarEstadoSierra()
    {
        bool bloqueada = cadaveresTocando > 0;

        if (animatorSierra != null)
            animatorSierra.speed = bloqueada ? 0f : 1f;
    }

    private void CambiarObjetivo()
    {
        objetivoActual = objetivoActual == puntoA ? puntoB : puntoA;

        if (esperarEnPuntos)
            StartCoroutine(Esperar());
    }

    private IEnumerator Esperar()
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
            Gizmos.DrawLine(puntoA.position, puntoB.position);
            Gizmos.DrawSphere(puntoA.position, 0.2f);
            Gizmos.DrawSphere(puntoB.position, 0.2f);
        }
    }
}
