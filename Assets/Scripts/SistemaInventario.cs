using System;
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
        if (!botones[indiceBoton].gameObject.activeSelf)
            return;

        Debug.Log("Botón clickado: " + indiceBoton);

        int duracion = ObtenerDuracionDesdeBoton(botones[indiceBoton]);
        if (duracion <= 0)
        {
            botones[indiceBoton].gameObject.SetActive(false);
            ActualizarInventarioVisual();
            return;
        }

        Image imagenOrigen = botones[indiceBoton].GetComponent<Image>() ?? botones[indiceBoton].GetComponentInChildren<Image>();
        Transform origenHealth = botones[indiceBoton].transform.Find("Health");

        // Si actualWeapon está vacío, simplemente asignar
        Image imagenDestino = actualWeapon.GetComponent<Image>();
        Transform destinoHealth = actualWeapon.transform.Find("Health");

        if (imagenDestino != null && imagenOrigen != null)
            imagenDestino.sprite = imagenOrigen.sprite;

        if (destinoHealth != null && origenHealth != null)
        {
            TextMeshProUGUI origenTMP = origenHealth.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI destinoTMP = destinoHealth.GetComponent<TextMeshProUGUI>();

            if (origenTMP != null && destinoTMP != null)
            {
                destinoTMP.text = origenTMP.text;
                destinoHealth.gameObject.SetActive(true);
            }
        }

        // Desactivar el botón y reorganizar
        botones[indiceBoton].gameObject.SetActive(false);
        ActualizarInventarioVisual();
    }

    public void NuevoItem(ItemSO datosItem)
    {
        for (int i = 0; i < botones.Length; i++)
        {
            if (!botones[i].gameObject.activeSelf)
            {
                Button boton = botones[i];
                boton.gameObject.SetActive(true);

                Image image = boton.GetComponent<Image>() ?? boton.GetComponentInChildren<Image>();
                if (image != null)
                    image.sprite = datosItem.icono;

                Transform healthTransform = boton.transform.Find("Health");
                if (healthTransform != null)
                {
                    TextMeshProUGUI texto = healthTransform.GetComponent<TextMeshProUGUI>();
                    if (texto != null)
                    {
                        healthTransform.gameObject.SetActive(true);
                        texto.text = "Duración: " + datosItem.haelth;
                    }
                }

                return;
            }
        }

        Debug.LogWarning("No hay espacio en el inventario.");
    }

    private void ActualizarInventarioVisual()
    {
        int destino = 0;

        for (int i = 0; i < botones.Length; i++)
        {
            if (!botones[i].gameObject.activeSelf)
                continue;

            if (i != destino)
            {
                Button temp = botones[destino];

                Image imgOrigen = botones[i].GetComponent<Image>() ?? botones[i].GetComponentInChildren<Image>();
                Image imgDestino = temp.GetComponent<Image>() ?? temp.GetComponentInChildren<Image>();
                if (imgOrigen != null && imgDestino != null)
                {
                    imgDestino.sprite = imgOrigen.sprite;
                    imgOrigen.sprite = null;
                }

                Transform origenHealth = botones[i].transform.Find("Health");
                Transform destinoHealth = temp.transform.Find("Health");
                if (origenHealth != null && destinoHealth != null)
                {
                    TextMeshProUGUI origenTMP = origenHealth.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI destinoTMP = destinoHealth.GetComponent<TextMeshProUGUI>();
                    if (origenTMP != null && destinoTMP != null)
                    {
                        destinoTMP.text = origenTMP.text;
                        origenTMP.text = "";
                        destinoHealth.gameObject.SetActive(true);
                        origenHealth.gameObject.SetActive(false);
                    }
                }

                botones[destino].gameObject.SetActive(true);
                botones[i].gameObject.SetActive(false);
            }

            destino++;
        }

        for (int j = destino; j < botones.Length; j++)
        {
            botones[j].gameObject.SetActive(false);
        }
    }

    private int ObtenerDuracionDesdeBoton(Button boton)
    {
        Transform healthTransform = boton.transform.Find("Health");
        if (healthTransform != null)
        {
            TextMeshProUGUI texto = healthTransform.GetComponent<TextMeshProUGUI>();
            if (texto != null)
            {
                string contenido = texto.text;
                string[] partes = contenido.Split(':');
                if (partes.Length == 2 && int.TryParse(partes[1].Trim(), out int valor))
                {
                    return valor;
                }
            }
        }

        return -1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            marcoInventario.SetActive(!marcoInventario.activeSelf);
        }
    }
}
