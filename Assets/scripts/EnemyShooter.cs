using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce = 10f;
    public float shootInterval = 2f;

    [Header("Referencia al jugador")]
    private Transform player;

    private float timer;

    void Start()
    {
        // Busca el jugador por tag (aseg�rate que tenga el tag "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= shootInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        // Guardar posici�n del jugador en el momento del disparo
        Vector2 targetPosition = player.position;
        Vector2 startPosition = firePoint.position;

        // Diferencia entre posiciones
        Vector2 distance = targetPosition - startPosition;

        // Par�metros f�sicos
        float gravity = Mathf.Abs(Physics2D.gravity.y); // gravedad del mundo 2D
        float height = distance.y;
        distance.y = 0;

        // Puedes ajustar esto para cambiar la "altura m�xima" del arco
        float heightBoost = 2f;

        // Calcular velocidad inicial en Y para alcanzar la altura
        float velocityY = Mathf.Sqrt(2 * gravity * heightBoost);

        // Tiempo de subida + bajada
        float timeUp = velocityY / gravity;
        float totalHeight = heightBoost + Mathf.Max(0, height);
        float timeDown = Mathf.Sqrt(2 * totalHeight / gravity);
        float totalTime = timeUp + timeDown;

        // Velocidad horizontal necesaria
        float velocityX = distance.x / totalTime;

        // Crear proyectil
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Aplicar velocidad calculada
            Vector2 velocity = new Vector2(velocityX, velocityY);
            rb.linearVelocity = velocity;
        }
    }


}
