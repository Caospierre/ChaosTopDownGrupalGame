using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour, Interactuable
{
    [Header("Gestión")]
    [SerializeField] private GameManagerSO gameManager;

    [Header("Diálogo")]
    [SerializeField, TextArea(1, 5)] private string[] frases;
    [SerializeField] private float tiempoEntreLetras = 0.05f;
    [SerializeField] private GameObject cuadroDialogo;
    [SerializeField] private TextMeshProUGUI textoDialogo;

    [Header("Automatización")]
    [SerializeField] private bool mostrarAlInicio = false;
    [SerializeField] private bool autoAvanzarFrases = false;
    [SerializeField] private float tiempoFinalEspera = 1.5f;

    private bool hablando = false;
    private int indiceActual = -1;

    void Start()
    {
        if (mostrarAlInicio)
            StartCoroutine(InicioDialogoAutomatico());
    }

    public void Interactuar()
    {
        gameManager.CambiarEstadoPlayer(false);
        cuadroDialogo.SetActive(true);

        if (!hablando)
            SiguienteFrase();
        else
            CompletarFrase();
    }

    private void SiguienteFrase()
    {
        indiceActual++;
        if (indiceActual >= frases.Length)
        {
            TerminarDialogo();
        }
        else
        {
            StartCoroutine(EscribirFrase());
        }
    }

    private void TerminarDialogo()
    {
        hablando = false;
        textoDialogo.text = "";
        indiceActual = -1;
        cuadroDialogo.SetActive(false);
        gameManager.CambiarEstadoPlayer(true);
    }

    private IEnumerator EscribirFrase()
    {
        hablando = true;
        textoDialogo.text = "";
        char[] caracteresFrase = frases[indiceActual].ToCharArray();

        foreach (char caracter in caracteresFrase)
        {
            textoDialogo.text += caracter;
            yield return new WaitForSeconds(tiempoEntreLetras);
        }

        hablando = false;
    }

    private void CompletarFrase()
    {
        StopAllCoroutines();
        textoDialogo.text = frases[indiceActual];
        hablando = false;
    }

    private IEnumerator InicioDialogoAutomatico()
    {
        Interactuar();

        while (indiceActual < frases.Length)
        {
            if (!hablando && autoAvanzarFrases)
            {
                yield return new WaitForSeconds(1);
                SiguienteFrase();
            }
            yield return null;
        }

        yield return new WaitForSeconds(tiempoFinalEspera); 
        TerminarDialogo();
    }
}
