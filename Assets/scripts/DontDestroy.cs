using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        // Esto mantiene el objeto (y todos sus hijos) al cargar una nueva escena.
        DontDestroyOnLoad(this.gameObject);
    }
}