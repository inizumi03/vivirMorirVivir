using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptorLuz : MonoBehaviour
{
    [Header("Estado")]
    public bool recibiendoLuz;

    [Header("Objeto a activar")]
    public GameObject objetoActivar;

    private void Update()
    {
        if (objetoActivar != null)
            objetoActivar.SetActive(recibiendoLuz);

        recibiendoLuz = false;
    }

    public void RecibirLuz()
    {
        recibiendoLuz = true;
    }
}
