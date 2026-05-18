using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvMirar : MonoBehaviour
{

    private Camera camara;

    private void Start()
    {
        camara = Camera.main;
    }

    private void LateUpdate()
    {
        if (camara == null) return;

        transform.LookAt(transform.position + camara.transform.forward);
    }
}
