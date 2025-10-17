using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb2D;
    private bool isFacingRight = true;

    private float move;

    public float jumpForce = 7f;

    // Ajustado para una caída menos acelerada por defecto.
    public float fallMultiplier = 1.5f; 
    public float lowJumpMultiplier = 3f; // Valor que mencionaste haber puesto.

    public float maxFallSpeed = 20f;

    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadious = 0.1f;
    public LayerMask groundLayer;

    private Animator animator;

    public List<PlayerMovementData> movementDataList = new List<PlayerMovementData>();

    private float recordInterval = 0.1f;
    private float recordTimer = 0f;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Inicialización segura: Asume que al inicio está a 0 grados (mirando a la derecha).
        isFacingRight = true; 
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(move));

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
        }

        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            PlayerMovementData data = new PlayerMovementData(Time.time, rb2D.position, move);
            movementDataList.Add(data);
            recordTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadious, groundLayer);

        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        TurnCheck();

        if (rb2D.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb2D.linearVelocity.y < 0)
        {
            rb2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x,
            Mathf.Clamp(rb2D.linearVelocity.y, -maxFallSpeed, maxFallSpeed + 5f));

        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void TurnCheck()
    {
        if (move > 0 && !isFacingRight)
        {
            Turn();
        }
        else if (move < 0 && isFacingRight)
        {
            Turn();
        }
    }

    private void Turn()
    {
        isFacingRight = !isFacingRight;

        float yRotation = isFacingRight ? 0f : 180f;

        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = yRotation;

        transform.rotation = Quaternion.Euler(currentRotation);
    }
}

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