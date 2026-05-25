using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuedarQuieto : MonoBehaviour
{
    [Header("Suelo")]
    public LayerMask capaSuelo;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody>();
        }

        Congelar();
    }

    public void Congelar()
    {
        if (rb == null) return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Liberar()
    {
        if (rb == null) return;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (EstaEnLayer(collision.gameObject.layer))
        {
            Congelar();
        }
    }

    private bool EstaEnLayer(int layer)
    {
        return (capaSuelo.value &
            (1 << layer)) != 0;
    }
}
