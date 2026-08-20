using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BotonCambiarEscena : MonoBehaviour
{
    [Header("Configuración de la Escena")]
    [Tooltip("Nombre EXACTO de la escena de acertijo a la que se cambiará.")]
    public string nombreEscenaAcertijo;

    [Header("Persistencia del Botón")]
    [Tooltip("ID único para guardar el estado del botón en SceneStateManager")]
    public string uniqueIDDesbloqueo;

    [Header("Pieza de Acertijo Requerida")]
    [Tooltip("ID del objeto en el inventario que activa este botón")]
    public string itemIdClave = "llave_acertijo";

    [Header("Transición y Post-Processing")]
    [Tooltip("Referencia opcional al controlador del efecto Focus/Desenfoque")]
    public EfectoFocusUI controladorFocus;

    private bool cambiandoDeEscena = false;

    private void Start()
    {
        // 1. Verificar si ya fue guardado en la persistencia de escena
        if (!string.IsNullOrEmpty(uniqueIDDesbloqueo) && SceneStateManager.Instance != null)
        {
            if (SceneStateManager.Instance.EstaRecogido(uniqueIDDesbloqueo))
            {
                MostrarBoton();
                return;
            }
        }

        // 2. Verificar si el jugador ya tiene el objeto clave en el inventario al cargar la escena
        if (barraInventario.Instance != null && barraInventario.Instance.TieneItem(itemIdClave))
        {
            MostrarBoton();
            return;
        }

        // 3. De lo contrario, iniciar oculto
        gameObject.SetActive(false);
    }

    public bool TieneElObjetoEnInventario()
    {
        if (barraInventario.Instance != null)
        {
            return barraInventario.Instance.TieneItem(itemIdClave);
        }
        return false;
    }

    public void MostrarBoton()
    {
        gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(uniqueIDDesbloqueo) && SceneStateManager.Instance != null)
        {
            SceneStateManager.Instance.RegistrarObjetoRecogido(uniqueIDDesbloqueo);
        }
    }

    // Asignado al evento OnClick() del Botón
    public void IrAEscenaAcertijo()
    {
        if (cambiandoDeEscena) return;

        StartCoroutine(SecuenciaCambioConFadeYPasos());
    }

    private IEnumerator SecuenciaCambioConFadeYPasos()
    {
        cambiandoDeEscena = true;

        // 1. Activar desenfoque temporal si existe el controlador
        if (controladorFocus != null)
        {
            controladorFocus.ActivarDesenfoque();
        }

        // 2. Feedback de pulsación en el botón
        transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.3f);

        // 3. Esperar un instante breve para que se aprecie la animación del botón
        yield return new WaitForSeconds(0.3f);

        // 4. Limpiar/desactivar el desenfoque antes del Fade para no dejarlo grabado
        if (controladorFocus != null)
        {
            controladorFocus.DesactivarDesenfoqueInmediato();
        }

        // 5. Utilizar el SceneChanger para ejecutar el Fade Out, sonido de pasos y Fade In
        if (!string.IsNullOrEmpty(nombreEscenaAcertijo))
        {
            if (SceneChanger.Instance != null)
            {
                SceneChanger.Instance.CambiarAEscena(nombreEscenaAcertijo);
            }
            else
            {
                // Respaldo por si no se encuentra SceneChanger en la escena
                UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaAcertijo);
            }
        }
        else
        {
            Debug.LogWarning("No se asignó el nombre de la escena en el Inspector.");
            cambiandoDeEscena = false;
        }
    }
}