using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    [Tooltip("Altura relativa máxima del arco")]
    public float maxArcHeight = 2f; // puedes ajustar para que la parabola sea más plana o más alta

    [Header("Referencia al jugador")]
    private Transform player;

    private float timer;

    void Start()
    {
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

        Vector2 start = firePoint.position;
        Vector2 end = player.position;
        Vector2 distance = end - start;

        float gravity = Mathf.Abs(Physics2D.gravity.y);

        // 🔹 Altura máxima de la parabola
        float arcHeight = Mathf.Min(maxArcHeight, distance.magnitude / 2f);

        // Velocidad inicial vertical para alcanzar la altura máxima
        float velocityY = Mathf.Sqrt(2 * gravity * arcHeight);

        // Tiempo para subir y bajar
        float timeUp = velocityY / gravity;
        float totalHeight = arcHeight + Mathf.Max(0, distance.y);
        float timeDown = Mathf.Sqrt(2 * totalHeight / gravity);
        float totalTime = timeUp + timeDown;

        // Velocidad horizontal necesaria
        float velocityX = distance.x / totalTime;

        // Instanciar proyectil
        GameObject proj = Instantiate(projectilePrefab, start, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(velocityX, velocityY);
        }

        // Alinear visualmente el proyectil hacia el jugador
        proj.transform.right = (end - start).normalized;
    }
}
