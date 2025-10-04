using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb2D;

    private float move;

    public float jumpForce = 7f;           // fuerza inicial del salto

    // Variables para salto tipo Hollow Knight
    public float fallMultiplier = 2.5f;    // cae más rápido
    public float lowJumpMultiplier = 2f;   // salto corto si se suelta el botón

    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadious = 0.1f;
    public LayerMask groundLayer;

    private Animator animator;

    // Lista para registrar los movimientos
    public List<PlayerMovementData> movementDataList = new List<PlayerMovementData>();

    // Variables para controlar el intervalo de registro
    private float recordInterval = 0.1f; // cada 0.1 segundos
    private float recordTimer = 0f;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Imprimir encabezado de tabla
        Debug.Log("Time      | PosX     | PosY     | Direction");
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

        // Actualizar si está en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadious, groundLayer);

        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        if (move != 0)
            transform.localScale = new Vector3(math.sign(move), 1, 1);

        // --- SALTO VARIABLE TIPO HOLLOW KNIGHT ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }

        // Saltos variables con gravedad modificada
        if (rb2D.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Si sube pero el botón ya no está presionado, hace un salto corto
            rb2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (rb2D.linearVelocity.y < 0)
        {
            // Si está cayendo, cae más rápido
            rb2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);

        // Registrar datos cada 0.1 segundos
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            PlayerMovementData data = new PlayerMovementData(Time.time, rb2D.position, move);
            movementDataList.Add(data);

            // Mostrar tabla en consola
            Debug.Log(string.Format("{0,6:0.00}    | {1,6:0.00}   | {2,6:0.00}   | {3,1}",
                data.time, data.position.x, data.position.y, data.moveDirection));

            recordTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadious, groundLayer);
    }
}

// Clase para guardar los datos de movimiento
[System.Serializable]
public class PlayerMovementData
{
    public float time;
    public Vector2 position;
    public float moveDirection; // -1 izquierda, 0 quieto, 1 derecha

    public PlayerMovementData(float time, Vector2 position, float moveDirection)
    {
        this.time = time;
        this.position = position;
        this.moveDirection = moveDirection;
    }
}
