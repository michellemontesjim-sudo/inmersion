using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    [Header("Efectos de Sonido")]
    public AudioSource audioSourcePasos;
    public AudioClip sonidoPasos;

    [Header("Transición Fade")]
    public CanvasGroup panelFade; // Panel negro para Fade In / Fade Out
    public float duracionFade = 0.5f;

    private bool realizandoTransicion = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Al iniciar el juego, aclara la pantalla y asegura que esté enfocada
        if (panelFade != null)
        {
            StartCoroutine(AclararPantallaInmediato());
        }

        QuitarDesenfoqueSiExiste();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarNuevaEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarNuevaEscena;
    }

    // Se ejecuta automáticamente cada vez que Unity termina de cargar una escena
    private void AlCargarNuevaEscena(Scene escena, LoadSceneMode modo)
    {
        // 1. Quitar la pantalla negra
        if (panelFade != null)
        {
            StartCoroutine(AclararPantallaInmediato());
        }

        // 2. 👈 RECUPERADO: Quitar el efecto borroso
        QuitarDesenfoqueSiExiste();
    }

    private void QuitarDesenfoqueSiExiste()
    {
        EfectoFocusUI focus = FindObjectOfType<EfectoFocusUI>();
        if (focus != null)
        {
            focus.DesactivarDesenfoqueInmediato();
        }
    }

    public void CambiarAEscena(string nombreEscenaDestino)
    {
        if (realizandoTransicion) return;

        // VALIDADOR ANTI-BUG: Si ya estamos en esa escena, no hace nada
        if (SceneManager.GetActiveScene().name == nombreEscenaDestino)
        {
            return;
        }

        StartCoroutine(SecuenciaCambioEscena(nombreEscenaDestino));
    }

    private IEnumerator SecuenciaCambioEscena(string nombreEscenaDestino)
    {
        realizandoTransicion = true;

        if (audioSourcePasos != null && sonidoPasos != null)
        {
            audioSourcePasos.PlayOneShot(sonidoPasos);
        }

        if (panelFade != null)
        {
            panelFade.blocksRaycasts = true;
            float tiempo = 0;
            while (tiempo < duracionFade)
            {
                tiempo += Time.deltaTime;
                panelFade.alpha = Mathf.Lerp(0f, 1f, tiempo / duracionFade);
                yield return null;
            }
            panelFade.alpha = 1f;
        }

        AsyncOperation carga = SceneManager.LoadSceneAsync(nombreEscenaDestino);
        while (!carga.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(AclararPantallaInmediato());

        realizandoTransicion = false;
    }

    private IEnumerator AclararPantallaInmediato()
    {
        if (panelFade == null) yield break;

        float tiempo = 0;
        float alphaInicial = panelFade.alpha;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            panelFade.alpha = Mathf.Lerp(alphaInicial, 0f, tiempo / duracionFade);
            yield return null;
        }

        panelFade.alpha = 0f;
        panelFade.blocksRaycasts = false;
    }
}