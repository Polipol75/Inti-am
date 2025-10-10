using UnityEngine;

public class EnemyFlipBody : MonoBehaviour
{
    public Transform player; // asigna el jugador en el inspector
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale; // guarda la escala original
    }

    void Update()
    {
        if (player == null) return;

        // Mira hacia el jugador invirtiendo X si es necesario
        float xScale = player.position.x > transform.position.x ? Mathf.Abs(originalScale.x) : -Mathf.Abs(originalScale.x);
        transform.localScale = new Vector3(xScale, originalScale.y, originalScale.z);
    }
}
