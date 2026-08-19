using UnityEngine;

public class ControladorVistaFlashback : MonoBehaviour
{
    [Header("Búsqueda Automática")]
    [Tooltip("Escribe el nombre EXACTO del GameObject/Canvas que contiene tus botones de navegación")]
    public string nombreExactoDelPanel = "botones"; // 👈 Escribe aquí el nombre que tiene en la Jerarquía

    private GameObject panelBuscado;

    private void Start()
    {
        // Busca el objeto en memoria aunque venga de otra escena mediante DontDestroyOnLoad
        panelBuscado = GameObject.Find(nombreExactoDelPanel);

        if (panelBuscado != null)
        {
            panelBuscado.SetActive(false); // Lo oculta al iniciar el flashback
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto llamado: " + nombreExactoDelPanel);
        }
    }

    private void OnDestroy()
    {
        // Al salir de la escena de flashback, vuelve a mostrar los botones para las demás habitaciones
        if (panelBuscado != null)
        {
            panelBuscado.SetActive(true);
        }
    }
}