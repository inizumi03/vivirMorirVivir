using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguirObjetivo : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 offsetLocal;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        transform.position = objetivo.position + objetivo.TransformDirection(offsetLocal);
        transform.rotation = objetivo.rotation;
    }
}
