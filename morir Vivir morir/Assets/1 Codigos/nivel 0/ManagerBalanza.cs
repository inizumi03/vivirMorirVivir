using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBalanza : MonoBehaviour
{
    [System.Serializable]
    public class GrupoBalanza
    {
        [Header("Detección de peso")]
        public Balanza balanza;

        [Header("Plataforma de la balanza que baja")]
        public Transform plataformaBalanza;

        [Tooltip("Distancia que baja la balanza cuando tiene peso.")]
        public float distanciaBajaBalanza = 0.3f;

        [Header("Caja que sube")]
        public Transform cajaSube;

        [Tooltip("Distancia que sube la caja cuando se alcanza el peso necesario.")]
        public float alturaSube = 3f;

        [HideInInspector]
        public Vector3 posicionInicialCaja;

        [HideInInspector]
        public Vector3 posicionFinalCaja;

        [HideInInspector]
        public Vector3 posicionInicialBalanza;

        [HideInInspector]
        public Vector3 posicionFinalBalanza;
    }

    [Header("Balanzas del puzle")]
    public List<GrupoBalanza> balanzas =
        new List<GrupoBalanza>();

    [Header("Caja central que baja")]
    public Transform cajaCentral;

    [Tooltip("Distancia que baja la caja central.")]
    public float alturaBajaCajaCentral = 3f;

    [Header("Movimiento")]
    public float velocidad = 3f;

    [Tooltip("Velocidad con la que baja y sube la plataforma de la balanza.")]
    public float velocidadBalanza = 2f;

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

            bool tienePeso =
                grupo.balanza != null &&
                grupo.balanza.ObtenerPesoActual() > 0;

            bool balanzaActiva =
                grupo.balanza != null &&
                grupo.balanza.EstaActiva();

            if (balanzaActiva)
            {
                algunaBalanzaActiva = true;
            }

            
            MoverPlataformaBalanza(
                grupo,
                tienePeso
            );

            
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
            if (grupo == null)
                continue;

            if (grupo.cajaSube != null)
            {
                grupo.posicionInicialCaja =
                    grupo.cajaSube.position;

                grupo.posicionFinalCaja =
                    grupo.posicionInicialCaja +
                    Vector3.up * grupo.alturaSube;
            }

            if (grupo.plataformaBalanza != null)
            {
                grupo.posicionInicialBalanza =
                    grupo.plataformaBalanza.position;

                grupo.posicionFinalBalanza =
                    grupo.posicionInicialBalanza +
                    Vector3.down *
                    grupo.distanciaBajaBalanza;
            }
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
            Vector3.down *
            alturaBajaCajaCentral;
    }

    private void MoverPlataformaBalanza(
        GrupoBalanza grupo,
        bool tienePeso)
    {
        if (grupo.plataformaBalanza == null)
            return;

        Vector3 destino;

        if (tienePeso)
        {
            destino =
                grupo.posicionFinalBalanza;
        }
        else
        {
            destino =
                grupo.posicionInicialBalanza;
        }

        grupo.plataformaBalanza.position =
            Vector3.MoveTowards(
                grupo.plataformaBalanza.position,
                destino,
                velocidadBalanza *
                Time.deltaTime
            );
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
            destino =
                grupo.posicionFinalCaja;
        }
        else
        {
            destino =
                grupo.posicionInicialCaja;
        }

        grupo.cajaSube.position =
            Vector3.MoveTowards(
                grupo.cajaSube.position,
                destino,
                velocidad *
                Time.deltaTime
            );
    }

    private void MoverCajaCentral(
        bool algunaBalanzaActiva)
    {
        if (cajaCentral == null)
            return;

        Vector3 destino;

        if (algunaBalanzaActiva)
        {
            destino =
                posicionFinalCajaCentral;
        }
        else
        {
            destino =
                posicionInicialCajaCentral;
        }

        cajaCentral.position =
            Vector3.MoveTowards(
                cajaCentral.position,
                destino,
                velocidad *
                Time.deltaTime
            );
    }
}
