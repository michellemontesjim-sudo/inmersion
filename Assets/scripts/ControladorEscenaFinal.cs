using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ControladorEscenaFinal : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("El componente TextMeshProUGUI donde se escribirá la reflexión.")]
    public TextMeshProUGUI textoReflexionUI;

    [Header("Configuración del Mensaje")]
    [TextArea(4, 8)]
    public string mensajeReflexion = "Aquí escribes el mensaje de reflexión final sobre tu juego...";

    [Header("Efecto de Escritura")]
    [Tooltip("Si está marcado, el texto aparecerá letra por letra.")]
    public bool usarEfectoEscritura = true;
    public float velocidadEscritura = 0.04f;

    [Header("Opciones Finales (Opcional)")]
    public GameObject botonReiniciar; // Por si quieres permitir volver a jugar

    private void Start()
    {
        if (botonReiniciar != null)
            botonReiniciar.SetActive(false);

        if (textoReflexionUI != null)
        {
            if (usarEfectoEscritura)
            {
                textoReflexionUI.text = "";
                StartCoroutine(EscribirTextoLetraPorLetra());
            }
            else
            {
                textoReflexionUI.text = mensajeReflexion;
                if (botonReiniciar != null) botonReiniciar.SetActive(true);
            }
        }
    }

    private IEnumerator EscribirTextoLetraPorLetra()
    {
        foreach (char letra in mensajeReflexion.ToCharArray())
        {
            textoReflexionUI.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        // Una vez terminado el texto, mostramos el botón para reiniciar si existe
        if (botonReiniciar != null)
        {
            botonReiniciar.SetActive(true);
        }
    }

    // Asignar al botón 'Volver a Jugar' o 'Menú Principal' en el Canvas UI
    public void ReiniciarJuego()
    {
        // Si usas un Gestor de Estado o PersistentObjects, considera reiniciarlos aquí
        SceneManager.LoadScene(0); // Carga la escena inicial (Build Index 0)
    }
}