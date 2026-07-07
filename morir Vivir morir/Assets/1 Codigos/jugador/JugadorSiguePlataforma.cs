using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorSiguePlataforma : MonoBehaviour
{
    [Header("Layer del piso donde pisa el jugador")]
    public LayerMask layerBasePlataforma;

    private Transform plataformaQueSeMueve;
    private Vector3 posicionAnterior;

    private void OnCollisionEnter(Collision collision)
    {
        if (!EstaEnLayer(collision.gameObject.layer)) return;

        TrampaMovil trampa =
            collision.collider.GetComponentInParent<TrampaMovil>();

        if (trampa != null)
        {
            plataformaQueSeMueve = trampa.transform;
        }
        else
        {
            plataformaQueSeMueve = collision.transform.parent != null
                ? collision.transform.parent
                : collision.transform;
        }

        posicionAnterior = plataformaQueSeMueve.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (plataformaQueSeMueve == null) return;
        if (!EstaEnLayer(collision.gameObject.layer)) return;

        Vector3 movimiento =
            plataformaQueSeMueve.position - posicionAnterior;

        transform.position += movimiento;

        posicionAnterior = plataformaQueSeMueve.position;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!EstaEnLayer(collision.gameObject.layer)) return;

        plataformaQueSeMueve = null;
    }

    private bool EstaEnLayer(int layer)
    {
        return (layerBasePlataforma.value & (1 << layer)) != 0;
    }
}
