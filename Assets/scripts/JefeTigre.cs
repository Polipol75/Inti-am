using UnityEngine;

public class JefeTigre : MonoBehaviour
{
    [Header("Configuración de Persecución")]
    public Transform playerTarget;
    public float velocidadPersecucion = 3f;
    public float tiempoInicioPersecucion = 3f;

    [Header("Configuración de Ataque")]
    public float rangoAtaque = 0.7f;

    private bool estaPersiguiendo = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (playerTarget == null)
            Debug.LogError("¡Arrastra el jugador en el Inspector!");

        Invoke(nameof(ActivarPersecucion), tiempoInicioPersecucion);
    }

    void Update()
    {
        // Movimiento constante mientras persigue
        if (EstaPersiguiendo() && playerTarget != null)
        {
            PerseguirJugador();
        }
    }

    public void PerseguirJugador()
    {
        Vector3 direccion = (playerTarget.position - transform.position).normalized;
        direccion.y = 0;

        transform.position += direccion * velocidadPersecucion * Time.deltaTime;

        float movimientoX = direccion.x;
        if (movimientoX > 0.05f) GirarSprite(1);
        else if (movimientoX < -0.05f) GirarSprite(-1);
    }

    public void ActivarPersecucion()
    {
        estaPersiguiendo = true;
        // NOTA: Asegúrate de que el Trigger "Caminar" también exista en el Animator.
        anim.SetTrigger("Caminar");
    }

    void GirarSprite(int nuevaDireccion)
    {
        Vector3 escalaActual = transform.localScale;
        escalaActual.x = nuevaDireccion == 1 ? -Mathf.Abs(escalaActual.x) : Mathf.Abs(escalaActual.x);
        transform.localScale = escalaActual;
    }

    public bool EstaPersiguiendo() => estaPersiguiendo;

    public void Atacar()
    {
        Debug.Log("¡El jefe ataca!");
    }

    public bool EstaCercaDelJugador()
    {
        if (playerTarget == null) return false;
        // Se corrige la distancia para que solo tome en cuenta el eje X (como lo hace en PerseguirJugador)
        float distancia = Mathf.Abs(playerTarget.position.x - transform.position.x);
        return distancia <= rangoAtaque;
    }
}