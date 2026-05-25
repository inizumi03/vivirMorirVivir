using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnim : MonoBehaviour
{
    public AgarraYLanzar agarrarYLanzar;

    public void EventoFinAgarre()
    {
        Debug.Log("EVENTO FIN AGARRE ACTIVADO");

        if (agarrarYLanzar != null)
        {
            Debug.Log("ENVIANDO EVENTO A AgarrarYLanzar");
            agarrarYLanzar.EventoFinAgarre();
        }
        else
        {
            Debug.Log("AgarrarYLanzar ES NULL");
        }
    }
}
