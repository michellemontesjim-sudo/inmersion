using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    [Header("Referencias Visuales y de Audio")]
    public CanvasGroup panelNegro;      // El CanvasGroup de la imagen negra en UI
    public AudioSource audioSource;    // Componente AudioSource
    public AudioClip sonidoPasos;      // Sonido de pasos

    [Header("Configuración de Tiempos")]
    public float duracionFade = 0.8f;   // Tiempo de oscurecimiento y aclarado
    public float tiempoPasos = 1.0f;    // Cuánto tiempo duran los pasos a oscuras

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruye duplicados al recargar
        }
    }

    private void Start()
    {
        // Si hay un panel asignado, inicia la escena haciendo un Fade In
        if (panelNegro != null)
        {
            panelNegro.alpha = 1f;
            panelNegro.DOFade(0f, duracionFade);
        }
    }

    // Este método es el que llamarás desde los botones de UI o activadores
    public void CambiarAEscena(string nombreEscena)
    {
        Debug.Log("Intentando cargar la escena con transición: " + nombreEscena);
        StartCoroutine(SecuenciaCambioHabitacion(nombreEscena));
    }

    private IEnumerator SecuenciaCambioHabitacion(string nombreEscena)
    {
        // 1. Fade Out: Oscurecer la pantalla gradualmente
        if (panelNegro != null)
        {
            panelNegro.DOFade(1f, duracionFade);
        }

        // Esperar a que la pantalla quede negra
        yield return new WaitForSeconds(duracionFade);

        // 2. Reproducir el audio de pasos durante la oscuridad
        if (audioSource != null && sonidoPasos != null)
        {
            audioSource.PlayOneShot(sonidoPasos);
            yield return new WaitForSeconds(tiempoPasos);
        }

        // 3. Cargar la escena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nombreEscena);

        // Espera a que la escena se termine de cargar por completo en memoria
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 4. Fade In: Aclarar la pantalla suavemente al entrar a la nueva habitación
        if (panelNegro != null)
        {
            panelNegro.DOFade(0f, duracionFade);
        }
    }

    private void OnEnable()
    {
        // Escucha cada vez que Unity carga una escena nueva
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += AlCargarNuevaEscena;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= AlCargarNuevaEscena;
    }

    private void AlCargarNuevaEscena(UnityEngine.SceneManagement.Scene escena, UnityEngine.SceneManagement.LoadSceneMode modo)
    {
        // Busca si en la nueva escena existe un controlador de Focus
        EfectoFocusUI focus = FindObjectOfType<EfectoFocusUI>();
        if (focus != null)
        {
            focus.DesactivarDesenfoqueInmediato();
        }
    }
}