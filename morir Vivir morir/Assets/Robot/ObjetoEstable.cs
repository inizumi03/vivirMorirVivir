using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoEstable : MonoBehaviour
{
    [Header("Acomodo en punto de carga")]
    public Vector3 offsetPosicion;
    public Vector3 offsetRotacion;

    [Header("Estabilidad al lanzar")]
    public bool estabilizarRotacion = true;
    public float velocidadAcomodo = 5f;
    public bool bloquearRotacionX = true;
    public bool bloquearRotacionZ = true;

    private Rigidbody rb;
    private Collider[] collidersObjeto;
    private Collider[] collidersIgnorados;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collidersObjeto = GetComponentsInChildren<Collider>();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;
        if (!estabilizarRotacion) return;

        Vector3 rotacionActual = transform.eulerAngles;

        float x = bloquearRotacionX ? 0f : rotacionActual.x;
        float z = bloquearRotacionZ ? 0f : rotacionActual.z;

        Quaternion rotacionObjetivo = Quaternion.Euler(x, rotacionActual.y, z);

        rb.MoveRotation(Quaternion.Slerp(
            rb.rotation,
            rotacionObjetivo,
            velocidadAcomodo * Time.fixedDeltaTime
        ));

        Vector3 velAngular = rb.angularVelocity;

        if (bloquearRotacionX) velAngular.x = 0f;
        if (bloquearRotacionZ) velAngular.z = 0f;

        rb.angularVelocity = velAngular;
    }

    public Vector3 ObtenerPosicionCarga(Transform puntoCarga)
    {
        return puntoCarga.position + puntoCarga.TransformDirection(offsetPosicion);
    }

    public Quaternion ObtenerRotacionCarga(Transform puntoCarga)
    {
        return puntoCarga.rotation * Quaternion.Euler(offsetRotacion);
    }

    public void IgnorarColisionConJugador(Collider[] collidersJugador, bool ignorar)
    {
        if (collidersJugador == null) return;

        foreach (Collider colObjeto in collidersObjeto)
        {
            foreach (Collider colJugador in collidersJugador)
            {
                if (colObjeto != null && colJugador != null)
                {
                    Physics.IgnoreCollision(colObjeto, colJugador, ignorar);
                }
            }
        }
    }
}
