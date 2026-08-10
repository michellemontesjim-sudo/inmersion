using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // LIBRERÍA BUILT-IN
using DG.Tweening;

public class EfectoFocusUI : MonoBehaviour
{
    public PostProcessVolume volumePostProcessing;

    [Header("Distancias de Enfoque")]
    public float distanciaFondoClaro = 10f;
    public float distanciaDesenfocada = 0.1f;
    public float duracionTransicion = 0.8f;

    private DepthOfField depthOfField;

    private void Start()
    {
        if (volumePostProcessing != null && volumePostProcessing.profile != null)
        {
            volumePostProcessing.profile.TryGetSettings(out depthOfField);
        }

        // 👈 RECONEXIÓN Y RESTABLECIMIENTO:
        // Asegura que al entrar a la escena el fondo comience NÍTIDO
        DesactivarDesenfoqueInmediato();
    }

    public void ActivarDesenfoque()
    {
        if (depthOfField != null)
        {
            depthOfField.focusDistance.overrideState = true;
            DOTween.To(() => depthOfField.focusDistance.value,
                       x => depthOfField.focusDistance.value = x,
                       distanciaDesenfocada,
                       duracionTransicion);
        }
    }

    public void DesactivarDesenfoque()
    {
        if (depthOfField != null)
        {
            DOTween.To(() => depthOfField.focusDistance.value,
                       x => depthOfField.focusDistance.value = x,
                       distanciaFondoClaro,
                       duracionTransicion);
        }
    }

    // 👈 Restablece instantáneamente el enfoque al cargar la escena
    public void DesactivarDesenfoqueInmediato()
    {
        if (depthOfField != null)
        {
            depthOfField.focusDistance.overrideState = true;
            depthOfField.focusDistance.value = distanciaFondoClaro;
        }
    }
}