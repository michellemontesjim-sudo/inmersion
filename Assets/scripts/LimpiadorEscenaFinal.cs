using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimpiadorEscenaFinal : MonoBehaviour
{
    private void Start()
    {
        // Option A: Ocultar la barra de inventario si usas un Singleton
        if (barraInventario.Instance != null)
        {
            barraInventario.Instance.gameObject.SetActive(false);
        }

        // Option B: Destruir todos los objetos interactuables persistentes que se trajeron al inventario
        objetoInteractuable[] objetosEnEscena = FindObjectsOfType<objetoInteractuable>();
        foreach (objetoInteractuable obj in objetosEnEscena)
        {
            Destroy(obj.gameObject);
        }

        // 2. Buscar TODOS los Canvas de la escena (incluidos los persistentes de DontDestroyOnLoad)
        Canvas[] todosLosCanvas = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in todosLosCanvas)
        {
            // Oculta cualquier Canvas que NO pertenezca a la Escena Final actual
            if (canvas.gameObject.scene.name == "DontDestroyOnLoad")
            {
                canvas.gameObject.SetActive(false);
            }
        }


    }
}
