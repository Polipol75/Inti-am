using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    private int bounceCount = 0;
    public int maxBounces = 3;

    // ⬅️ ¡Esta variable será visible en el Inspector!
    public int damageToDeal = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            vida playerHealth = collision.gameObject.GetComponent<vida>();

            if (playerHealth != null)
            {
                // Llama a TakeDamage() y le pasa el valor de daño (1)
                playerHealth.TakeDamage(damageToDeal);
            }
        }

        bounceCount++;
        if (bounceCount > maxBounces)
        {
            Destroy(gameObject);
        }
    }
}