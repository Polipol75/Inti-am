using UnityEngine;

public class CanvasDontDestroy : MonoBehaviour
{
    // Patrón Singleton para evitar duplicados del Canvas
    public static CanvasDontDestroy Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ¡La clave para que persista!
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe uno, destruye esta copia para evitar duplicados.
            Destroy(gameObject);
        }
    }
}