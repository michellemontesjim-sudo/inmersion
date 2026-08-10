using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening; // Usamos DOTween para el fade suave

public class TransicionEscenas : MonoBehaviour
{
    [Header("UI y Audio")]
    public CanvasGroup panelNegro;      // Panel negro que cubre la pantalla
    public AudioSource audioSource;    // Reproductor de audio
    public AudioClip sonidoPasos;      // Efecto de sonido de pasos

    [Header("Tiempos de Transición")]
    public float duracionFade = 0.8f;   // Tiempo que tarda en oscurecer/aclarar
    public float tiempoPasos = 1.2f;    // Tiempo que suenan los pasos a oscuras

    private void Start()
    {
        // Al iniciar la escena, nos aseguramos de que la pantalla se aclare (Fade In)
        if (panelNegro != null)
        {
            panelNegro.alpha = 1f; // Inicia totalmente negro
            panelNegro.DOFade(0f, duracionFade); // Transición suave a transparente
        }
    }

    // Llama a este método desde los botones de cambio de habitación
    public void CambiarDeHabitacion(string nombreEscena)
    {
        StartCoroutine(SecuenciaCambioHabitacion(nombreEscena));
    }

    private IEnumerator SecuenciaCambioHabitacion(string nombreEscena)
    {
        // 1. FADE OUT: La pantalla se oscurece gradualmente
        if (panelNegro != null)
        {
            panelNegro.DOFade(1f, duracionFade);
        }

        // Esperar a que el fade out termine completamente
        yield return new WaitForSeconds(duracionFade);

        // 2. SONIDO DE PASOS: Reproducir audio a oscuras
        if (audioSource != null && sonidoPasos != null)
        {
            audioSource.PlayOneShot(sonidoPasos);
            // Esperamos el tiempo que duran los pasos
            yield return new WaitForSeconds(tiempoPasos);
        }

        // 3. CAMBIO DE ESCENA: Cargar la siguiente habitación
        SceneManager.LoadScene(nombreEscena);
    }
}