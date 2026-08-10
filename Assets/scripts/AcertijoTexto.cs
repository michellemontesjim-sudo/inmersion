using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class AcertijoTexto : MonoBehaviour
{
    [Header("Efectos de Audio")]
    [Tooltip("Audio que se reproducirá al abrir la interfaz del acertijo")]
    public AudioClip sonidoAbrirPanel;
    private AudioSource audioSource;

    [Header("Identificador para Persistencia")]
    [Tooltip("ID único para recordar si este acertijo ya fue resuelto (Ej: Acertijo_CajaFuerte_01)")]
    public string uniqueID;

    [Header("Configuración del Acertijo")]
    [TextArea(2, 4)]
    public string enunciadoAcertijo = "Sun digs its heel to taunt you, but after sunlit days, one thing stays the same...";

    [Tooltip("La respuesta correcta (No importa si se escribe en mayúsculas o minúsculas)")]
    public string respuestaCorrecta = "rises the moon";

    [Header("Referencias de la Interfaz UI")]
    public GameObject panelAcertijoUI;       // El Panel pop-up en el Canvas
    public TextMeshProUGUI textoEnunciado;   // Texto que muestra la pregunta
    public TMP_InputField inputRespuesta;    // El campo donde el usuario escribe
    public TextMeshProUGUI textoFeedback;     // Muestra "¡Correcto!" o "Incorrecto"

    [Header("Recompensa / Resultado al Resolver")]
    public GameObject prefabRecompensa;      // (Opcional) Objeto que va al inventario
    [TextArea(2, 4)]
    public string mensajeExito = "¡La caja fuerte se ha abierto!";

    [Header("Cambio a Escena Final")]
    [Tooltip("Marcar si al resolver este acertijo se debe pasar automáticamente a la escena final.")]
    public bool irAEscenaFinal = true;

    [Tooltip("Nombre EXACTO de la escena final en los Build Settings.")]
    public string nombreEscenaFinal = "EscenaFinal";

    [Tooltip("Segundos de espera mostrando el mensaje/recompensa antes de cambiar de escena.")]
    public float tiempoEsperaTransicion = 3.0f;

    private bool yaResuelto = false;

    public AcertijoInmersivo animadorCamara;

    [Header("Efectos Visuales")]
    public EfectoFocusUI controladorFocus;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (panelAcertijoUI != null)
        {
            panelAcertijoUI.SetActive(false);
        }

        if (!string.IsNullOrEmpty(uniqueID) && SceneStateManager.Instance != null)
        {
            if (SceneStateManager.Instance.EstaRecogido(uniqueID))
            {
                yaResuelto = true;
            }
        }
    }

    private void OnMouseDown()
    {
        if (yaResuelto)
        {
            if (RecetaManager.Instance != null)
            {
                RecetaManager.Instance.ShowDialog("Este acertijo ya fue resuelto.");
            }
            return;
        }

        AbrirPanelAcertijo();
    }

    // Método que se llama desde OnMouseDown o botones
    public void AbrirPanelAcertijo()
    {
        StartCoroutine(SecuenciaAbrirPanelConZoom());
    }

    private IEnumerator SecuenciaAbrirPanelConZoom()
    {
        // 1. Iniciar la animación de la cámara (Zoom / Acercamiento)
        if (animadorCamara != null)
        {
            animadorCamara.EnfocarObjeto();
        }

        if (controladorFocus != null)
        {
            controladorFocus.ActivarDesenfoque();
        }

        // 2. Esperar 1 segundo mientras se realiza el zoom de la cámara
        yield return new WaitForSeconds(1.0f);

        // 3. Abrir la interfaz UI del acertijo
        if (panelAcertijoUI != null)
        {
            panelAcertijoUI.SetActive(true);

            if (textoEnunciado != null)
                textoEnunciado.text = enunciadoAcertijo;

            if (inputRespuesta != null)
                inputRespuesta.text = "";

            if (textoFeedback != null)
                textoFeedback.text = "";

            // Reproducir sonido al abrir la UI
            if (sonidoAbrirPanel != null && audioSource != null)
            {
                audioSource.clip = sonidoAbrirPanel;
                audioSource.Play();
            }
        }
    }

    public void CerrarPanelAcertijo()
    {
        // Ocultar el panel UI inmediatamente
        if (panelAcertijoUI != null)
        {
            panelAcertijoUI.SetActive(false);
        }

        // Detener audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Regresar la cámara a su posición original
        if (animadorCamara != null)
        {
            animadorCamara.DesenfoqueObjeto();
        }

        if (controladorFocus != null)
        {
            controladorFocus.DesactivarDesenfoque();
        }
    }

    public void ValidarRespuesta()
    {
        if (yaResuelto || inputRespuesta == null) return;

        string intentoUsuario = inputRespuesta.text.Trim().ToLower();
        string respuestaEsperada = respuestaCorrecta.Trim().ToLower();

        if (intentoUsuario == respuestaEsperada)
        {
            yaResuelto = true;

            if (!string.IsNullOrEmpty(uniqueID) && SceneStateManager.Instance != null)
            {
                SceneStateManager.Instance.RegistrarObjetoRecogido(uniqueID);
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            CerrarPanelAcertijo();

            // 1. Muestra el mensaje de éxito en pantalla
            if (RecetaManager.Instance != null && !string.IsNullOrEmpty(mensajeExito))
            {
                RecetaManager.Instance.ShowDialog(mensajeExito);
            }

            // 2. Entrega el objeto al inventario
            if (prefabRecompensa != null)
            {
                EntregarRecompensa();
            }

            // 3. Inicia la corrutina que da tiempo a leer antes de cambiar de escena
            if (irAEscenaFinal && !string.IsNullOrEmpty(nombreEscenaFinal))
            {
                StartCoroutine(CargarEscenaFinalConDelay());
            }
        }
        else
        {
            if (textoFeedback != null)
            {
                textoFeedback.text = "Respuesta incorrecta. Inténtalo de nuevo.";
            }
        }
    }

    private void EntregarRecompensa()
    {
        GameObject nuevoObjetoObj = Instantiate(prefabRecompensa);
        objetoInteractuable nuevoObjeto = nuevoObjetoObj.GetComponent<objetoInteractuable>();

        if (nuevoObjeto != null && barraInventario.Instance != null)
        {
            Vector3 slotPosition = barraInventario.Instance.GetNextFreeSlot(nuevoObjeto);
            slotPosition.z = -2f;
            nuevoObjeto.transform.position = slotPosition;

            DontDestroyOnLoad(nuevoObjetoObj);
        }
    }

    private IEnumerator CargarEscenaFinalConDelay()
    {
        // Espera los segundos configurados para que el jugador lea el mensaje y vea la recompensa
        yield return new WaitForSeconds(tiempoEsperaTransicion);

        // Carga la escena final directamente (se limpiará la UI automáticamente)
        SceneManager.LoadScene(nombreEscenaFinal);
    }
}