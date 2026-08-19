using System.Collections;
using UnityEngine;

public class DialogoEntradaEscena : MonoBehaviour
{
    [Header("Identificador Único")]
    [Tooltip("ID único para recordar que el diálogo de esta habitación ya se mostró (Ej: Intro_Habitacion_01)")]
    public string uniqueIDDialogo;

    [Header("Contenido del Diálogo")]
    [TextArea(2, 5)]
    [Tooltip("Texto que dirá el personaje o narrador al entrar por primera vez")]
    public string mensajeEntrada = "Esta habitación se siente muy fría... Debería buscar una forma de salir.";

    [Header("Tiempos")]
    [Tooltip("Segundos de espera antes de mostrar el diálogo (ideal para esperar a que termine el Fade In de la escena)")]
    public float delayInicial = 1.2f;

    private void Start()
    {
        // 1. Verificar si este diálogo ya fue mostrado anteriormente mediante el SceneStateManager
        if (!string.IsNullOrEmpty(uniqueIDDialogo) && SceneStateManager.Instance != null)
        {
            if (SceneStateManager.Instance.EstaRecogido(uniqueIDDialogo))
            {
                // Si ya fue mostrado previamente, no hace nada y termina
                return;
            }
        }

        // 2. Si es la primera vez que se entra a este escenario, inicia el proceso
        StartCoroutine(MostrarDialogoEntrada());
    }

    private IEnumerator MostrarDialogoEntrada()
    {
        // Guardar de inmediato en el SceneStateManager que este diálogo ya fue consumido
        if (!string.IsNullOrEmpty(uniqueIDDialogo) && SceneStateManager.Instance != null)
        {
            SceneStateManager.Instance.RegistrarObjetoRecogido(uniqueIDDialogo);
        }

        // Espera el tiempo configurado (permite que la pantalla termine el Fade In)
        yield return new WaitForSeconds(delayInicial);

        // Muestra el diálogo en pantalla con tu RecetaManager
        if (RecetaManager.Instance != null && !string.IsNullOrEmpty(mensajeEntrada))
        {
            RecetaManager.Instance.ShowDialog(mensajeEntrada);
        }
    }
}