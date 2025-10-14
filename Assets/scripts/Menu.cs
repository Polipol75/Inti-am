using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Para cambiar de escena

public class Menu : MonoBehaviour, IPointerClickHandler
{
    public string SampleScene; // Nombre de la escena a cargar

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clic en imagen: " + gameObject.name);

        if (!string.IsNullOrEmpty(SampleScene))
        {
            SceneManager.LoadScene(SampleScene);
        }
        else
        {
            Debug.LogWarning("No se ha asignado una escena en " + gameObject.name);
        }
    }
}
