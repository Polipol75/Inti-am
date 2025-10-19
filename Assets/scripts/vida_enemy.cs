using UnityEngine;
using System.Collections;

public class Vida : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("Vida total del enemigo. Aumenta este valor para hacerlo más fuerte.")]
    public float maxHealth = 100f; // más alto = más fuerte (por defecto 100)
    private float currentHealth;

    [Header("Daño recibido")]
    [Tooltip("Cuánto daño recibe por cada disparo del jugador.")]
    public float damagePerHit = 5f; // 100 / 5 = 20 disparos para morir

    [Header("Invulnerabilidad y efectos")]
    [Tooltip("Tiempo tras recibir daño donde no puede volver a ser dañado.")]
    public float invulnerabilityTime = 0.2f;
    public int flashTimes = 2;
    public float flashInterval = 0.08f;

    private bool canTakeDamage = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning(gameObject.name + " no tiene SpriteRenderer. El efecto flash no se aplicará.");
        }
        Debug.Log(gameObject.name + " inicializado con vida: " + currentHealth);
    }

    // Llamado por el proyectil
    public void TakeDamage(float damageAmount)
    {
        if (!canTakeDamage) return;

        // Aplicar daño reducido según configurado
        float finalDamage = damageAmount <= 0 ? damagePerHit : damageAmount;

        currentHealth -= finalDamage;
        Debug.Log(gameObject.name + " recibió " + finalDamage + " de daño. Vida restante: " + currentHealth);

        if (spriteRenderer != null)
            StartCoroutine(InvulnerabilityAndFlash());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator InvulnerabilityAndFlash()
    {
        canTakeDamage = false;

        if (spriteRenderer == null)
        {
            yield return new WaitForSeconds(invulnerabilityTime);
            canTakeDamage = true;
            yield break;
        }

        for (int i = 0; i < flashTimes; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }

        yield return new WaitForSeconds(invulnerabilityTime);
        canTakeDamage = true;
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " ha sido derrotado tras " + maxHealth / damagePerHit + " disparos!");
        Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }
}
