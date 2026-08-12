using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjetoInspeccionable : MonoBehaviour
{
    [Header("Identificador Único")]
    [Tooltip("ID único para este contenedor/grieta (Ej: Grieta_Habitacion_01)")]
    public string uniqueID;

    [Header("Mensaje de Diálogo")]
    [TextArea(2, 4)]
    public string mensajeInspeccion = "No parece haber nada interesante aquí...";

    [Header("Inspección 3D del Contenedor (Opcional)")]
    [Tooltip("Modelo 3D que se mostrará en pantalla grande al examinar este objeto/grieta en la escena")]
    public GameObject prefabModelo3DObjeto;

    [Header("Recompensa (Opcional)")]
    public GameObject prefabRecompensa; // Objeto que irá al inventario (ej: Llave)

    [Tooltip("Modelo 3D que tendrá el objeto entregado para cuando el jugador lo inspeccione en su inventario")]
    public GameObject prefabModelo3DRecompensa;

    public bool darRecompensaUnaSolaVez = true;

    [Header("Mensaje tras recoger la recompensa")]
    [TextArea(2, 4)]
    public string mensajeYaInspeccionado = "La grieta ya está vacía.";

    private bool yaFueRecompensado = false;

    private void Start()
    {
        // Al cargar o regresar a la escena, consultamos si este objeto ya entregó su recompensa
        if (!string.IsNullOrEmpty(uniqueID) && SceneStateManager.Instance != null)
        {
            if (SceneStateManager.Instance.EstaRecogido(uniqueID))
            {
                yaFueRecompensado = true;
            }
        }
    }

    private void OnMouseDown()
    {
        if (RecetaManager.Instance == null) return;

        // Caso 1: Ya entregó la recompensa anteriormente
        if (yaFueRecompensado)
        {
            if (!string.IsNullOrEmpty(mensajeYaInspeccionado))
            {
                RecetaManager.Instance.ShowDialog(mensajeYaInspeccionado);
            }
            return;
        }

        // Caso 2: Si este contenedor tiene un modelo 3D propio, lo abre en la UI de inspección
        if (prefabModelo3DObjeto != null && InspeccionObjeto3D.Instance != null)
        {
            InspeccionObjeto3D.Instance.AbrirInspeccion(prefabModelo3DObjeto, name);
        }

        // Mostrar el texto principal de la inspección
        if (!string.IsNullOrEmpty(mensajeInspeccion))
        {
            RecetaManager.Instance.ShowDialog(mensajeInspeccion);
        }

        // Caso 3: Tiene una recompensa para entregar
        if (prefabRecompensa != null)
        {
            EntregarRecompensa();

            if (darRecompensaUnaSolaVez)
            {
                yaFueRecompensado = true;

                // Registrar en el SceneStateManager que esta grieta ya entregó la recompensa
                if (!string.IsNullOrEmpty(uniqueID) && SceneStateManager.Instance != null)
                {
                    SceneStateManager.Instance.RegistrarObjetoRecogido(uniqueID);
                }
            }
        }
    }

    private void EntregarRecompensa()
    {
        GameObject nuevoObjetoObj = Instantiate(prefabRecompensa);
        objetoInteractuable nuevoObjeto = nuevoObjetoObj.GetComponent<objetoInteractuable>();

        if (nuevoObjeto != null)
        {
            // 👈 Si le asignaste un modelo 3D específico para la recompensa desde este script, se lo inyectamos al objeto
            if (prefabModelo3DRecompensa != null)
            {
                nuevoObjeto.prefabModelo3DInspeccion = prefabModelo3DRecompensa;
            }

            if (barraInventario.Instance != null)
            {
                Vector3 slotPosition = barraInventario.Instance.GetNextFreeSlot(nuevoObjeto);
                slotPosition.z = -2f;
                nuevoObjeto.transform.position = slotPosition;

                DontDestroyOnLoad(nuevoObjetoObj);
            }
        }
    }
}