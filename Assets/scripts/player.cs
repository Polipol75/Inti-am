using UnityEngine;
using System.Collections.Generic;
using System; // Para [System.Serializable]

public class Player : MonoBehaviour
{
    // --- VARIABLES DE MOVIMIENTO Y FÍSICA ---
    public float speed = 5f;
    private Rigidbody2D rb2D;

    private float move;
    public float jumpForce = 7f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    private Animator animator;

    // --- VARIABLES DE GRABACIÓN ---
    public List<PlayerMovementData> movementDataList = new List<PlayerMovementData>();
    private float recordInterval = 0.1f;
    private float recordTimer = 0f;

    // --- VARIABLES DEL DISPARO DEL CHULLO ---
    [Header("Disparo de Chullo")]
    public GameObject chulloPrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f; // Velocidad del proyectil
    public float spawnOffset = 0.4f;    // Distancia de aparición respecto al jugador
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private Collider2D playerCollider;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
            Debug.LogWarning("Player: no se encontró Collider2D en el jugador (para ignorar colisiones con proyectil).");
    }

    void Update()
    {
        // --- MOVIMIENTO Y SALTO ---
        move = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        // Giro del personaje
        if (move != 0)
        {
            float directionSign = move > 0 ? 1 : -1;
            transform.localScale = new Vector3(directionSign, 1, 1);
        }

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }

        // Lógica de salto con mejor gravedad
        if (rb2D.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (rb2D.linearVelocity.y < 0)
        {
            rb2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // --- DISPARO DEL CHULLO: ahora con tecla X ---
        if (Input.GetKeyDown(KeyCode.X) && Time.time >= nextFireTime)
        {
            ShootChullo();
        }

        // --- ANIMACIONES ---
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(move));
            animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
            animator.SetBool("IsGrounded", isGrounded);
        }

        // --- GRABACIÓN DE MOVIMIENTOS ---
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            PlayerMovementData data = new PlayerMovementData(Time.time, rb2D.position, move);
            movementDataList.Add(data);
            recordTimer = 0f;
        }
    }

    void ShootChullo()
    {
        nextFireTime = Time.time + fireRate;

        if (chulloPrefab == null || firePoint == null)
        {
            Debug.LogError("ERROR: Prefab del Chullo o FirePoint no asignado en el Inspector del Player.");
            return;
        }

        // Dirección del disparo según la orientación del jugador (transform.right respeta escala)
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        // Posición de aparición del proyectil (evita colisión con el jugador)
        Vector3 spawnPosition = firePoint.position + (transform.right * direction * spawnOffset);

        // Instanciar el chullo
        GameObject chullo = Instantiate(chulloPrefab, spawnPosition, Quaternion.identity);

        // Evitar colisiones entre proyectil y jugador (si tenemos colliders)
        if (playerCollider != null)
        {
            Collider2D projectileCollider = chullo.GetComponent<Collider2D>();
            if (projectileCollider != null)
            {
                Physics2D.IgnoreCollision(projectileCollider, playerCollider, true);
            }
        }

        // Asignar velocidad al Rigidbody2D del proyectil si existe
        Rigidbody2D rb = chullo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.right * direction * projectileSpeed;
            rb.gravityScale = 0f; // Aseguramos que no afecte la gravedad
        }

        // Destruir el proyectil tras tiempo máximo (por seguridad)
        Destroy(chullo, 6f);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }
}

// --- CLASE PARA GRABAR DATOS DE MOVIMIENTO ---
[System.Serializable]
public class PlayerMovementData
{
    public float time;
    public Vector2 position;
    public float moveDirection;

    public PlayerMovementData(float time, Vector2 position, float moveDirection)
    {
        this.time = time;
        this.position = position;
        this.moveDirection = moveDirection;
    }
}
