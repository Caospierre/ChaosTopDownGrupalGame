using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public float danho;
    public string nombre;
    public string nivelNecesario;
    public Sprite icono;

}
