using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MenuPausa : MonoBehaviour
{
    [Header("Tecla opcional")]
    public bool usarTecla = true;
    public KeyCode teclaMenu = KeyCode.Escape;

    [Header("Mando")]
    public bool usarMando = true;
    public KeyCode botonMandoMenu = KeyCode.JoystickButton9;

    [Header("Canvas")]
    public GameObject canvasMenu;
    public GameObject canvasTutorial;

    [Header("Botones seleccionados")]
    public GameObject primerBotonMenu;
    public GameObject primerBotonTutorial;

    [Header("Panel que baja")]
    public RectTransform panelMenu;
    public Vector2 posicionOculta = new Vector2(0, 900);
    public Vector2 posicionVisible = new Vector2(0, 0);
    public float velocidadAnimacion = 10f;

    private bool pausado = false;
    private Coroutine rutinaMenu;

    private void Start()
    {
        Time.timeScale = 1f;

        if (canvasMenu != null)
            canvasMenu.SetActive(false);

        if (canvasTutorial != null)
            canvasTutorial.SetActive(false);

        if (panelMenu != null)
            panelMenu.anchoredPosition = posicionOculta;
    }

    private void Update()
    {
        bool presionoMenu = false;

        if (usarTecla && Input.GetKeyDown(teclaMenu))
            presionoMenu = true;

        if (usarMando && Input.GetKeyDown(botonMandoMenu))
            presionoMenu = true;

        if (presionoMenu)
        {
            if (pausado)
                Reanudar();
            else
                AbrirMenu();
        }
    }

    public void AbrirMenu()
    {
        pausado = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (canvasTutorial != null)
            canvasTutorial.SetActive(false);

        if (canvasMenu != null)
            canvasMenu.SetActive(true);

        SeleccionarBoton(primerBotonMenu);

        if (rutinaMenu != null)
            StopCoroutine(rutinaMenu);

        rutinaMenu = StartCoroutine(MoverPanel(posicionVisible));
    }

    public void Reanudar()
    {
        pausado = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (canvasTutorial != null)
            canvasTutorial.SetActive(false);

        if (rutinaMenu != null)
            StopCoroutine(rutinaMenu);

        rutinaMenu = StartCoroutine(CerrarMenu());
    }

    public void AbrirTutorial()
    {
        if (canvasMenu != null)
            canvasMenu.SetActive(false);

        if (canvasTutorial != null)
            canvasTutorial.SetActive(true);

        SeleccionarBoton(primerBotonTutorial);
    }

    public void VolverAlMenu()
    {
        if (canvasTutorial != null)
            canvasTutorial.SetActive(false);

        if (canvasMenu != null)
            canvasMenu.SetActive(true);

        if (panelMenu != null)
            panelMenu.anchoredPosition = posicionVisible;

        SeleccionarBoton(primerBotonMenu);
    }

    public void SalirJuego()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("SALIR DEL JUEGO");
    }

    private void SeleccionarBoton(GameObject boton)
    {
        if (boton == null) return;
        if (EventSystem.current == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(boton);
    }

    private IEnumerator CerrarMenu()
    {
        yield return MoverPanel(posicionOculta);

        if (canvasMenu != null)
            canvasMenu.SetActive(false);
    }

    private IEnumerator MoverPanel(Vector2 destino)
    {
        if (panelMenu == null)
            yield break;

        while (Vector2.Distance(panelMenu.anchoredPosition, destino) > 1f)
        {
            panelMenu.anchoredPosition = Vector2.Lerp(
                panelMenu.anchoredPosition,
                destino,
                velocidadAnimacion * Time.unscaledDeltaTime
            );

            yield return null;
        }

        panelMenu.anchoredPosition = destino;
    }
}
