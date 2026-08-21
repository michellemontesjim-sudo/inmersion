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

    [Header("Efecto máquina de escribir")]
    public float velocidadEscritura = 0.04f;
    public AudioSource audioSource;
    public AudioClip sonidoTecleo;

    // Guardamos los textos originales
    private string textoTitulo;
    private string textoInfo;
    private string textoInfo2;

    void Start()
    {
        // Guardamos el contenido original
        textoTitulo = titulo.text;
        textoInfo = info.text;
        textoInfo2 = info2.text;

        // Ocultamos los textos al comenzar
        titulo.text = "";
        info.text = "";
        info2.text = "";

        // Ocultamos el botón
        botonIniciar.SetActive(false);

        StartCoroutine(IniciarCinematica());
    }

    IEnumerator IniciarCinematica()
    {
        // ==========================
        // TÍTULO
        // ==========================
        yield return StartCoroutine(
            EscribirTexto(titulo, textoTitulo)
        );

        yield return new WaitForSeconds(3f);

        titulo.text = "";

        // ==========================
        // PRIMER TEXTO
        // ==========================
        yield return StartCoroutine(
            EscribirTexto(info, textoInfo)
        );

        yield return new WaitForSeconds(3f);

        info.text = "";


        // ==========================
        // SEGUNDO TEXTO
        // ==========================
        yield return StartCoroutine(
            EscribirTexto(info2, textoInfo2)
        );

        yield return new WaitForSeconds(3f);

        info2.text = "";

        // ==========================
        // BOTÓN
        // ==========================
        botonIniciar.SetActive(true);
    }

    // ==========================
    // EFECTO MÁQUINA DE ESCRIBIR
    // ==========================
    IEnumerator EscribirTexto(Text texto, string contenido)
    {
        texto.text = "";

        foreach (char letra in contenido.ToCharArray())
        {
            texto.text += letra;

            // Sonido por cada letra
            if (audioSource != null &&
                sonidoTecleo != null &&
                letra != ' ')
            {
                audioSource.PlayOneShot(sonidoTecleo);
            }

            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    // ==========================
    // BOTÓN INICIAR
    // ==========================
    public void IniciarJuego()
    {
        SceneManager.LoadScene("habitacion");
    }
}