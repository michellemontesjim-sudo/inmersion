using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FotoFinalNarrativa : MonoBehaviour
{
    [Header("Animación de la Foto")]
    [Tooltip("Escala a la que aumentará la foto al hacer clic")]
    public Vector3 escalaFinal = new Vector3(1.5f, 1.5f, 1f);
    [Tooltip("Duración de la animación de zoom")]
    public float duracionAnimacion = 0.8f;
    [Tooltip("Posición central en pantalla a la que se moverá la foto")]
    public Vector3 posicionCentral = new Vector3(0f, 0f, -2f);

    [Header("Diálogo Narrativo")]
    [TextArea(2, 5)]
    public string mensajeFoto = "Esta foto... lo aclara todo. Por fin entiendo lo que pasó aquel día.";

    [Header("Escena Final")]
    [Tooltip("Nombre EXACTO de la escena final a la que cambiará")]
    public string nombreEscenaFinal = "EscenaFinal";

    private bool interactuado = false;

    private void OnMouseDown()
    {
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

        // SOLUCIÓN 1: Reemplazamos WaitForCompletion por WaitForSeconds. 
        // A veces WaitForCompletion bugea las corrutinas en ciertas versiones de DOTween.
        yield return new WaitForSeconds(duracionAnimacion + 0.2f);

        // 2. MOSTRAR EL DIÁLOGO
        if (RecetaManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(mensajeFoto))
            {
                RecetaManager.Instance.ShowDialog(mensajeFoto);
            }
        }
        else
        {
            // SOLUCIÓN 2: Aviso por consola si falta el Gestor
            Debug.LogError("¡ATENCIÓN! No se encontró el RecetaManager en esta escena. El texto no puede mostrarse.");
        }

        // 3. ESPERAR A QUE EL JUGADOR LEA Y HAGA CLIC PARA CONTINUAR
        // Damos 1 segundo de gracia para que no lo cierre por accidente al hacer doble clic
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return));

        // Ocultar el diálogo
        if (RecetaManager.Instance != null)
        {
            RecetaManager.Instance.CloseDialog();
        }

        // 4. FADE OUT Y CAMBIO A LA ESCENA FINAL
        if (!string.IsNullOrEmpty(nombreEscenaFinal))
        {
            if (SceneChanger.Instance != null)
            {
                SceneChanger.Instance.CambiarAEscena(nombreEscenaFinal);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaFinal);
            }
        }
    }
}