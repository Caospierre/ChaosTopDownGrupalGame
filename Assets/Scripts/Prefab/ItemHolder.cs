using UnityEngine;

namespace Prefab
{
    public class ItemHolder : MonoBehaviour
    {
        [Header("Referencia al ScriptableObject del item")]
        public ItemSO item;

        [Header("Vista previa (readonly)")]
        [SerializeField] private float healthPreview;
        [SerializeField] private string nombrePreview;
        [SerializeField] private Sprite iconoPreview;

        public float HealthValue => healthPreview;
        public string ItemName => nombrePreview;
        public Sprite Icono => iconoPreview;

        public void CargarPreviewDesdeItem()
        {
            if (item != null)
            {
                float.TryParse(item.health, out healthPreview);
                nombrePreview = item.name;
                iconoPreview = item.icono;
            }
            else
            {
                healthPreview = 0f;
                nombrePreview = "";
                iconoPreview = null;
            }
        }
    }
}