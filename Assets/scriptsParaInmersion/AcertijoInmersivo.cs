using UnityEngine;
using DG.Tweening;

public class AcertijoInmersivo : MonoBehaviour
{
    public Transform puntoEnfoqueCamara; // Objeto vacío destino
    public Camera camaraPrincipal;

    [Tooltip("Zoom deseado para cámaras Ortográficas 2D (Valores menores = Más zoom)")]
    public float zoomOrthographic = 2.5f;

    private Vector3 posicionOriginalCamara;
    private float tamanoOriginalCamara;

    private void Start()
    {
        if (camaraPrincipal != null)
        {
            posicionOriginalCamara = camaraPrincipal.transform.position;
            tamanoOriginalCamara = camaraPrincipal.orthographicSize;
        }
    }

    public void EnfocarObjeto()
    {
        if (camaraPrincipal == null || puntoEnfoqueCamara == null) return;

        // 1. Mueve la posición de la cámara (X, Y, Z)
        camaraPrincipal.transform.DOMove(puntoEnfoqueCamara.position, 1.2f).SetEase(Ease.InOutQuad);

        // 2. Si la cámara es 2D (Ortográfica), también anima el Zoom
        if (camaraPrincipal.orthographic)
        {
            camaraPrincipal.DOOrthoSize(zoomOrthographic, 1.2f).SetEase(Ease.InOutQuad);
        }
    }

    public void DesenfoqueObjeto()
    {
        if (camaraPrincipal == null) return;

        // Regresa la posición original
        camaraPrincipal.transform.DOMove(posicionOriginalCamara, 1.2f).SetEase(Ease.InOutQuad);

        // Regresa el zoom original si es 2D
        if (camaraPrincipal.orthographic)
        {
            camaraPrincipal.DOOrthoSize(tamanoOriginalCamara, 1.2f).SetEase(Ease.InOutQuad);
        }
    }
}