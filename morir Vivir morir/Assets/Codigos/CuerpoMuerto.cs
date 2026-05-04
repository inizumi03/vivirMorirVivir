using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuerpoMuerto : MonoBehaviour
{
    [Header("Animación")]
    public Animator animator;
    public string triggerMuerte = "morir";

    [Header("Físicas")]
    public Rigidbody rb;
    public bool activarFisicasAlFinal = true;

    private bool yaMurio = false;

    public void ActivarMuerte()
    {
        if (yaMurio) return;

        yaMurio = true;

        if (animator != null)
        {
            animator.SetTrigger(triggerMuerte);
        }
    }

    // ESTE método se llama desde un Animation Event
    public void FinalizarMuerte()
    {
        if (activarFisicasAlFinal && rb != null)
        {
            rb.isKinematic = false;
        }

        // Opcional: desactivar animator para congelar pose final
        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}
