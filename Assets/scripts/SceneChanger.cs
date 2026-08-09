using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public static SceneChanger Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que el SceneManager se destruya
        }
        else
        {
            Destroy(gameObject); // Destruye duplicados
        }
    }
    // Cambia a la escena mediante su nombre exacto
    public void CambiarAEscena(string nombreEscena)
    {
        Debug.Log("Intentando cargar la escena: " + nombreEscena);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
        //SceneManager.LoadScene(nombreEscena);
    }
}
