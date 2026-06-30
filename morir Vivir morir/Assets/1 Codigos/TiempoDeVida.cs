using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiempoDeVida : MonoBehaviour
{
    [Header("Vida del cuerpo")]
    public float tiempoDeVida = 10f;
    private float tiempoActual;

    [Header("UI")]
    public Image circuloVida;


    private void Start()
    {
        tiempoActual = tiempoDeVida;

        if (circuloVida != null)
        {
            circuloVida.fillAmount = 1f;
        }
    }

    private void Update()
    {
        tiempoActual -= Time.deltaTime;

        if (circuloVida != null)
        {
            circuloVida.fillAmount = tiempoActual / tiempoDeVida;
        }

        if (tiempoActual <= 0f)
        {
            Destroy(gameObject);
        }
    }
}