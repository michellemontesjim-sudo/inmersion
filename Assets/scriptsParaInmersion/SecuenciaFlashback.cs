using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SecuenciaFlashback : MonoBehaviour
{
    [Header("UI de Diálogo")]
    public TextMeshProUGUI textoUI;         // El componente de texto donde se escribirá
    public GameObject indicadorClicUI;       // (Opcional) Icono o flechita que parpadea indicando "haz clic para continuar"

    [Header("Lista de Frases del Recuerdo")]
    [TextArea(3, 5)]
    public List<string> frasesRecuerdo;     // Añade aquí todas las frases del recuerdo en orden

    [Header("Configuración de Escritura")]
    public float velocidadEscritura = 0.04f;
    public AudioSource audioSource;
    public AudioClip sonidoTecleo;

    [Header("Al Finalizar el Recuerdo (Opcional)")]
    public bool cambiarDeEscenaAlTerminar = false;
    public string nombreEscenaSiguiente;

    private int indiceFraseActual = 0;
    private bool estaEscribiendo = false;
    private bool esperandoClic = false;
    private Coroutine corrutinaEscritura;

    [Header("Ocultar Navegación (Opcional)")]
    [Tooltip("Panel o Canvas con los botones de cambio de habitación que deseas ocultar durante el flashback")]
    public GameObject panelNavegacionHabitaciones;

    private void Start()
    {
        // Desactivar los botones al comenzar el flashback
        if (panelNavegacionHabitaciones != null)
        {
            panelNavegacionHabitaciones.SetActive(false);
        }

        if (indicadorClicUI != null)
            indicadorClicUI.SetActive(false);

        if (frasesRecuerdo != null && frasesRecuerdo.Count > 0)
        {
            IniciarSiguienteFrase();
        }
        else
        {
            Debug.LogWarning("No hay frases agregadas en la lista de SecuenciaFlashback.");
        }
    }

    private void Update()
    {
        // Detecta el clic del ratón o la barra espaciadora para avanzar
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (estaEscribiendo)
            {
                // Si el jugador hace clic MIENTRAS se está escribiendo, completa el texto al instante
                CompletarTextoInmediatamente();
            }
            else if (esperandoClic)
            {
                // Si el texto ya se terminó de escribir, avanza a la siguiente frase
                esperandoClic = false;
                indiceFraseActual++;

                if (indiceFraseActual < frasesRecuerdo.Count)
                {
                    IniciarSiguienteFrase();
                }
                else
                {
                    FinalizarSecuencia();
                }
            }
        }
    }

    private void IniciarSiguienteFrase()
    {
        if (indicadorClicUI != null)
            indicadorClicUI.SetActive(false);

        corrutinaEscritura = StartCoroutine(EscribirFrase(frasesRecuerdo[indiceFraseActual]));
    }

    private IEnumerator EscribirFrase(string frase)
    {
        estaEscribiendo = true;
        textoUI.text = "";

        foreach (char letra in frase.ToCharArray())
        {
            textoUI.text += letra;

            if (audioSource != null && sonidoTecleo != null && letra != ' ')
            {
                audioSource.PlayOneShot(sonidoTecleo);
            }

            yield return new WaitForSeconds(velocidadEscritura);
        }

        estaEscribiendo = false;
        esperandoClic = true;

        // Muestra la flechita o icono que indica que el jugador puede hacer clic
        if (indicadorClicUI != null)
            indicadorClicUI.SetActive(true);
    }

    private void CompletarTextoInmediatamente()
    {
        if (corrutinaEscritura != null)
            StopCoroutine(corrutinaEscritura);

        textoUI.text = frasesRecuerdo[indiceFraseActual];
        estaEscribiendo = false;
        esperandoClic = true;

        if (indicadorClicUI != null)
            indicadorClicUI.SetActive(true);
    }

    private void FinalizarSecuencia()
    {
        if (cambiarDeEscenaAlTerminar && !string.IsNullOrEmpty(nombreEscenaSiguiente))
        {
            // Si tienes el SceneChanger con transición de fundido/pasos:
            if (SceneChanger.Instance != null)
            {
                SceneChanger.Instance.CambiarAEscena(nombreEscenaSiguiente);
            }
            else
            {
                SceneManager.LoadScene(nombreEscenaSiguiente);
            }
        }
        else
        {
            // Opcional: oculta el texto si la escena continúa en lugar de cambiar
            textoUI.text = "";
            if (indicadorClicUI != null)
                indicadorClicUI.SetActive(false);
        }
    }
}