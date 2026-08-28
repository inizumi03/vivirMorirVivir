using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CambioForma : MonoBehaviour
{
    [Header("Modelos")]
    //public GameObject robotBase;
    //public GameObject robotSalto;
    //public GameObject robotMetal;
    public List<GameObject> listaRobots = new List<GameObject>();

    [Header("Imagen forma pendiente")]
    public Image imagenForma;

    //public Sprite spriteBase;
    //public Sprite spriteSalto;
    //public Sprite spriteMetal;
    public List<Sprite> listaSprite = new List<Sprite>();


    private int formaActual = 0;
    private int formaPendiente = 0;

    private void Start()
    {
        //ActivarForma(formaActual);
        ActualizarImagenForma();
    }

    public void ElegirForma(int elegido)
    {
        formaPendiente = elegido;
        ActualizarImagenForma();
    }

    //public void ElegirBase()
    //{
    //    formaPendiente = 0;
    //    ActualizarImagenForma();
    //    Debug.Log("Forma pendiente: BASE");
    //}

    //public void ElegirSalto()
    //{
    //    formaPendiente = 1;
    //    ActualizarImagenForma();
    //    Debug.Log("Forma pendiente: SALTO");
    //}

    //public void ElegirMetal()
    //{
    //    formaPendiente = 2;
    //    ActualizarImagenForma();
    //    Debug.Log("Forma pendiente: METAL");
    //}

    public void AplicarFormaPendiente()
    {
        //formaActual = formaPendiente;
        //ActivarForma(formaActual);

        listaRobots[formaActual].SetActive(false);
        listaRobots[formaPendiente].SetActive(true);
        formaActual = formaPendiente;
    }
    //se simplifico el metodo aplicar forma pendiente fusionandolo con el con le de con el activar forma para esto se modificaron los 3 game obj de los robots por una lista 

    //private void ActivarForma(int forma)
    //{
    //    //if (robotBase != null)
    //    //    robotBase.SetActive(false);

    //    //if (robotSalto != null)
    //    //    robotSalto.SetActive(false);

    //    //if (robotMetal != null)
    //    //    robotMetal.SetActive(false);

    //    //if (forma == 0 && robotBase != null)
    //    //    robotBase.SetActive(true);

    //    //if (forma == 1 && robotSalto != null)
    //    //    robotSalto.SetActive(true);

    //    //if (forma == 2 && robotMetal != null)
    //    //    robotMetal.SetActive(true);

    //    foreach (GameObject robot in listaRobots)
    //    {
    //        if (robot != null)
    //            robot.SetActive(false);
    //    }

    //    listaRobots[forma].SetActive(true);
    //}

    private void ActualizarImagenForma()
    {
        imagenForma.sprite = listaSprite[formaPendiente];

        //if (imagenForma == null)
        //    return;

        //switch (formaPendiente)
        //{
        //    case 0:
        //        imagenForma.sprite = spriteBase;
        //        break;

        //    case 1:
        //        imagenForma.sprite = spriteSalto;
        //        break;

        //    case 2:
        //        imagenForma.sprite = spriteMetal;
        //        break;
        //}
    }

    public int ObtenerFormaActual()
    {
        return formaActual;
    }

    public bool EstaEnFormaSalto()
    {
        return formaActual == 1;
    }

    public bool EstaEnFormaBase()
    {
        return formaActual == 0;
    }
}
