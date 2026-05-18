using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraJugador : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;

    [Header("Rotación Mouse")]
    public float sensibilidadX = 200f;
    public float sensibilidadY = 150f;
    public float minY = -30f;
    public float maxY = 60f;

    private float rotacionX;
    private float rotacionY;

    [Header("Distancia")]
    public float distancia = 6f;
    public float altura = 2f;
    public float suavizado = 10f;
    public float distanciaMinima = 0.8f;

    [Header("Colisión")]
    public bool evitarParedes = true;
    public LayerMask capasColision;
    public float radioCamara = 0.35f;
    public float margenPared = 0.25f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadY * Time.deltaTime;

        rotacionX += mouseX;
        rotacionY -= mouseY;
        rotacionY = Mathf.Clamp(rotacionY, minY, maxY);

        Quaternion rotacion = Quaternion.Euler(rotacionY, rotacionX, 0f);

        Vector3 puntoObjetivo = objetivo.position + Vector3.up * altura;
        Vector3 direccion = rotacion * Vector3.back;

        Vector3 posicionDeseada = puntoObjetivo + direccion * distancia;

        if (evitarParedes)
        {
            posicionDeseada = AjustarPorColision(puntoObjetivo, direccion);
        }

        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );

        transform.LookAt(puntoObjetivo);
    }

    private Vector3 AjustarPorColision(Vector3 origen, Vector3 direccion)
    {
        RaycastHit hit;

        float distanciaFinal = distancia;

        if (Physics.SphereCast(
            origen,
            radioCamara,
            direccion,
            out hit,
            distancia,
            capasColision,
            QueryTriggerInteraction.Ignore))
        {
            distanciaFinal = Mathf.Clamp(
                hit.distance - margenPared,
                distanciaMinima,
                distancia
            );
        }

        return origen + direccion * distanciaFinal;
    }
}
