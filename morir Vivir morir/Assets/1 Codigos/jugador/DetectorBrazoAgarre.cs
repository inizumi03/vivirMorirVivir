using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorBrazoAgarre : MonoBehaviour
{
    public AgarraYLanzar agarrarYLanzar;
    public string tagAgarrable = "Agarrable";
    public GameObject imagenAgarrar;

    private void Start()
    {
        if (imagenAgarrar != null)
            imagenAgarrar.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagAgarrable))
        {
            agarrarYLanzar.SetObjetoEnRango(other.gameObject);

            if (imagenAgarrar != null)
                imagenAgarrar.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagAgarrable))
        {
            agarrarYLanzar.QuitarObjetoEnRango(other.gameObject);

            if (imagenAgarrar != null)
                imagenAgarrar.SetActive(false);
        }
    }
}
