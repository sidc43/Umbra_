using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartOver : MonoBehaviour
{
    public AudioManager audioManager;

    void Start()
    {
        audioManager.Play("GameOver");
        Cursor.visible = true; 
        Time.timeScale = 1;
    }
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void Quit() => Application.Quit();
    public void Load() => SceneManager.LoadScene("Main");
}
