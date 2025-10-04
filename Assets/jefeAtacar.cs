using UnityEngine;

public class jefeAtacar : StateMachineBehaviour
{
    private JefeTigre jefe;

    // Se inicializa el jefe una sola vez al entrar al estado
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (jefe == null)
            jefe = animator.GetComponentInParent<JefeTigre>();

        // Llama a la función Atacar una sola vez al entrar al estado (si la quieres como un evento único)
        // jefe?.Atacar(); 
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (jefe == null || jefe.playerTarget == null) return;

        // Si quieres que el ataque se ejecute continuamente mientras la animación esté activa (menos común)
        // Puedes dejarlo así:
        jefe.Atacar();
    }
}