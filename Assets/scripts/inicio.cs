using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CinematicIntro : MonoBehaviour
{
    public Text titulo;
    public Text info;
    public Text info2;

    public GameObject botonIniciar;

    public float velocidadFade = 1f;
    public float tiempoVisible = 3f;

    void Start()
    {
        // Comenzamos con los tres textos invisibles
        PonerAlpha(titulo, 0);
        PonerAlpha(info, 0);
        PonerAlpha(info2, 0);

        // Comenzamos con el botón oculto
        botonIniciar.SetActive(false);

        StartCoroutine(IniciarCinematica());
    }

    IEnumerator IniciarCinematica()
    {
        // APARECE EL TÍTULO
        yield return StartCoroutine(FadeIn(titulo));

        yield return new WaitForSeconds(tiempoVisible);

        // DESAPARECE EL TÍTULO
        yield return StartCoroutine(FadeOut(titulo));

        // APARECE EL PRIMER TEXTO
        yield return StartCoroutine(FadeIn(info));

        yield return new WaitForSeconds(tiempoVisible);

        // DESAPARECE EL PRIMER TEXTO
        yield return StartCoroutine(FadeOut(info));

        // APARECE EL SEGUNDO TEXTO
        yield return StartCoroutine(FadeIn(info2));

        yield return new WaitForSeconds(tiempoVisible);

        // DESAPARECE EL SEGUNDO TEXTO
        yield return StartCoroutine(FadeOut(info2));

        // APARECE EL BOTÓN INICIAR
        botonIniciar.SetActive(true);
    }

    IEnumerator FadeIn(Text texto)
    {
        Color color = texto.color;
        color.a = 0;
        texto.color = color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime * velocidadFade;
            texto.color = color;
            yield return null;
        }

        color.a = 1;
        texto.color = color;
    }

    IEnumerator FadeOut(Text texto)
    {
        Color color = texto.color;

        while (color.a > 0)
        {
            color.a -= Time.deltaTime * velocidadFade;
            texto.color = color;
            yield return null;
        }

        color.a = 0;
        texto.color = color;
    }

    void PonerAlpha(Text texto, float alpha)
    {
        Color color = texto.color;
        color.a = alpha;
        texto.color = color;
    }
    public void IniciarJuego()
    {
        SceneManager.LoadScene("habitacion");
    }
}