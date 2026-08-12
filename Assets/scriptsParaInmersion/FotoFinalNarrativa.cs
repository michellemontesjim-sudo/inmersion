using System.Collections;
using UnityEngine;
using DG.Tweening; // Importante para animar la foto de forma suave

public class FotoFinalNarrativa : MonoBehaviour
{
    [Header("Animación de la Foto")]
    [Tooltip("Escala a la que aumentará la foto al hacer clic (ej. 1.5 o 2.0)")]
    public Vector3 escalaFinal = new Vector3(1.5f, 1.5f, 1f);
    [Tooltip("Duración de la animación de zoom")]
    public float duracionAnimacion = 0.8f;
    [Tooltip("Posición central en pantalla a la que se moverá la foto (opcional)")]
    public Vector3 posicionCentral = new Vector3(0f, 0f, -2f);

    [Header("Diálogo Narrativo")]
    [TextArea(2, 5)]
    public string mensajeFoto = "Esta foto... lo aclara todo. Por fin entiendo lo que pasó aquel día.";

    [Header("Escena Final")]
    [Tooltip("Nombre EXACTO de la escena final a la que cambiará")]
    public string nombreEscenaFinal = "EscenaFinal";
    [Tooltip("Tiempo que espera en negro antes de cargar el final")]
    public float esperaFadeOut = 1.0f;

    private bool interactuado = false;
    private Vector3 escalaInicial;
    private Vector3 posicionInicial;

    private void Start()
    {
        escalaInicial = transform.localScale;
        posicionInicial = transform.position;
    }

    private void OnMouseDown()
    {
        // Evitamos que el jugador haga clic múltiples veces
        if (interactuado) return;
        interactuado = true;

        StartCoroutine(SecuenciaFotoYFinal());
    }

    private IEnumerator SecuenciaFotoYFinal()
    {
        // 1. AGRANDAR Y CENTRAR LA FOTO CON DOTWEEN
        Sequence secuenciaFoto = DOTween.Sequence();
        secuenciaFoto.Join(transform.DOScale(escalaFinal, duracionAnimacion).SetEase(Ease.OutBack));
        secuenciaFoto.Join(transform.DOMove(posicionCentral, duracionAnimacion).SetEase(Ease.OutQuad));

        // Esperar a que la animación de la foto termine
        yield return secuenciaFoto.WaitForCompletion();

        // 2. MOSTRAR EL DIÁLOGO (Usando RecetaManager y su máquina de escribir)
        if (RecetaManager.Instance != null && !string.IsNullOrEmpty(mensajeFoto))
        {
            RecetaManager.Instance.ShowDialog(mensajeFoto);
        }

        // 3. ESPERAR A QUE EL JUGADOR LEA Y HAGA CLIC PARA CONTINUAR
        // Esperamos a que vuelva a hacer clic con el ratón o presione Espacio/Enter
        yield return new WaitForSeconds(0.5f); // Breve margen para no saltárselo por error
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return));

        // Ocultar el diálogo si estaba activo
        if (RecetaManager.Instance != null)
        {
            RecetaManager.Instance.CloseDialog();
        }

        // 4. FADE OUT Y CAMBIO A LA ESCENA FINAL
        if (!string.IsNullOrEmpty(nombreEscenaFinal))
        {
            if (SceneChanger.Instance != null)
            {
                // Ejecuta el Fade Out, reproduce pasos/sonido si los tiene y carga la escena final
                SceneChanger.Instance.CambiarAEscena(nombreEscenaFinal);
            }
            else
            {
                // Respaldo directo si no está el SceneChanger
                UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaFinal);
            }
        }
    }
}