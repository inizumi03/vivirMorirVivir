using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorBrazoAgarre : MonoBehaviour
{
    public AgarraYLanzar agarrarYLanzar;
    public CambioForma cambioForma;

    public string tagAgarrable = "Agarrable";

    public GameObject imagenAgarrar;

    private void Start()
    {
        if (imagenAgarrar != null)
            imagenAgarrar.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagAgarrable))
            return;

        // Solo la forma base puede agarrar
        if (cambioForma != null && !cambioForma.EstaEnFormaBase())
            return;

        agarrarYLanzar.SetObjetoEnRango(other.gameObject);

        if (imagenAgarrar != null)
            imagenAgarrar.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(tagAgarrable))
            return;

        if (imagenAgarrar == null)
            return;

        // Si cambia de forma mientras está en el trigger
        if (cambioForma != null && cambioForma.EstaEnFormaBase())
        {
            imagenAgarrar.SetActive(true);
            agarrarYLanzar.SetObjetoEnRango(other.gameObject);
        }
        else
        {
            imagenAgarrar.SetActive(false);
            agarrarYLanzar.QuitarObjetoEnRango(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagAgarrable))
            return;

        agarrarYLanzar.QuitarObjetoEnRango(other.gameObject);

        if (imagenAgarrar != null)
            imagenAgarrar.SetActive(false);
    }
}
