using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SombraJugador : MonoBehaviour
{
    [Header("Jugador")]
    public Transform jugador;

    [Header("Detección del suelo")]
    public LayerMask capasSuelo;
    public float distanciaRaycast = 20f;
    public float alturaInicioRaycast = 0.5f;
    public float separacionDelSuelo = 0.02f;

    private void LateUpdate()
    {
        if (jugador == null)
            return;

        ColocarSombraEnElSuelo();
    }

    private void ColocarSombraEnElSuelo()
    {
        Vector3 origen =
            jugador.position + Vector3.up * alturaInicioRaycast;

        RaycastHit hit;

        bool encontroSuelo = Physics.Raycast(
            origen,
            Vector3.down,
            out hit,
            distanciaRaycast,
            capasSuelo,
            QueryTriggerInteraction.Ignore
        );

        if (!encontroSuelo)
            return;

        /*
         * La sombra sigue al jugador en X y Z,
         * pero su altura queda sobre la superficie detectada.
         */
        Vector3 nuevaPosicion = transform.position;

        nuevaPosicion.x = jugador.position.x;
        nuevaPosicion.y = hit.point.y + separacionDelSuelo;
        nuevaPosicion.z = jugador.position.z;

        transform.position = nuevaPosicion;
    }

    private void OnDrawGizmosSelected()
    {
        if (jugador == null)
            return;

        Vector3 origen =
            jugador.position + Vector3.up * alturaInicioRaycast;

        Gizmos.DrawLine(
            origen,
            origen + Vector3.down * distanciaRaycast
        );
    }
}
