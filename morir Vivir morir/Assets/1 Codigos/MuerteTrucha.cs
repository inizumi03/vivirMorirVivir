using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuerteTrucha : MonoBehaviour
{
    [Header("Canvas")]
    public CanvasGroup canvasGroup;

    [Header("Velocidad")]
    public float velocidadFade = 2f;

    [Header("Tiempo de respawn")]
    public float tiempoAntesDelRespawn = 0.5f;
    public float tiempoDespuesDelRespawn = 0.1f;

    public bool ocupado = false;

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    public IEnumerator FadeRespawn(
        System.Action accionRespawn)
    {
        if (ocupado)
            yield break;

        ocupado = true;

        if (canvasGroup == null)
        {
            yield return new WaitForSecondsRealtime(
                tiempoAntesDelRespawn
            );

            accionRespawn?.Invoke();

            ocupado = false;
            yield break;
        }

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha +=
                Time.unscaledDeltaTime *
                velocidadFade;

            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(
            tiempoAntesDelRespawn
        );

        accionRespawn?.Invoke();

        yield return new WaitForSecondsRealtime(
            tiempoDespuesDelRespawn
        );

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -=
                Time.unscaledDeltaTime *
                velocidadFade;

            yield return null;
        }

        canvasGroup.alpha = 0f;

        ocupado = false;
    }
}
