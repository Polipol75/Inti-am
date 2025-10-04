using UnityEngine;

public class jefeCaminar : StateMachineBehaviour
{
    private JefeTigre jefe;

    // Se inicializa el jefe una sola vez al entrar al estado
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Solo busca si es nulo (optimizaci�n)
        if (jefe == null)
            jefe = animator.GetComponentInParent<JefeTigre>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Validaci�n de referencias
        if (jefe != null && jefe.playerTarget != null)
        {
            // Solo activamos el trigger de ataque seg�n la distancia
            if (jefe.EstaCercaDelJugador())
            {
                // ESTA ES LA L�NEA QUE NECESITA QUE EL TRIGGER EXISTA EN UNITY
                animator.SetTrigger("Ataque");
            }
            // NOTA: No es necesario llamar a jefe.PerseguirJugador() aqu�, 
            // ya que est� en el Update del JefeTigre.cs
        }
    }

    // Se recomienda limpiar las referencias al salir del estado si fuera necesario, 
    // pero no es estrictamente necesario en este caso.
}