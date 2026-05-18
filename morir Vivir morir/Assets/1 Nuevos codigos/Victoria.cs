using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victoria : MonoBehaviour
{
    [Header("Detección")]
    public string tagJugador = "Player";

    [Header("Canvas Victoria")]
    public GameObject canvasVictoria;

    private bool activado = false;

    private void Start()
    {
        if (canvasVictoria != null)
        {
            canvasVictoria.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;

        if (other.CompareTag(tagJugador))
        {
            ActivarVictoria();
        }
    }

    private void ActivarVictoria()
    {
        activado = true;

        if (canvasVictoria != null)
        {
            canvasVictoria.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        Debug.Log("VICTORIA");
    }

    // BOTON REPLAY
    public void Reintentar()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
