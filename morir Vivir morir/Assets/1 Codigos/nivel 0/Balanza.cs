using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balanza : MonoBehaviour
{
    [Header("Peso")]
    [Min(1)]
    public int pesoNecesario = 1;

    [Header("Información")]
    [SerializeField]
    private int pesoActual;

    [SerializeField]
    private bool activa;

    /*
     * Cada robot puede tener varios colliders.
     * Por eso guardamos los colliders de cada RobotBase.
     */
    private readonly Dictionary<RobotBase, HashSet<Collider>> robots =
        new Dictionary<RobotBase, HashSet<Collider>>();

    private void OnTriggerEnter(Collider other)
    {
        AgregarCollider(other);
    }

    private void OnTriggerStay(Collider other)
    {
        /*
         * Esto asegura que el robot sea detectado aunque
         * OnTriggerEnter no se haya registrado correctamente.
         */
        AgregarCollider(other);
    }

    private void OnTriggerExit(Collider other)
    {
        QuitarCollider(other);
    }

    private void FixedUpdate()
    {
        /*
         * Se revisa constantemente porque un clon puede
         * destruirse o desactivarse sin ejecutar OnTriggerExit.
         */
        LimpiarRobotsInvalidos();
        CalcularPeso();
    }

    private void AgregarCollider(Collider other)
    {
        if (other == null)
            return;

        RobotBase robot =
            other.GetComponentInParent<RobotBase>();

        if (robot == null)
            return;

        if (!robots.ContainsKey(robot))
        {
            robots.Add(
                robot,
                new HashSet<Collider>()
            );
        }

        robots[robot].Add(other);

        CalcularPeso();
    }

    private void QuitarCollider(Collider other)
    {
        if (other == null)
            return;

        RobotBase robot =
            other.GetComponentInParent<RobotBase>();

        if (robot == null)
            return;

        if (!robots.ContainsKey(robot))
            return;

        robots[robot].Remove(other);

        /*
         * Cuando ya no queda ningún collider del robot
         * dentro de la balanza, se elimina completamente.
         */
        if (robots[robot].Count == 0)
        {
            robots.Remove(robot);
        }

        CalcularPeso();
    }

    private void LimpiarRobotsInvalidos()
    {
        List<RobotBase> robotsParaEliminar =
            new List<RobotBase>();

        foreach (
            KeyValuePair<RobotBase, HashSet<Collider>> elemento
            in robots)
        {
            RobotBase robot = elemento.Key;

            /*
             * El clon fue destruido o desactivado.
             */
            if (robot == null ||
                !robot.gameObject.activeInHierarchy)
            {
                robotsParaEliminar.Add(robot);
                continue;
            }

            HashSet<Collider> colliders =
                elemento.Value;

            colliders.RemoveWhere(
                collider =>
                    collider == null ||
                    !collider.enabled ||
                    !collider.gameObject.activeInHierarchy
            );

            if (colliders.Count == 0)
            {
                robotsParaEliminar.Add(robot);
            }
        }

        foreach (RobotBase robot in robotsParaEliminar)
        {
            robots.Remove(robot);
        }
    }

    private void CalcularPeso()
    {
        int nuevoPeso = 0;

        foreach (
            KeyValuePair<RobotBase, HashSet<Collider>> elemento
            in robots)
        {
            RobotBase robot = elemento.Key;

            if (robot == null)
                continue;

            if (!robot.gameObject.activeInHierarchy)
                continue;

            if (elemento.Value.Count == 0)
                continue;

            nuevoPeso += robot.peso;
        }

        pesoActual = nuevoPeso;

        /*
         * Si ya no alcanza el peso necesario,
         * activa vuelve a ser false y las cajas regresan.
         */
        activa =
            pesoActual >= pesoNecesario;
    }

    public bool EstaActiva()
    {
        return activa;
    }

    public int ObtenerPesoActual()
    {
        return pesoActual;
    }

    private void OnDisable()
    {
        ReiniciarBalanza();
    }

    private void OnDestroy()
    {
        ReiniciarBalanza();
    }

    private void ReiniciarBalanza()
    {
        robots.Clear();

        pesoActual = 0;
        activa = false;
    }
}
