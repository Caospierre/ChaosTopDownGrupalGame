using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public float danho;
    public string nombre;
    public string health;
    public string nivelNecesario;
    public Sprite icono;
    public bool isWall;

}
