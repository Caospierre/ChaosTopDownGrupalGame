using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas1;
    [SerializeField] private GameObject canvas2;
    [SerializeField] private GameObject canvas3;
    [SerializeField] private AudioClip uiClickSound;
    [SerializeField] private Animator transitionAnim;

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void ClickBotonIniciar()
    {
        PlayClickSound();
        SceneManager.LoadScene(1);
    }

    public void ClickBotonOpciones()
    {
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        PlayClickSound();
    }
    public void ClickBotonRegresar()
    {
        canvas2.SetActive(false);
        canvas1.SetActive(true);
        PlayClickSound();
    }

    public void ClickBotonSalir()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void ClickBotonBotones()
    {
        canvas1.SetActive(false);
        canvas3.SetActive(true);
        PlayClickSound();
    }

    public void ClickBotonRegresarBotones()
    {
        canvas3.SetActive(false);
        canvas1.SetActive(true);
        PlayClickSound();
    }

    /*IEnumerator ChangeToScene1()
    {
        transitionAnim.SetTrigger("exit");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }*/

    void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEffect(uiClickSound);
    }
}
