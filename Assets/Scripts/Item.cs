using UnityEngine;

public class Item : MonoBehaviour, Interactuable
{
    [SerializeField] private ItemSO misDatos;
    [SerializeField] private GameManagerSO gameManager;
    public void Interactuar()
    {
        try
        {
            ItemSO copia = Instantiate(misDatos); // 👈 clona el ScriptableObject
            gameManager.Inventario.NuevoItem(copia);

            Debug.Log("→ Item agregado correctamente, ahora se destruirá");
            Destroy(this.gameObject);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al intentar agregar el item: " + ex.Message);
        }
    }
}
