using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instancia;

    void Awake()
    {
        // Si ya existe un AudioManager, destruye este duplicado
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        // Mantiene este objeto al cambiar de escena
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }
}