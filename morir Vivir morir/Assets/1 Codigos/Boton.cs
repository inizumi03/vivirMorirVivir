using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boton : MonoBehaviour
{
    [Header("Detección")]
    public string tagJugador = "Player";
    public string tagObjeto = "Agarrable";

    [Header("Controlador")]
    public PuertaPorBotones puerta;

    [Header("Funcionamiento")]
    [Tooltip("Si está activado, el botón solo funciona mientras haya algo encima.")]
    public bool botonMantenido = false;

    [Tooltip("Si está activado, el botón queda encendido para siempre después de usarse.")]
    public bool usarSoloUnaVez = true;

    [Header("Visual")]
    public Renderer rendererBoton;
    public Material materialNormal;
    public Material materialActivado;

    [Header("Información")]
    [SerializeField]
    private bool activado = false;

    /*
     * Guarda todos los colliders válidos que están encima.
     * Esto evita errores si el jugador o un clon tienen
     * más de un collider.
     */
    private readonly HashSet<Collider> objetosEncima =
        new HashSet<Collider>();

    private void Start()
    {
        CambiarMaterial(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PuedeActivar(other.gameObject))
            return;

        objetosEncima.Add(other);

        /*
         * Si ya está activado, no volvemos
         * a sumar el botón en la puerta.
         */
        if (activado)
            return;

        /*
         * En el modo normal, un botón de un solo uso
         * no puede volver a activarse.
         */
        if (!botonMantenido && activado && usarSoloUnaVez)
            return;

        ActivarBoton();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PuedeActivar(other.gameObject))
            return;

        objetosEncima.Remove(other);

        /*
         * En el funcionamiento normal no ocurre nada
         * cuando el jugador se baja del botón.
         */
        if (!botonMantenido)
            return;

        LimpiarCollidersInvalidos();

        /*
         * El botón mantenido se apaga solamente cuando
         * ya no queda ningún objeto válido encima.
         */
        if (objetosEncima.Count == 0)
        {
            DesactivarBoton();
        }
    }

    private void ActivarBoton()
    {
        if (activado)
            return;

        activado = true;

        CambiarMaterial(true);

        if (puerta != null)
        {
            puerta.SumarBoton();
        }

        Debug.Log("BOTÓN ACTIVADO");
    }

    private void DesactivarBoton()
    {
        if (!activado)
            return;

        activado = false;

        CambiarMaterial(false);

        if (puerta != null)
        {
            puerta.RestarBoton();
        }

        Debug.Log("BOTÓN DESACTIVADO");
    }

    private bool PuedeActivar(GameObject obj)
    {
        if (obj == null)
            return false;

        return obj.CompareTag(tagJugador) ||
               obj.CompareTag(tagObjeto);
    }

    private void CambiarMaterial(bool estaActivado)
    {
        if (rendererBoton == null)
            return;

        if (estaActivado)
        {
            if (materialActivado != null)
            {
                rendererBoton.material = materialActivado;
            }
        }
        else
        {
            if (materialNormal != null)
            {
                rendererBoton.material = materialNormal;
            }
        }
    }

    private void LimpiarCollidersInvalidos()
    {
        objetosEncima.RemoveWhere(
            collider =>
                collider == null ||
                !collider.enabled ||
                !collider.gameObject.activeInHierarchy
        );
    }

    private void OnDisable()
    {
        objetosEncima.Clear();

        /*
         * Si se desactiva el botón mientras estaba
         * funcionando en modo mantenido, se lo resta
         * de la puerta.
         */
        if (botonMantenido && activado)
        {
            DesactivarBoton();
        }
    }

    public bool EstaActivado()
    {
        return activado;
    }
}
