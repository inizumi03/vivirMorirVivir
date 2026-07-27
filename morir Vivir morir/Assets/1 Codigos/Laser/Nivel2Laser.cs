using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel2Laser : MonoBehaviour
{
    [Header("Puntos del láser")]
    public Transform puntoA;
    public Transform puntoB;

    [Header("Movimiento")]
    public float velocidad = 3f;

    [Tooltip("Distancia necesaria para cambiar de dirección.")]
    public float distanciaParaCambiar = 0.03f;

    [Header("Ejes bloqueados")]
    public bool bloquearX;
    public bool bloquearY;
    public bool bloquearZ;

    [Header("Salida del láser")]
    [Tooltip("Desactiva el movimiento cuando el cadáver abandona el Trigger.")]
    public bool desactivarAlSalir = false;

    private readonly List<CadaverEnLaser> cadaveres =
        new List<CadaverEnLaser>();
    private GameObject cadaverActual;
    private void FixedUpdate()
    {
        MoverCadaveres();
    }

    private void OnTriggerEnter(Collider other)
    {
        DetectarCadaver(other);
    }

    private void OnTriggerStay(Collider other)
    {
        DetectarCadaver(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!desactivarAlSalir)
            return;

        CadaverPlataforma cadaver =
            other.GetComponentInParent<CadaverPlataforma>();

        if (cadaver == null)
            return;

        QuitarCadaver(cadaver);
    }

    private void DetectarCadaver(Collider other)
    {
        CadaverPlataforma cadaver =
            other.GetComponentInParent<CadaverPlataforma>();

        if (cadaver == null)
            return;

        if (puntoA == null || puntoB == null)
        {
            Debug.LogWarning(
                "Nivel2Laser necesita PuntoA y PuntoB.",
                gameObject
            );

            return;
        }

        if (BuscarCadaver(cadaver) != null)
            return;

        AgregarCadaver(cadaver);
    }

    private void AgregarCadaver(
     CadaverPlataforma cadaver)
    {
        // Si ya existe un cadáver controlando este láser,
        // lo destruimos antes de agregar el nuevo.
        if (cadaverActual != null &&
            cadaverActual != cadaver.gameObject)
        {
            Destroy(cadaverActual);
        }

        // Guardamos el nuevo cadáver como el activo.
        cadaverActual = cadaver.gameObject;

        Vector3 posicionActual =
            ObtenerPosicionCadaver(cadaver);

        Vector3 posicionSobreRecorrido =
            ObtenerPuntoMasCercanoDelSegmento(
                posicionActual,
                puntoA.position,
                puntoB.position
            );

        Vector3 posicionBloqueada =
            posicionSobreRecorrido;

        Transform objetivoInicial =
            ObtenerPuntoMasLejano(
                posicionSobreRecorrido
            );

        CadaverEnLaser nuevoCadaver =
            new CadaverEnLaser();

        nuevoCadaver.cadaver = cadaver;
        nuevoCadaver.objetivoActual = objetivoInicial;
        nuevoCadaver.posicionBloqueada = posicionBloqueada;

        cadaveres.Add(nuevoCadaver);

        cadaver.ActivarComoPlataforma();

        Vector3 posicionFinal =
            AplicarEjesBloqueados(
                posicionSobreRecorrido,
                posicionBloqueada
            );

        Vector3 desplazamiento =
            posicionFinal - posicionActual;

        cadaver.MoverPlataforma(
            posicionFinal,
            desplazamiento
        );
    }
    private void MoverCadaveres()
    {
        for (int i = cadaveres.Count - 1; i >= 0; i--)
        {
            CadaverEnLaser datos = cadaveres[i];

            if (datos == null ||
                datos.cadaver == null)
            {
                cadaveres.RemoveAt(i);
                continue;
            }

            MoverCadaver(datos);
        }
    }

    private void MoverCadaver(
        CadaverEnLaser datos)
    {
        if (datos.objetivoActual == null)
        {
            datos.objetivoActual = puntoB;
        }

        Vector3 posicionAnterior =
            ObtenerPosicionCadaver(
                datos.cadaver
            );

        Vector3 posicionSobreRecorrido =
            ObtenerPuntoMasCercanoDelSegmento(
                posicionAnterior,
                puntoA.position,
                puntoB.position
            );

        posicionSobreRecorrido =
            AplicarEjesBloqueados(
                posicionSobreRecorrido,
                datos.posicionBloqueada
            );

        Vector3 posicionObjetivo =
            AplicarEjesBloqueados(
                datos.objetivoActual.position,
                datos.posicionBloqueada
            );

        Vector3 nuevaPosicion =
            Vector3.MoveTowards(
                posicionSobreRecorrido,
                posicionObjetivo,
                velocidad * Time.fixedDeltaTime
            );

        Vector3 desplazamiento =
            nuevaPosicion - posicionAnterior;

        datos.cadaver.MoverPlataforma(
            nuevaPosicion,
            desplazamiento
        );

        float distancia =
            Vector3.Distance(
                nuevaPosicion,
                posicionObjetivo
            );

        if (distancia <= distanciaParaCambiar)
        {
            CambiarObjetivo(datos);
        }
    }

    private Vector3 AplicarEjesBloqueados(
        Vector3 posicion,
        Vector3 posicionBloqueada)
    {
        if (bloquearX)
        {
            posicion.x = posicionBloqueada.x;
        }

        if (bloquearY)
        {
            posicion.y = posicionBloqueada.y;
        }

        if (bloquearZ)
        {
            posicion.z = posicionBloqueada.z;
        }

        return posicion;
    }

    private Vector3 ObtenerPuntoMasCercanoDelSegmento(
        Vector3 posicion,
        Vector3 inicio,
        Vector3 final)
    {
        Vector3 direccion = final - inicio;

        float longitudCuadrada =
            direccion.sqrMagnitude;

        if (longitudCuadrada <= 0.0001f)
        {
            return inicio;
        }

        float porcentaje =
            Vector3.Dot(
                posicion - inicio,
                direccion
            ) / longitudCuadrada;

        porcentaje = Mathf.Clamp01(porcentaje);

        return inicio + direccion * porcentaje;
    }

    private Transform ObtenerPuntoMasLejano(
        Vector3 posicion)
    {
        float distanciaA =
            Vector3.Distance(
                posicion,
                puntoA.position
            );

        float distanciaB =
            Vector3.Distance(
                posicion,
                puntoB.position
            );

        if (distanciaA > distanciaB)
        {
            return puntoA;
        }

        return puntoB;
    }

    private void CambiarObjetivo(
        CadaverEnLaser datos)
    {
        if (datos.objetivoActual == puntoA)
        {
            datos.objetivoActual = puntoB;
        }
        else
        {
            datos.objetivoActual = puntoA;
        }
    }

    private Vector3 ObtenerPosicionCadaver(
        CadaverPlataforma cadaver)
    {
        Rigidbody rbCadaver =
            cadaver.GetComponent<Rigidbody>();

        if (rbCadaver == null)
        {
            rbCadaver =
                cadaver.GetComponentInParent<Rigidbody>();
        }

        if (rbCadaver != null)
        {
            return rbCadaver.position;
        }

        return cadaver.transform.position;
    }

    private CadaverEnLaser BuscarCadaver(
        CadaverPlataforma cadaver)
    {
        foreach (CadaverEnLaser datos in cadaveres)
        {
            if (datos.cadaver == cadaver)
            {
                return datos;
            }
        }

        return null;
    }

    private void QuitarCadaver(
    CadaverPlataforma cadaver)
    {
        for (int i = cadaveres.Count - 1; i >= 0; i--)
        {
            if (cadaveres[i].cadaver == cadaver)
            {
                cadaveres.RemoveAt(i);
            }
        }

        if (cadaverActual == cadaver.gameObject)
        {
            cadaverActual = null;
        }

        cadaver.DesactivarComoPlataforma();
    }

    private void OnDrawGizmos()
    {
        if (puntoA == null || puntoB == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            puntoA.position,
            puntoB.position
        );

        Gizmos.DrawSphere(
            puntoA.position,
            0.15f
        );

        Gizmos.DrawSphere(
            puntoB.position,
            0.15f
        );
    }

    private class CadaverEnLaser
    {
        public CadaverPlataforma cadaver;
        public Transform objetivoActual;
        public Vector3 posicionBloqueada;
    }

}
