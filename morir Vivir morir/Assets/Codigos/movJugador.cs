using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float velocidadRotacion = 12f;
    public float aceleracion = 15f;
    public float desaceleracion = 20f;

    [Header("Referencias")]
    public Transform camara;

    private Rigidbody rb;
    private Vector3 direccionMovimiento;
    private Vector3 velocidadActual;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 adelante = camara.forward;
        Vector3 derecha = camara.right;

        adelante.y = 0f;
        derecha.y = 0f;

        adelante.Normalize();
        derecha.Normalize();

        direccionMovimiento = (adelante * vertical + derecha * horizontal).normalized;
    }

    private void FixedUpdate()
    {
        MoverJugador();
        RotarJugador();
    }

    private void MoverJugador()
    {
        Vector3 velocidadObjetivo = direccionMovimiento * velocidad;

        float suavizado = direccionMovimiento.magnitude > 0.1f ? aceleracion : desaceleracion;

        velocidadActual = Vector3.MoveTowards(
            velocidadActual,
            velocidadObjetivo,
            suavizado * Time.fixedDeltaTime
        );

        Vector3 nuevaVelocidad = new Vector3(
            velocidadActual.x,
            rb.velocity.y,
            velocidadActual.z
        );

        rb.velocity = nuevaVelocidad;
    }

    private void RotarJugador()
    {
        if (direccionMovimiento.magnitude < 0.1f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            velocidadRotacion * Time.fixedDeltaTime
        );
    }
}
