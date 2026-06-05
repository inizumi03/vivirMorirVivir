using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorCambioForma : MonoBehaviour
{
    [Header("Jugador")]
    public string tagJugador = "Player";
    public movJugador movimientoJugador;

    [Header("UI")]
    public GameObject imagenPresionarQ;
    public GameObject menuCambioForma;

    private bool jugadorEnRango = false;
    private bool menuAbierto = false;

    private void Start()
    {
        if (imagenPresionarQ != null)
            imagenPresionarQ.SetActive(false);

        if (menuCambioForma != null)
            menuCambioForma.SetActive(false);
    }

    private void Update()
    {
        if (!jugadorEnRango) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (menuAbierto)
                CerrarMenu();
            else
                AbrirMenu();
        }
    }

    private void AbrirMenu()
    {
        menuAbierto = true;

        if (menuCambioForma != null)
            menuCambioForma.SetActive(true);

        if (imagenPresionarQ != null)
            imagenPresionarQ.SetActive(false);

        if (movimientoJugador != null)
            movimientoJugador.BloquearMovimiento();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarMenu()
    {
        menuAbierto = false;

        if (menuCambioForma != null)
            menuCambioForma.SetActive(false);

        if (jugadorEnRango && imagenPresionarQ != null)
            imagenPresionarQ.SetActive(true);

        if (movimientoJugador != null)
            movimientoJugador.DesbloquearMovimiento();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            jugadorEnRango = true;

            if (!menuAbierto && imagenPresionarQ != null)
                imagenPresionarQ.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            jugadorEnRango = false;

            if (imagenPresionarQ != null)
                imagenPresionarQ.SetActive(false);

            if (menuAbierto)
                CerrarMenu();
        }
    }
}
