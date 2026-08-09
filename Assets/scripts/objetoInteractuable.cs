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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        AdjustColliderSize();
    }

    public void UpdateItem(string newId, Sprite newSprite)
    {
        itemId = newId;

        if (newSprite != null)
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            spriteRenderer.sprite = newSprite;
            AdjustColliderSize();

            // Nos aseguramos de que el collider quede activo tras cambiar el sprite
            if (boxCollider != null) boxCollider.enabled = true;
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
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Camera currentCam = Camera.main;

            if (currentCam != null)
            {
                Vector3 mousePos = currentCam.ScreenToWorldPoint(Input.mousePosition);
                // FORZAMOS Z = -2f para que el ítem vuele POR ENCIMA del escenario y UI al arrastrar
                mousePos.z = -2f;
                transform.position = mousePos;
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        CheckCombination();
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
                    // Aseguramos que el slot conserve el plano frontal (-2)
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

                    RecetaManager.Instance.TryCombine(this, targetItem);
                    return;
                }
            }
        }

        Debug.Log("No se encontró ningún otro objeto válido debajo.");
        // Si no se soltó en un lugar válido, regresa a su casilla manteniendo la Z correcta
        transform.position = initialPosition;
    }
}