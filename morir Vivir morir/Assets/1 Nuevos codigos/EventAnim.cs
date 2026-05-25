using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnim : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Referencias")]
    public AgarraYLanzar agarrarYLanzar;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void ActivarSalto()
    {
        if (animator != null)
        {
            animator.SetTrigger("Salto");
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
