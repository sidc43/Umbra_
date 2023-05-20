using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartOver : MonoBehaviour
{
    void Start() => Time.timeScale = 1;
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
