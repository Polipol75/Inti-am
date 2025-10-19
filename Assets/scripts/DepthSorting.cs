using UnityEngine;

public class DepthSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 1. Obtiene el componente SpriteRenderer. Este componente maneja la imagen.
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Error: El script DepthSorting requiere un SpriteRenderer en el mismo GameObject.");
            enabled = false; // Desactiva el script si no hay SpriteRenderer para evitar errores.
        }
    }

    void Update()
    {
        // La lógica: Cuanto más bajo esté el objeto en el eje Y (más cerca del jugador), 
        // más alto debe ser su 'Sorting Order' para dibujarse al frente.

        // Multiplicamos la posición Y por un número negativo grande (-100, -1000, etc.)
        // para convertirla en un valor entero para el Sorting Order.
        // Usar -100 da suficiente rango para que el orden sea muy granular.

        int order = Mathf.RoundToInt(transform.position.y * -100);

        // 2. Asigna el nuevo orden de renderizado.
        spriteRenderer.sortingOrder = order;
    }
}