using System;
using Prefab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SistemaInventario : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject marcoInventario;
    [SerializeField] private GameObject actualWeapon;
    [SerializeField] private Button[] botones;

    void Start()
    {
        for (int i = 0; i < botones.Length; i++)
        {
            int indiceBoton = i;
            botones[i].onClick.AddListener(() => BotonClickado(indiceBoton));
        }
    }

    private void BotonClickado(int indiceBoton)
    {
        var holder = botones[indiceBoton].GetComponent<ItemHolder>();
        if (holder == null || holder.item == null)
        {
            Debug.LogWarning("❌ ItemHolder o item nulo en botón " + indiceBoton);
            return;
        }

        var armaHolder = actualWeapon.GetComponent<ItemHolder>();
        if (armaHolder != null && armaHolder.item != null && armaHolder.HealthValue > 0f)
        {
            Debug.Log("↩️ Arma actual tiene salud > 0. Se devolverá al inventario.");
            NuevoItem(armaHolder.item);
        }

        Debug.Log($"🟡 Click en botón {indiceBoton} con item: {holder.ItemName}");

        Image imagenDestino = actualWeapon.GetComponent<Image>();
        if (imagenDestino != null)
        {
            imagenDestino.sprite = holder.Icono;
            imagenDestino.color = Color.white;
            Debug.Log("🟢 Sprite de arma actualizado.");
        }

        Transform destinoHealth = actualWeapon.transform.Find("Health");
        if (destinoHealth != null)
        {
            TextMeshProUGUI destinoTMP = destinoHealth.GetComponent<TextMeshProUGUI>();
            if (destinoTMP != null)
            {
                destinoTMP.text = holder.HealthValue.ToString("0");
                destinoHealth.gameObject.SetActive(true);
                Debug.Log($"🟢 Health visual del arma actualizado a {holder.HealthValue}");
            }
        }

        if (armaHolder != null)
        {
            armaHolder.item = Instantiate(holder.item);
            armaHolder.CargarPreviewDesdeItem();
            Debug.Log("🟢 Item asignado al arma actual.");
        }

        holder.item = null;
        botones[indiceBoton].gameObject.SetActive(false);
        ActualizarInventarioVisual();
    }

    public void NuevoItem(ItemSO datosItem)
    {
        for (int i = 0; i < botones.Length; i++)
        {
            var holder = botones[i].GetComponent<ItemHolder>();
            if (!botones[i].gameObject.activeSelf)
            {
                botones[i].gameObject.SetActive(true);
                holder.item = Instantiate(datosItem);
                holder.CargarPreviewDesdeItem();

                Debug.Log($"🆕 Nuevo item '{holder.ItemName}' con {holder.HealthValue} salud asignado a slot {i}.");

                Image image = botones[i].GetComponent<Image>();
                if (image != null)
                    image.sprite = holder.Icono;

                Transform healthTransform = botones[i].transform.Find("Health");
                if (healthTransform != null)
                {
                    TextMeshProUGUI texto = healthTransform.GetComponent<TextMeshProUGUI>();
                    if (texto != null)
                    {
                        texto.text = holder.HealthValue.ToString("0");
                        healthTransform.gameObject.SetActive(true);
                    }
                }

                return;
            }
        }

        Debug.LogWarning("❌ Inventario lleno. No se pudo agregar nuevo item.");
    }

    private void ActualizarInventarioVisual()
    {
        int destino = 0;

        for (int i = 0; i < botones.Length; i++)
        {
            var origenHolder = botones[i].GetComponent<ItemHolder>();
            if (!botones[i].gameObject.activeSelf || origenHolder.item == null)
                continue;

            if (i != destino)
            {
                var destinoHolder = botones[destino].GetComponent<ItemHolder>();
                destinoHolder.item = origenHolder.item;
                destinoHolder.CargarPreviewDesdeItem();

                Image imgDestino = botones[destino].GetComponent<Image>();
                if (imgDestino != null)
                    imgDestino.sprite = destinoHolder.Icono;

                Transform destinoHealth = botones[destino].transform.Find("Health");
                if (destinoHealth != null)
                {
                    TextMeshProUGUI destinoTMP = destinoHealth.GetComponent<TextMeshProUGUI>();
                    if (destinoTMP != null)
                    {
                        destinoTMP.text = destinoHolder.HealthValue.ToString("0");
                        destinoHealth.gameObject.SetActive(true);
                    }
                }

                origenHolder.item = null;
                botones[i].gameObject.SetActive(false);
                Debug.Log($"🔄 Item movido de slot {i} a {destino}");
            }

            botones[destino].gameObject.SetActive(true);
            destino++;
        }

        for (int j = destino; j < botones.Length; j++)
        {
            botones[j].gameObject.SetActive(false);
            var holder = botones[j].GetComponent<ItemHolder>();
            if (holder != null) holder.item = null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            marcoInventario.SetActive(!marcoInventario.activeSelf);
            Debug.Log("📂 Inventario toggled.");
        }
    }
}
