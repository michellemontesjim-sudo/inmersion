using UnityEngine;
using TMPro;

public class InspeccionObjeto3D : MonoBehaviour
{
    public static InspeccionObjeto3D Instance;

    [Header("Referencias UI")]
    public GameObject panelInspeccionUI;
    public TextMeshProUGUI textoNombreUI;
    public GameObject canvasInspeccion;

    [Header("Configuración de Control")]
    public float velocidadRotacion = 250f;
    [Tooltip("Distancia a la que aparecerá el objeto frente a la cámara")]
    public float distanciaFrenteCamara = 2.5f;

    // Ya no necesitas asignarlo en el inspector, el script lo hace solo
    private Transform puntoAnclajeAutomatico;
    private GameObject modeloActual;
    private bool estaInspeccionando = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (canvasInspeccion != null)
            {
                DontDestroyOnLoad(canvasInspeccion);
            }
        }
        else
        {
            if (canvasInspeccion != null) Destroy(canvasInspeccion);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (panelInspeccionUI != null)
            panelInspeccionUI.SetActive(false);
    }

    private void Update()
    {
        if (!estaInspeccionando) return;

        // Rotación del modelo con clic izquierdo
        if (Input.GetMouseButton(0) && puntoAnclajeAutomatico != null)
        {
            float rotX = Input.GetAxis("Mouse X") * velocidadRotacion * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse Y") * velocidadRotacion * Time.deltaTime;

            puntoAnclajeAutomatico.Rotate(Vector3.up, -rotX, Space.World);
            puntoAnclajeAutomatico.Rotate(Vector3.right, rotY, Space.World);
        }

        // Cerrar con Escape o Clic Derecho
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CerrarInspeccion();
        }
    }

    // Este método prepara el anclaje en la escena en la que te encuentres actualmente
    private void PrepararAnclajeDinamico()
    {
        Camera camaraActual = Camera.main;

        if (camaraActual == null)
        {
            Debug.LogError("No se encontró una Main Camera en esta escena.");
            return;
        }

        // Si el anclaje se destruyó al cambiar de escena o no existe, lo creamos
        if (puntoAnclajeAutomatico == null)
        {
            GameObject nuevoAnclaje = new GameObject("PuntoAnclaje_Generado");
            puntoAnclajeAutomatico = nuevoAnclaje.transform;
        }

        // Lo hacemos hijo de la cámara actual y lo ponemos justo enfrente
        puntoAnclajeAutomatico.SetParent(camaraActual.transform);
        puntoAnclajeAutomatico.localPosition = new Vector3(0, 0, distanciaFrenteCamara);
        puntoAnclajeAutomatico.localRotation = Quaternion.identity;
    }

    public void AbrirInspeccion(GameObject prefabModelo3D, string nombreObjeto)
    {
        if (prefabModelo3D == null) return;

        LimpiarModeloActual();

        // 1. Configuramos el anclaje para esta habitación específica
        PrepararAnclajeDinamico();

        if (panelInspeccionUI != null)
            panelInspeccionUI.SetActive(true);

        if (textoNombreUI != null)
            textoNombreUI.text = nombreObjeto;

        // 2. Instanciamos el objeto en el anclaje generado
        if (puntoAnclajeAutomatico != null)
        {
            modeloActual = Instantiate(prefabModelo3D, puntoAnclajeAutomatico);
            modeloActual.transform.localPosition = Vector3.zero;
            modeloActual.transform.localRotation = Quaternion.identity;
        }

        estaInspeccionando = true;
    }

    public void CerrarInspeccion()
    {
        estaInspeccionando = false;
        LimpiarModeloActual();

        if (panelInspeccionUI != null)
            panelInspeccionUI.SetActive(false);
    }

    private void LimpiarModeloActual()
    {
        if (modeloActual != null)
        {
            Destroy(modeloActual);
            modeloActual = null;
        }
    }
}