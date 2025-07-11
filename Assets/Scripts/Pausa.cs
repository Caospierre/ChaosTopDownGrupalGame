using Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    [SerializeField] private GameObject canvas1;
    [SerializeField] private GameObject canvas2;
    [SerializeField] private AudioClip uiClickSound;
    private Player player;
    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (canvas1.activeSelf)
            {
                ClickBotonReanudar();
            }

            else if (canvas2.activeSelf)
            {
                canvas2.SetActive(false);
                canvas1.SetActive(true);
                PlayClickSound();
            }

            else
            {
                canvas1.SetActive(true);
                Time.timeScale = 0;
                if (player != null)
                {
                    Player playerScript = player.GetComponent<Player>();
                    if (playerScript != null)
                    {
                        playerScript.enabled = false;
                    }
                }
            }
        }
    }

    public void ClickBotonReanudar()
    {
        canvas1.SetActive(false);
        PlayClickSound();
        Time.timeScale = 1;
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.enabled = true;
            }
                }
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
    public void ClickBotonMenuPrincipal()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        PlayClickSound();
    }
    public void ClickBotonReiniciarNivel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
        PlayClickSound();
    }

    void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEffect(uiClickSound);
    }
}