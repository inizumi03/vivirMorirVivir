using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBalanza : MonoBehaviour
{
    [System.Serializable]
    public class GrupoBalanza
    {
        [Header("Balanza")]
        public Balanza balanza;

        [Header("Caja que sube")]
        public Transform cajaSube;
        public float alturaSube = 3f;

        [HideInInspector]
        public Vector3 posicionInicial;

        [HideInInspector]
        public Vector3 posicionFinal;
    }

    [Header("Balanzas del puzle")]
    public List<GrupoBalanza> balanzas =
        new List<GrupoBalanza>();

    [Header("Caja central que baja")]
    public Transform cajaCentral;
    public float alturaBajaCajaCentral = 3f;

    [Header("Movimiento")]
    public float velocidad = 3f;

    private Vector3 posicionInicialCajaCentral;
    private Vector3 posicionFinalCajaCentral;

    private void Start()
    {
        PrepararBalanzas();
        PrepararCajaCentral();
    }

    private void Update()
    {
        bool algunaBalanzaActiva = false;

        foreach (GrupoBalanza grupo in balanzas)
        {
            if (grupo == null)
                continue;

            bool balanzaActiva =
                grupo.balanza != null &&
                grupo.balanza.EstaActiva();

            if (balanzaActiva)
            {
                algunaBalanzaActiva = true;
            }

            MoverCajaDeBalanza(
                grupo,
                balanzaActiva
            );
        }

        MoverCajaCentral(
            algunaBalanzaActiva
        );
    }

    private void PrepararBalanzas()
    {
        foreach (GrupoBalanza grupo in balanzas)
        {
            if (grupo == null ||
                grupo.cajaSube == null)
            {
                continue;
            }

            grupo.posicionInicial =
                grupo.cajaSube.position;

            grupo.posicionFinal =
                grupo.posicionInicial +
                Vector3.up * grupo.alturaSube;
        }
    }

    private void PrepararCajaCentral()
    {
        if (cajaCentral == null)
            return;

        posicionInicialCajaCentral =
            cajaCentral.position;

        posicionFinalCajaCentral =
            posicionInicialCajaCentral +
            Vector3.down * alturaBajaCajaCentral;
    }

    private void MoverCajaDeBalanza(
        GrupoBalanza grupo,
        bool balanzaActiva)
    {
        if (grupo.cajaSube == null)
            return;

        Vector3 destino;

        if (balanzaActiva)
        {
            destino = grupo.posicionFinal;
        }
        else
        {
            destino = grupo.posicionInicial;
        }

        grupo.cajaSube.position =
            Vector3.MoveTowards(
                grupo.cajaSube.position,
                destino,
                velocidad * Time.deltaTime
            );
    }

    private void MoverCajaCentral(
        bool algunaBalanzaActiva)
    {
        if (cajaCentral == null)
            return;

        Vector3 destino;

        /*
         * Cuando al menos una balanza está llena,
         * la caja central baja y tapa la puerta.
         */
        if (algunaBalanzaActiva)
        {
            destino = posicionFinalCajaCentral;
        }
        else
        {
            destino = posicionInicialCajaCentral;
        }

        cajaCentral.position =
            Vector3.MoveTowards(
                cajaCentral.position,
                destino,
                velocidad * Time.deltaTime
            );
    }
}
