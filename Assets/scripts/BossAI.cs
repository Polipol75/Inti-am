using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;

    // NUEVA: Distancia mínima que el jefe mantendrá con el jugador antes de detenerse
    public float attackRange = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 1. Orientación (Flip)
        FlipTowardsPlayer();

        // 2. Movimiento condicional
        MoveTowardsPlayer();
    }

    void FlipTowardsPlayer()
    {
        // (La lógica de girar la escala X se mantiene igual)
        float directionX = player.position.x - transform.position.x;

        if (directionX > 0) // Jugador a la DERECHA
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (directionX < 0) // Jugador a la IZQUIERDA
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void MoveTowardsPlayer()
    {
        // 1. Calcular la distancia horizontal al jugador
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        // 2. Comprobar si está fuera del rango de ataque
        if (distanceToPlayer > attackRange)
        {
            // SI ESTÁ FUERA DE RANGO: MOVERSE

            // Calcula la dirección en X (1 para derecha, -1 para izquierda)
            float directionX = Mathf.Sign(player.position.x - transform.position.x);

            // Aplica la velocidad en la dirección del jugador
            Vector2 targetVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // SI ESTÁ DENTRO DEL RANGO: DETENERSE

            // Establece la velocidad horizontal a cero, manteniendo la velocidad vertical (rb.velocity.y)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // Aquí es donde irá la lógica del ataque cuando la implementes
            Debug.Log("Jefe en rango de ataque. ¡Detenido!");
        }
    }
}