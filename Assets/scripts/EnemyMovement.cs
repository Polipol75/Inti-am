using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3f;           // Velocidad horizontal
    public float jumpForce = 7f;           // Fuerza de salto ocasional
    public float randomMoveInterval = 2f;  // Intervalo para cambiar dirección lateral
    public float jumpCooldown = 1f;        // Tiempo entre saltos
    [Range(0f, 1f)]
    public float chanceToJump = 0.3f;      // Probabilidad de saltar

    [Header("Suelo y paredes")]
    public LayerMask groundLayer;          // Layer de plataformas/ground
    public float groundCheckDistance = 1f; // Distancia del raycast vertical
    public float wallCheckDistance = 0.5f; // Distancia del raycast horizontal

    private Rigidbody2D rb;
    private float randomTimer;
    private float jumpTimer;
    private float moveDirection;           // Dirección horizontal actual (-1, 0, 1)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        randomTimer = randomMoveInterval;
        jumpTimer = 0f;
        moveDirection = Random.Range(-1f, 1f); // Dirección inicial
    }

    void Update()
    {
        // Reducir timers
        randomTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        HandleMovement();
    }

    void HandleMovement()
    {
        // --- Saltos ocasionales ---
        if (IsGrounded() && jumpTimer <= 0f)
        {
            if (Random.value < chanceToJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimer = jumpCooldown;
            }
        }

        // --- Cambio de dirección si hay pared adelante ---
        if (IsWallAhead())
        {
            moveDirection *= -1f;
        }

        // --- Movimiento lateral aleatorio / caminata ---
        if (randomTimer <= 0f && IsGrounded())
        {
            moveDirection = Random.Range(-1f, 1f); // Cambia dirección
            randomTimer = randomMoveInterval;
        }

        // Aplicar movimiento horizontal
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        // Raycast hacia abajo solo para colliders en la layer groundLayer
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    bool IsWallAhead()
    {
        // Raycast horizontal según la dirección actual
        Vector2 dir = new Vector2(moveDirection, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, wallCheckDistance, groundLayer);

        // Dibuja el raycast para depuración en la Scene
        Debug.DrawRay(transform.position, dir * wallCheckDistance, Color.red);

        return hit.collider != null;
    }
}
