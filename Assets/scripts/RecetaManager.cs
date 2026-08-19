using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct Receta
{
    public string itemA;
    public string itemB;
    public string resultId;
    public Sprite resultSprite;

    [Header("Inspección 3D del Objeto Resultante")]
    public GameObject resultPrefab3D;

    [Header("Opciones de Consumo")]
    public bool destroyItemA;
    public bool destroyItemB;

    [Header("Recompensa / Mensaje Opcional")]
    [TextArea(2, 4)]
    public string descriptionMessage;
    public GameObject extraRewardPrefab;
}

public class RecetaManager : MonoBehaviour
{
    public static RecetaManager Instance;

    [Header("UI de Texto / Diálogo")]
    public TextMeshProUGUI dialogText;
    public GameObject dialogPanel;

    [Header("Configuración de Diálogo")]
    public bool autoHide = false;
    public float displayDuration = 3.5f;

    [Header("Efecto Máquina de Escribir")]
    [Tooltip("Tiempo de espera entre cada letra (menor = más rápido)")]
    public float velocidadEscritura = 0.04f;
    [Tooltip("AudioSource para reproducir el sonido (puede ser el mismo del panel)")]
    public AudioSource audioSourceDialogo;
    [Tooltip("Clip de sonido corto (un 'blip', 'tick' o tecla)")]
    public AudioClip sonidoTecleo;

    private Coroutine hideCoroutine;
    private Coroutine typingCoroutine; // Controla la corrutina de escritura

    [Header("Lista de Recetas")]
    public List<Receta> recetas = new List<Receta>();

    [Header("Referencias de Canvas / UI")]
    public GameObject canvasDialogo;

    private bool estaEscribiendo = false;
    private bool esperandoClicParaCerrar = false;
    private string mensajeActual = "";

    private void Update()
    {
        // Si el panel de diálogo está activo y el jugador hace clic izquierdo o presiona Espacio/Enter
        if (dialogPanel != null && dialogPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (estaEscribiendo)
                {
                    // 1. Clic MIENTRAS se escribe: Completa el texto de golpe
                    CompletarTextoInmediatamente();
                }
                else if (esperandoClicParaCerrar)
                {
                    // 2. Clic CUANDO ya se terminó de escribir: Cierra el diálogo de inmediato
                    CloseDialog();
                }
            }
        }
    }

    public void CompletarTextoInmediatamente()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        dialogText.text = mensajeActual;
        estaEscribiendo = false;
        esperandoClicParaCerrar = true;

        if (autoHide)
        {
            if (hideCoroutine != null) StopCoroutine(hideCoroutine);
            hideCoroutine = StartCoroutine(HideDialogAfterDelay(displayDuration));
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (canvasDialogo != null)
            {
                DontDestroyOnLoad(canvasDialogo);
            }

            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
        }
        else
        {
            if (canvasDialogo != null) Destroy(canvasDialogo);
            Destroy(gameObject);
        }
    }

    public void ShowDialog(string message)
    {
        if (dialogPanel == null || dialogText == null) return;

        // 1. Detener cualquier diálogo previo que se esté escribiendo u ocultando
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        dialogPanel.SetActive(true);
        dialogText.text = ""; // Limpiar el texto antes de empezar

        // 2. Iniciar el efecto de máquina de escribir
        typingCoroutine = StartCoroutine(EfectoMaquinaDeEscribir(message));
    }

    private IEnumerator EfectoMaquinaDeEscribir(string message)
    {
        dialogText.text = "";

        // Convertimos el mensaje en un arreglo de letras y lo recorremos uno por uno
        foreach (char letra in message.ToCharArray())
        {
            dialogText.text += letra;

            // Reproducimos el sonido de la letra (omitimos el sonido en los espacios en blanco)
            if (audioSourceDialogo != null && sonidoTecleo != null && letra != ' ')
            {
                // PlayOneShot permite que el sonido se superponga suavemente si se escribe muy rápido
                audioSourceDialogo.PlayOneShot(sonidoTecleo);
            }

            // Esperamos una fracción de segundo antes de la siguiente letra
            yield return new WaitForSeconds(velocidadEscritura);
        }

        // 3. Una vez que termina de escribir TODAS las letras, inicia el conteo para ocultarse
        if (autoHide)
        {
            hideCoroutine = StartCoroutine(HideDialogAfterDelay(displayDuration));
        }
    }

    public void CloseDialog()
    {
        //if (dialogPanel != null)
        //{
        //dialogPanel.SetActive(false);
        //}

        estaEscribiendo = false;
        esperandoClicParaCerrar = false;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    private IEnumerator HideDialogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseDialog();
    }

    public void TryCombine(objetoInteractuable item1, objetoInteractuable item2)
    {
        if (item1 == null || item2 == null) return;

        foreach (Receta recipe in recetas)
        {
            bool isDirectMatch = (item1.itemId == recipe.itemA && item2.itemId == recipe.itemB);
            bool isReverseMatch = (item1.itemId == recipe.itemB && item2.itemId == recipe.itemA);

            if (isDirectMatch || isReverseMatch)
            {
                objetoInteractuable objectA = isDirectMatch ? item1 : item2;
                objetoInteractuable objectB = isDirectMatch ? item2 : item1;

                PersistentObject pA = objectA.GetComponent<PersistentObject>();
                if (pA != null) pA.MarcarComoRecogido();

                PersistentObject pB = objectB.GetComponent<PersistentObject>();
                if (pB != null) pB.MarcarComoRecogido();

                if (!string.IsNullOrEmpty(recipe.resultId))
                {
                    objetoInteractuable objetoResultado = null;

                    if (!recipe.destroyItemA)
                    {
                        objectA.UpdateItem(recipe.resultId, recipe.resultSprite, recipe.resultPrefab3D);
                        DontDestroyOnLoad(objectA.gameObject);
                        objetoResultado = objectA;
                    }
                    else if (!recipe.destroyItemB)
                    {
                        objectB.UpdateItem(recipe.resultId, recipe.resultSprite, recipe.resultPrefab3D);
                        DontDestroyOnLoad(objectB.gameObject);
                        objetoResultado = objectB;
                    }

                    if (objetoResultado != null && barraInventario.Instance != null)
                    {
                        barraInventario.Instance.VerificarYDesbloquearBoton(objetoResultado);
                    }
                }

                if (!string.IsNullOrEmpty(recipe.descriptionMessage))
                {
                    ShowDialog(recipe.descriptionMessage);
                }

                if (recipe.extraRewardPrefab != null)
                {
                    GiveRewardToInventory(recipe.extraRewardPrefab);
                }

                if (recipe.destroyItemA && objectA != null) Destroy(objectA.gameObject);
                if (recipe.destroyItemB && objectB != null) Destroy(objectB.gameObject);

                return;
            }
        }

        Debug.Log("No existe ninguna receta que combine: " + item1.itemId + " con " + item2.itemId);
    }

    private void GiveRewardToInventory(GameObject rewardPrefab)
    {
        GameObject newItemObj = Instantiate(rewardPrefab);
        objetoInteractuable newItem = newItemObj.GetComponent<objetoInteractuable>();

        if (newItem != null && barraInventario.Instance != null)
        {
            Vector3 slotPosition = barraInventario.Instance.GetNextFreeSlot(newItem);
            newItem.transform.position = slotPosition;
            DontDestroyOnLoad(newItemObj);
        }
    }
}