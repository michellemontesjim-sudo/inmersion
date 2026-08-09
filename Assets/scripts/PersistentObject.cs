using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    [Header("ID Único del Objeto en el Escenario")]
    [Tooltip("Debe ser un identificador único para este objeto específico en la escena (Ej: Vela_Habitacion1)")]
    public string uniqueID;

    private bool fueRecogido = false;

    private void Start()
    {
        // SI YA FUE MARCADOCOMO RECOGIDO EN ESTE SCRIPT (está en el inventario), NO LO DESTRUIMOS
        if (fueRecogido) return;

        // Al cargar la escena, verificar si este objeto ya fue recogido anteriormente en el mundo
        if (SceneStateManager.Instance != null && SceneStateManager.Instance.EstaRecogido(uniqueID))
        {
            // Solo destruimos la copia del escenario que vuelve a aparecer al cargar la escena
            Destroy(gameObject);
        }
    }

    // Llama a esta función en el momento exacto en que el jugador recoge o guarda el objeto
    public void MarcarComoRecogido()
    {
        fueRecogido = true; // Marca que este clon en específico es el del inventario

        if (SceneStateManager.Instance != null)
        {
            SceneStateManager.Instance.RegistrarObjetoRecogido(uniqueID);
        }
    }
}