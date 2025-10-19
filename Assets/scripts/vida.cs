using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class vida : MonoBehaviour
{
    private GameObject[] hearts;

    // Cambia 6 (3 corazones * 2 puntos) 
    private const int MAX_HEALTH_POINTS = 18; // ⬅️ ¡Aumenta duracion de vida!
    private int currentHealthPoints;

    // Nombre de tu escena de menú
    private const string menuSceneName = "Menu";

    void Start()
    {
        // Lógica de búsqueda del HUD y corazones (asume Tag "GameUI" y ruta "Canvas/vidaX")
        GameObject uiRoot = GameObject.FindGameObjectWithTag("GameUI");

        if (uiRoot != null)
        {
            hearts = new GameObject[3];
            hearts[0] = uiRoot.transform.Find("Canvas/vida0")?.gameObject;
            hearts[1] = uiRoot.transform.Find("Canvas/vida1")?.gameObject;
            hearts[2] = uiRoot.transform.Find("Canvas/vida2")?.gameObject;

            if (hearts[0] != null && hearts[1] != null && hearts[2] != null)
            {
                // Inicializa los puntos de vida al máximo
                if (currentHealthPoints <= 0)
                {
                    currentHealthPoints = MAX_HEALTH_POINTS;
                }
                UpdateHealthUI();
            }
            else
            {
                Debug.LogError("Error: Las referencias de los corazones NO se encontraron.");
            }
        }
        else
        {
            Debug.LogError("Error: El script 'vida.cs' no encontró el objeto persistente (HUD/Canvas).");
        }
    }

    /// <summary>
    /// Función llamada por el proyectil. Acepta el daño como parámetro.
    /// </summary>
    public void TakeDamage(int damageAmount) // ⬅️ ¡Modificado para recibir el daño!
    {
        if (currentHealthPoints > 0)
        {
            currentHealthPoints -= damageAmount; // Resta la cantidad de daño
            UpdateHealthUI();

            if (currentHealthPoints <= 0)
            {
                Die();
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (hearts == null || hearts.Length == 0) return;

        // 3 corazones = 6 puntos. Corazón 1 desaparece con 4 o menos, Corazón 2 con 2 o menos.
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                // Calcula el umbral de salud (ej: Corazón 0 necesita > 0 puntos, Corazón 1 necesita > 2 puntos, Corazón 2 necesita > 4 puntos)
                int healthThreshold = (i + 1) * 2;
                hearts[i].SetActive(currentHealthPoints > (healthThreshold - 2));
            }
        }
    }

    private void Die()
    {
        Debug.Log("¡Player Game Over! Regresando al menú.");
        gameObject.SetActive(false);
        SceneManager.LoadScene(menuSceneName);
    }
}