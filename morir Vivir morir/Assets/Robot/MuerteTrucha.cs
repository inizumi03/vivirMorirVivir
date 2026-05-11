using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuerteTrucha : MonoBehaviour
{
    [Header("Canvas")]
    public CanvasGroup canvasGroup;

    [Header("Velocidad")]
    public float velocidadFade = 2f;

    public bool ocupado = false;

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    public IEnumerator FadeRespawn(System.Action accionRespawn)
    {
        if (ocupado) yield break;

        ocupado = true;

        // FADE A NEGRO
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * velocidadFade;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // RESPAWN
        accionRespawn?.Invoke();

        yield return new WaitForSeconds(0.1f);

        // SACAR NEGRO
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * velocidadFade;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        ocupado = false;
    }
}
