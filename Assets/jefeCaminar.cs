using UnityEngine;

public class jefeCaminar : StateMachineBehaviour
{
    private JefeTigre jefe;

    // Se inicializa el jefe una sola vez al entrar al estado
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Solo busca si es nulo (optimización)
        if (jefe == null)
            jefe = animator.GetComponentInParent<JefeTigre>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Validación de referencias
        if (jefe != null && jefe.playerTarget != null)
        {
            // Solo activamos el trigger de ataque según la distancia
            if (jefe.EstaCercaDelJugador())
            {
                // ESTA ES LA LÍNEA QUE NECESITA QUE EL TRIGGER EXISTA EN UNITY
                animator.SetTrigger("Ataque");
            }
            // NOTA: No es necesario llamar a jefe.PerseguirJugador() aquí, 
            // ya que está en el Update del JefeTigre.cs
        }
    }

    // Se recomienda limpiar las referencias al salir del estado si fuera necesario, 
    // pero no es estrictamente necesario en este caso.
}