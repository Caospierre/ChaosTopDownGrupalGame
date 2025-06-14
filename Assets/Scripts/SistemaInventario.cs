using System;
using UnityEngine;
using UnityEngine.UI;

public class SistemaInventario : MonoBehaviour
{
    [SerializeField] private GameObject marcoInventario;
    [SerializeField] private Button[] botones;
    private int itemsDisponibles = 0;
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
        Debug.Log("Boton Clickado" + indiceBoton);
    }

    public void NuevoItem(ItemSO datosItem)
    {
        botones[itemsDisponibles].gameObject.SetActive(true);
        botones[itemsDisponibles].GetComponent<Image>().sprite = datosItem.icono;
        itemsDisponibles++;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            marcoInventario.SetActive(!marcoInventario.activeSelf);
        }
    }
}