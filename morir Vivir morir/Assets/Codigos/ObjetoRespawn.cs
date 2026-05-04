using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoRespawn : MonoBehaviour
{
    [Header("Configuración")]
    public bool bloquearRotacion = true;
    public bool fijarAlTocarSuelo = true;

    [Header("Suelo")]
    public LayerMask capaSuelo;
    public float distanciaSuelo = 0.3f;

    private Rigidbody rb;
    private bool enSuelo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (bloquearRotacion)
        {
            rb.freezeRotation = true;
        }
    }

    private void Update()
    {
        VerificarSuelo();

        if (fijarAlTocarSuelo && enSuelo)
        {
            FijarObjeto();
        }
    }

    void VerificarSuelo()
    {
        enSuelo = Physics.Raycast(
            transform.position,
            Vector3.down,
            distanciaSuelo,
            capaSuelo
        );
    }

    void FijarObjeto()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;

        // Asegura que quede recto
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }

    // Esto se puede llamar cuando lo agarrás otra vez
    public void ActivarFisicas()
    {
        rb.isKinematic = false;
    }
}
