using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLaser : MonoBehaviour
{
    [Header("Láseres sincronizados")]
    public List<Nivel2Laser> lasers =
        new List<Nivel2Laser>();

    [Header("Movimiento general")]
    public float velocidadRecorrido = 0.5f;

    [Tooltip("Tiempo que esperan todos los láseres al llegar a un extremo.")]
    public float tiempoEspera = 0f;

    [Header("Estado")]
    [SerializeField]
    [Range(0f, 1f)]
    private float progreso;

    [SerializeField]
    private bool haciaPuntoB = true;

    private float tiempoEsperando;
    private bool esperando;

    private void Start()
    {
        PrepararLasers();
        EnviarProgreso();
    }

    private void Update()
    {
        if (esperando)
        {
            tiempoEsperando += Time.deltaTime;

            if (tiempoEsperando >= tiempoEspera)
            {
                esperando = false;
                tiempoEsperando = 0f;
                haciaPuntoB = !haciaPuntoB;
            }

            EnviarProgreso();
            return;
        }

        float direccion = haciaPuntoB ? 1f : -1f;

        progreso +=
            direccion *
            velocidadRecorrido *
            Time.deltaTime;

        if (progreso >= 1f)
        {
            progreso = 1f;
            LlegarAlExtremo();
        }
        else if (progreso <= 0f)
        {
            progreso = 0f;
            LlegarAlExtremo();
        }

        EnviarProgreso();
    }

    private void PrepararLasers()
    {
        foreach (Nivel2Laser laser in lasers)
        {
            if (laser == null)
                continue;

            laser.UsarManager(this);
        }
    }

    private void EnviarProgreso()
    {
        foreach (Nivel2Laser laser in lasers)
        {
            if (laser == null)
                continue;

            laser.ActualizarDesdeManager(
                progreso
            );
        }
    }

    private void LlegarAlExtremo()
    {
        if (tiempoEspera > 0f)
        {
            esperando = true;
            tiempoEsperando = 0f;
        }
        else
        {
            haciaPuntoB = !haciaPuntoB;
        }
    }

    public float ObtenerProgreso()
    {
        return progreso;
    }

    public bool VaHaciaPuntoB()
    {
        return haciaPuntoB;
    }
}
