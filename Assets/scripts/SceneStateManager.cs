using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance;

    // Lista de IDs de objetos que han sido recogidos/destruidos
    private HashSet<string> objetosRecogidos = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Registrar que un objeto fue tomado o destruido
    public void RegistrarObjetoRecogido(string objectID)
    {
        if (!string.IsNullOrEmpty(objectID) && !objetosRecogidos.Contains(objectID))
        {
            objetosRecogidos.Add(objectID);
        }
    }

    // Consultar si un objeto ya fue tomado previamente
    public bool EstaRecogido(string objectID)
    {
        return objetosRecogidos.Contains(objectID);
    }
}
