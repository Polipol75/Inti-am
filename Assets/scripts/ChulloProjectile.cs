using UnityEngine;

public class ChulloProjectile : MonoBehaviour
{
    public int damage = 1;           // daño que hace por impacto (ajusta a 1)
    public float lifeTime = 6f;      // tiempo máximo de vida del proyectil
    public bool ignorePlayerCollision = true; // evitar colisionar con jugador si ya lo ignoraste desde Player

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Usamos trigger para detectar impacto (recomendado: Collider2D isTrigger = true)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignoramos colisiones con el jugador si quieres
        if (ignorePlayerCollision && collision.CompareTag("Player"))
            return;

        // Si el objeto impactado tiene el script Vida, le aplicamos daño
        Vida vida = collision.GetComponent<Vida>();
        if (vida != null)
        {
            vida.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Si no es jugador ni enemigo, por defecto destruir el proyectil (opcional)
        // Por ejemplo: paredes, suelo o cualquier otro collider lo destruirá.
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
        else
        {
            // Si quieres que atraviese triggers no destructivos, quita esta sección.
            Destroy(gameObject);
        }
    }

    // Si prefieres colisiones físicas (no trigger), usa esto en vez del OnTriggerEnter2D:
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ignorePlayerCollision && collision.collider.CompareTag("Player"))
            return;

        Vida vida = collision.collider.GetComponent<Vida>();
        if (vida != null)
        {
            vida.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
    */
}
