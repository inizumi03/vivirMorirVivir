using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmisorLuz : MonoBehaviour
{
    [Header("Luz")]
    public Transform puntoSalida;
    public float distanciaMaxima = 30f;
    public LayerMask capasDetectables;

    [Header("Visual")]
    public LineRenderer linea;

    [Header("Rebotes")]
    public int maximosRebotes = 3;

    private void Update()
    {
        DispararLuz();
    }

    private void DispararLuz()
    {
        if (puntoSalida == null || linea == null) return;

        linea.positionCount = 1;
        linea.SetPosition(0, puntoSalida.position);

        Vector3 origen = puntoSalida.position;
        Vector3 direccion = puntoSalida.forward;

        for (int i = 0; i < maximosRebotes; i++)
        {
            if (Physics.Raycast(origen, direccion, out RaycastHit hit, distanciaMaxima, capasDetectables))
            {
                AgregarPunto(hit.point);

                ReceptorLuz receptor = hit.collider.GetComponentInParent<ReceptorLuz>();

                if (receptor != null)
                {
                    receptor.RecibirLuz();
                    return;
                }

                EspejoLuz espejo = hit.collider.GetComponentInParent<EspejoLuz>();

                if (espejo != null)
                {
                    origen = espejo.ObtenerPosicionSalida();
                    direccion = espejo.ObtenerDireccionSalida();
                    AgregarPunto(origen);
                }
                else
                {
                    return;
                }
            }
            else
            {
                AgregarPunto(origen + direccion * distanciaMaxima);
                return;
            }
        }
    }

    private void AgregarPunto(Vector3 punto)
    {
        linea.positionCount++;
        linea.SetPosition(linea.positionCount - 1, punto);
    }
}
