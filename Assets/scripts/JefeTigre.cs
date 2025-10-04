using UnityEngine;

public class JefeTigre : MonoBehaviour
{
    public Animator animator;
    public float speed_walk;
    public GameObject target;

    void Update()
    {
        if (target != null)
        {
            // Calculamos dirección hacia el target
            Vector3 dir = target.transform.position - transform.position;
            dir.Normalize();

            // Movemos al jefe hacia el target
            transform.position += dir * speed_walk * Time.deltaTime;

            // Actualizamos el parámetro "speed" del Animator
            animator.SetFloat("speed", speed_walk);

            // Cambiamos la escala X para mirar al jugador
            if (dir.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Sign(dir.x) * Mathf.Abs(scale.x); // positivo si a la derecha, negativo si a la izquierda
                transform.localScale = scale;
            }
        }
        else
        {
            // Si no hay target, la velocidad es 0
            animator.SetFloat("speed", 0f);
        }
    }
}
