using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class objetoInteractuable : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    public string itemId;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Vector3 initialPosition;
    private bool isDragging = false;

    // Variables para detectar si es Clic o Arrastre
    private Vector3 clickStartPosition;
    private float dragThreshold = 0.2f; // Distancia mínima para considerar que se está arrastrando

    [Header("Inspección 3D")]
    [Tooltip("Prefab del modelo 3D detallado que se mostrará en pantalla grande")]
    public GameObject prefabModelo3DInspeccion;
    public GameObject newPrefab3D;
    [Header("Sonido de Combinación")]
    public AudioClip sonidoCombinacion;
    private AudioSource audioSource;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        AdjustColliderSize();
    }

    public void UpdateItem(string newId, Sprite newSprite, GameObject newPrefab3D = null)
    {
        itemId = newId;

        if (newSprite != null)
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            spriteRenderer.sprite = newSprite;
            AdjustColliderSize();

            if (boxCollider != null) boxCollider.enabled = true;
        }

        if (newPrefab3D != null)
        {
            prefabModelo3DInspeccion = newPrefab3D;
        }
    }

    private void AdjustColliderSize()
    {
        if (spriteRenderer != null && spriteRenderer.sprite != null && boxCollider != null)
        {
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            boxCollider.offset = spriteRenderer.sprite.bounds.center;
        }
    }

    void OnMouseDown()
    {
        // Guardamos la posición inicial respetando el plano frontal Z = -2
        initialPosition = new Vector3(transform.position.x, transform.position.y, -2f);
        transform.position = initialPosition;

        clickStartPosition = Input.mousePosition;
        isDragging = false;
    }

    void OnMouseDrag()
    {
        // Solo activamos el arrastre si el ratón se movió más allá del umbral
        if (Vector3.Distance(clickStartPosition, Input.mousePosition) > dragThreshold)
        {
            isDragging = true;
        }

        if (isDragging)
        {
            Camera currentCam = Camera.main;

            if (currentCam != null)
            {
                Vector3 mousePos = currentCam.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = -2f;
                transform.position = mousePos;
            }
        }
    }

    void OnMouseUp()
    {
        // CASO 1: Si NO se arrastró, fue un CLIC SIMPLE -> Abrir Inspección 3D
        if (!isDragging)
        {
            AbrirInspeccion();
            return;
        }

        // CASO 2: Si SÍ se arrastró -> Ejecutar la lógica de combinación o guardado
        isDragging = false;
        CheckCombination();
    }

    private void AbrirInspeccion()
    {
        // Verificamos si el ítem ya está guardado en el inventario
        bool estaEnInventario = false;

        if (barraInventario.Instance != null)
        {
            estaEnInventario = barraInventario.Instance.TieneItem(itemId);
        }

        // Si está en el inventario (o si quieres permitir inspeccionarlo directamente)
        if (estaEnInventario || transform.parent != null)
        {
            if (InspeccionObjeto3D.Instance != null && prefabModelo3DInspeccion != null)
            {
                InspeccionObjeto3D.Instance.AbrirInspeccion(prefabModelo3DInspeccion, itemId);
            }
            else
            {
                Debug.LogWarning("InspeccionObjeto3D.Instance o el prefabModelo3DInspeccion no están asignados.");
            }
        }
    }

    void CheckCombination()
    {
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(transform.position);

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.gameObject != gameObject)
            {
                // 1. Detección del Inventario
                barraInventario inventory = hit.GetComponent<barraInventario>();
                if (inventory != null)
                {
                    Vector3 slotPos = inventory.GetNextFreeSlot(this);
                    slotPos.z = -2f;
                    transform.position = slotPos;

                    PersistentObject persistent = GetComponent<PersistentObject>();
                    if (persistent != null)
                    {
                        persistent.MarcarComoRecogido();
                    }

                    DontDestroyOnLoad(gameObject);
                    return;
                }

                // 2. Detección de Combinación con otro objeto
                objetoInteractuable targetItem = hit.GetComponent<objetoInteractuable>();
                if (targetItem != null)
                {
                    if (barraInventario.Instance != null)
                    {
                        barraInventario.Instance.RemoveFromInventory(this);
                    }
                    if (audioSource != null && sonidoCombinacion != null)
                    {
                        AudioSource.PlayClipAtPoint(sonidoCombinacion, transform.position);
                    }

                    RecetaManager.Instance.TryCombine(this, targetItem);
                    return;
                }
            }
        }

        Debug.Log("No se encontró ningún otro objeto válido debajo.");
        transform.position = initialPosition;
    }
}