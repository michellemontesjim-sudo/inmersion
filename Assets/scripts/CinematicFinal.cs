using UnityEngine;
using TMPro;
using System.Collections;

public class CinematicFinal : MonoBehaviour
{
    public TMP_Text texto;

    [Header("Efecto máquina de escribir")]
    public float velocidadEscritura = 0.04f;
    public AudioSource audioSource;
    public AudioClip sonidoTecleo;

    // Guardamos el texto original
    private string textoOriginal;

    void Start()
    {
        // Guardamos el contenido original
        textoOriginal = texto.text;

        // Ocultamos el texto al comenzar
        texto.text = "";

        StartCoroutine(IniciarCinematica());
    }

    IEnumerator IniciarCinematica()
    {
        // ==========================
        // TEXTO
        // ==========================
        yield return StartCoroutine(
            EscribirTexto(texto, textoOriginal)
        );

        yield return new WaitForSeconds(3f);

        texto.text = "";
    }

    // ==========================
    // EFECTO MÁQUINA DE ESCRIBIR
    // ==========================
    IEnumerator EscribirTexto(TMP_Text texto, string contenido)
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
}