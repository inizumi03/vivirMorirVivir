using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerTutoriales : MonoBehaviour
{
    [System.Serializable]
    public class Tutorial
    {
        [Header("Canvas")]
        public GameObject canvasTutorial;

        [Header("Texto")]
        public TextMeshProUGUI textoTutorial;

        [TextArea(3, 8)]
        public List<string> mensajes =
            new List<string>();

        [Header("Botón")]
        public Button botonContinuar;

        [Header("Configuración")]
        public bool soloUnaVez = true;

        [HideInInspector]
        public bool completado = false;
    }

    [Header("Tutoriales")]
    public List<Tutorial> tutoriales =
        new List<Tutorial>();

    [Header("Escritura")]
    public float velocidadEscritura = 0.04f;

    [Header("Opciones")]
    public bool pausarJuegoDuranteTutorial = false;

    private Tutorial tutorialActual;
    private int indiceTutorialActual = -1;
    private int indiceMensajeActual = 0;

    private string mensajeActual = "";

    private bool escribiendo = false;
    private bool tutorialActivo = false;

    private Coroutine rutinaEscritura;

    private CursorLockMode estadoCursorAnterior;
    private bool cursorVisibleAnterior;

    private void Start()
    {
        DesactivarTodosLosCanvas();
    }

    public void ActivarTutorial(int indice)
    {
        if (tutorialActivo)
            return;

        if (indice < 0 || indice >= tutoriales.Count)
            return;

        Tutorial nuevoTutorial =
            tutoriales[indice];

        if (nuevoTutorial == null)
            return;

        if (nuevoTutorial.soloUnaVez &&
            nuevoTutorial.completado)
        {
            return;
        }

        if (nuevoTutorial.canvasTutorial == null ||
            nuevoTutorial.textoTutorial == null ||
            nuevoTutorial.botonContinuar == null)
        {
            return;
        }

        if (nuevoTutorial.mensajes == null ||
            nuevoTutorial.mensajes.Count == 0)
        {
            return;
        }

        tutorialActual = nuevoTutorial;
        indiceTutorialActual = indice;
        indiceMensajeActual = 0;
        tutorialActivo = true;

        estadoCursorAnterior = Cursor.lockState;
        cursorVisibleAnterior = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        tutorialActual.canvasTutorial.SetActive(true);

        tutorialActual.botonContinuar.onClick.RemoveListener(
            ContinuarTutorial
        );

        tutorialActual.botonContinuar.onClick.AddListener(
            ContinuarTutorial
        );

        if (pausarJuegoDuranteTutorial)
        {
            Time.timeScale = 0f;
        }

        MostrarMensajeActual();
    }

    public void ContinuarTutorial()
    {
        if (!tutorialActivo ||
            tutorialActual == null)
        {
            return;
        }

        if (escribiendo)
        {
            CompletarMensajeActual();
            return;
        }

        indiceMensajeActual++;

        if (indiceMensajeActual <
            tutorialActual.mensajes.Count)
        {
            MostrarMensajeActual();
        }
        else
        {
            CerrarTutorial();
        }
    }

    private void MostrarMensajeActual()
    {
        if (tutorialActual == null)
            return;

        if (indiceMensajeActual < 0 ||
            indiceMensajeActual >=
            tutorialActual.mensajes.Count)
        {
            return;
        }

        mensajeActual =
            tutorialActual.mensajes[
                indiceMensajeActual
            ];

        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
        }

        rutinaEscritura =
            StartCoroutine(
                EscribirMensaje()
            );
    }

    private IEnumerator EscribirMensaje()
    {
        escribiendo = true;

        tutorialActual.textoTutorial.text = "";

        foreach (char letra in mensajeActual)
        {
            tutorialActual.textoTutorial.text +=
                letra;

            yield return new WaitForSecondsRealtime(
                velocidadEscritura
            );
        }

        tutorialActual.textoTutorial.text =
            mensajeActual;

        escribiendo = false;
        rutinaEscritura = null;
    }

    private void CompletarMensajeActual()
    {
        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
            rutinaEscritura = null;
        }

        if (tutorialActual != null &&
            tutorialActual.textoTutorial != null)
        {
            tutorialActual.textoTutorial.text =
                mensajeActual;
        }

        escribiendo = false;
    }

    private void CerrarTutorial()
    {
        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
            rutinaEscritura = null;
        }

        if (tutorialActual != null)
        {
            tutorialActual.completado = true;

            if (tutorialActual.botonContinuar != null)
            {
                tutorialActual.botonContinuar.onClick
                    .RemoveListener(
                        ContinuarTutorial
                    );
            }

            if (tutorialActual.canvasTutorial != null)
            {
                tutorialActual.canvasTutorial.SetActive(
                    false
                );
            }
        }

        if (pausarJuegoDuranteTutorial)
        {
            Time.timeScale = 1f;
        }

        Cursor.lockState = estadoCursorAnterior;
        Cursor.visible = cursorVisibleAnterior;

        tutorialActual = null;
        indiceTutorialActual = -1;
        indiceMensajeActual = 0;
        mensajeActual = "";
        escribiendo = false;
        tutorialActivo = false;
    }

    private void DesactivarTodosLosCanvas()
    {
        foreach (Tutorial tutorial in tutoriales)
        {
            if (tutorial == null)
                continue;

            if (tutorial.canvasTutorial != null)
            {
                tutorial.canvasTutorial.SetActive(
                    false
                );
            }
        }
    }

    public bool TutorialEstaActivo()
    {
        return tutorialActivo;
    }

    public int ObtenerTutorialActual()
    {
        return indiceTutorialActual;
    }

    private void OnDisable()
    {
        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
            rutinaEscritura = null;
        }

        if (pausarJuegoDuranteTutorial)
        {
            Time.timeScale = 1f;
        }

        Cursor.lockState = estadoCursorAnterior;
        Cursor.visible = cursorVisibleAnterior;
    }
}
