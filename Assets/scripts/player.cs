using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb2D;

    private float move;

    public float jumForce = 4;
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

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumForce);
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
