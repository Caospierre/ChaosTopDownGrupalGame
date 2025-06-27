using UnityEngine;
using UnityEngine.SceneManagement;

public class RedirectGame : MonoBehaviour
{
    [Header("Configuración")]
    public int sceneIndex = 1; // Índice de la escena a cargar
    public float delaySeconds = 5f; // Tiempo de espera en segundos

    [Header("Objeto a ocultar")]
    public GameObject objectToHide; // GameObject a desactivar

    void Start()
    {
        Invoke(nameof(HideObjectAndLoadScene), delaySeconds);
    }

    void HideObjectAndLoadScene()
    {
        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
