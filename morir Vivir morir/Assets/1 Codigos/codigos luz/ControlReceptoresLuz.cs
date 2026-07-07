using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlReceptoresLuz : MonoBehaviour
{
    [Header("Cantidad necesaria")]
    public int receptoresNecesarios = 3;

    [Header("Objetos a destruir")]
    public GameObject objetoDestruir1;
    public GameObject objetoDestruir2;

    [Header("Objeto a activar")]
    public GameObject objetoActivar;

    private int receptoresActivados = 0;
    private bool completado = false;

    private void Start()
    {
        if (objetoActivar != null)
            objetoActivar.SetActive(false);
    }

    public void SumarReceptor()
    {
        if (completado) return;

        receptoresActivados++;

        if (receptoresActivados >= receptoresNecesarios)
        {
            Completar();
        }
    }

    private void Completar()
    {
        completado = true;

        if (objetoDestruir1 != null)
            Destroy(objetoDestruir1);

        if (objetoDestruir2 != null)
            Destroy(objetoDestruir2);

        if (objetoActivar != null)
            objetoActivar.SetActive(true);

        Debug.Log("LOS 3 RECEPTORES FUERON ACTIVADOS");
    }
}
