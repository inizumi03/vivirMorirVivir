using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dialogos : MonoBehaviour
{
    [Header("Texto")]
    public TextMeshProUGUI textoDialogo;

    [Header("Diálogos")]
    [TextArea(3, 8)]
    public List<string> dialogos =
        new List<string>();

    [Header("Escritura")]
    public float velocidadEscritura = 0.04f;

    [Header("Botón")]
    public Button botonContinuar;

    [Header("Escena siguiente")]
    public string nombreEscenaSiguiente;

    private int indiceDialogo = 0;
    private bool escribiendo = false;
    private string dialogoActual = "";

    private Coroutine rutinaEscritura;

    private void Start()
    {
        Time.timeScale = 1f;

        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(ContinuarDialogo);
            botonContinuar.interactable = true;
        }
        else
        {
            Debug.LogError(
                "No asignaste el botón continuar.",
                gameObject
            );
        }

        if (textoDialogo == null)
        {
            Debug.LogError(
                "No asignaste el texto del diálogo.",
                gameObject
            );

            return;
        }

        if (dialogos == null || dialogos.Count == 0)
        {
            Debug.LogWarning(
                "La lista de diálogos está vacía.",
                gameObject
            );

            return;
        }

        indiceDialogo = 0;
        MostrarDialogoActual();
    }

    public void ContinuarDialogo()
    {
        Debug.Log("Botón continuar presionado");

        if (dialogos == null || dialogos.Count == 0)
            return;

        if (escribiendo)
        {
            CompletarTextoActual();
            return;
        }

        indiceDialogo++;

        if (indiceDialogo < dialogos.Count)
        {
            MostrarDialogoActual();
        }
        else
        {
            CargarEscenaSiguiente();
        }
    }

    private void MostrarDialogoActual()
    {
        if (textoDialogo == null)
            return;

        if (indiceDialogo < 0 ||
            indiceDialogo >= dialogos.Count)
        {
            return;
        }

        dialogoActual = dialogos[indiceDialogo];

        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
        }

        rutinaEscritura =
            StartCoroutine(EscribirDialogo());
    }

    private IEnumerator EscribirDialogo()
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in dialogoActual)
        {
            textoDialogo.text += letra;

            yield return new WaitForSecondsRealtime(
                velocidadEscritura
            );
        }

        textoDialogo.text = dialogoActual;

        escribiendo = false;
        rutinaEscritura = null;
    }

    private void CompletarTextoActual()
    {
        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
            rutinaEscritura = null;
        }

        textoDialogo.text = dialogoActual;
        escribiendo = false;
    }

    private void CargarEscenaSiguiente()
    {
        if (string.IsNullOrWhiteSpace(
            nombreEscenaSiguiente))
        {
            Debug.LogError(
                "No escribiste el nombre de la escena siguiente.",
                gameObject
            );

            return;
        }

        SceneManager.LoadScene(
            nombreEscenaSiguiente
        );
    }

    private void OnDestroy()
    {
        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveListener(
                ContinuarDialogo
            );
        }
    }
}
