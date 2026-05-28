using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tstmando : MonoBehaviour
{
    private void Update()
    {
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.JoystickButton0 + i)))
            {
                Debug.Log("Boton mando detectado: " + i);
            }
        }
    }
}
