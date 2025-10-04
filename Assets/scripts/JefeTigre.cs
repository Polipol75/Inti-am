using UnityEngine;

public class JefeTigre : MonoBehaviour
{
    public Animator animator;
    public float speed_walk;
    public GameObject target;

    public float detectionRange = 10f; // distancia máxima para detectar al jugador
    private string lastState = "";

    void Update()
    {
        if (target == null)
        {
            animator.SetFloat("speed", 0f);
            return;
        }

        Vector3 diff = target.transform.position - transform.position;

        // Si el jugador está fuera del rango, no hacer nada
        if (diff.magnitude > detectionRange)
        {
            animator.SetFloat("speed", 0f);
            return;
        }

        // Determinar el estado según posición relativa
        string state;
        if (diff.y > 1f)
            state = "Arriba";
        else if (diff.x > 0)
            state = diff.y > 0.5f ? "Derecha saltando" : "Derecha";
        else
            state = diff.y > 0.5f ? "Izquierda saltando" : "Izquierda";

        // Solo mostrar en consola si cambia de estado
        if (state != lastState)
        {
            Debug.Log("Jugador está en: " + state);
            lastState = state;
        }

        // --- Movimiento del jefe hacia el jugador ---
        Vector3 dir = diff.normalized;
        transform.position += dir * speed_walk * Time.deltaTime;

        // Actualizar Animator
        animator.SetFloat("speed", speed_walk);

        // Cambiar escala X para mirar al jugador
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
