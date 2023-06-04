using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;
    public AudioManager audioManager;
    private bool isPaused;

    private void Start() => fullscreenToggle.isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow);
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void Options() => optionsMenu.SetActive(true);
    public void CloseOptions() => optionsMenu.SetActive(false);

    public void PauseGame()
    {
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void SetVolume()
    {
        foreach (Sound s in audioManager.sounds)
        {
            if (s.name is "Theme" or "ButtonPress")
                s.source.volume = sfxSlider.value - .34f;
            else
                s.source.volume = sfxSlider.value;
        }
        string filePath = @"C:\Users\sidc2\Umbra_ testing\Assets\_Scripts\StateManagers\volume.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(sfxSlider.value);
            writer.Close();
        }
    }
    public void PlayButtonPressSound() => audioManager.Play("ButtonPress");
    public void ResumeGame()
    {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void ToggleFullScreen()
    {
        if (fullscreenToggle.isOn)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    public void QuitApp() => Application.Quit();
}
