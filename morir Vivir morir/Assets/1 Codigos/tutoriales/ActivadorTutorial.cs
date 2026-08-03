using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorTutorial : MonoBehaviour
{
    [Header("Manager")]
    public ManagerTutoriales managerTutoriales;

    [Header("Tutorial que activa")]
    public int indiceTutorial = 0;

    [Header("Jugador")]
    public string tagJugador = "Player";

    [Header("Configuración")]
    public bool desactivarColliderAlUsarse = true;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado)
            return;

        if (!other.CompareTag(tagJugador))
            return;

        if (managerTutoriales == null)
            return;

        managerTutoriales.ActivarTutorial(
            indiceTutorial
        );

        activado = true;

        if (desactivarColliderAlUsarse)
        {
            Collider col =
                GetComponent<Collider>();

            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}
