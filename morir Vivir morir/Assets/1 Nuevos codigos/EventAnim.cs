using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnim : MonoBehaviour
{
    public SaltJugador saltoJugador;
    public AgarraYLanzar agarrarYLanzar;

    public void EventoSaltar()
    {
        Debug.Log("EVENTO SALTAR ACTIVADO");

        if (saltoJugador != null)
        {
            Debug.Log("ENVIANDO SALTO A SaltJugador");
            saltoJugador.EventoSaltar();
        }
        else
        {
            Debug.Log("SaltJugador ES NULL");
        }
    }

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
