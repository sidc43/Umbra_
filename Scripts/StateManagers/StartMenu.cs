using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject controlsMenu;
    public AudioManager audioManager;
    public Toggle fullscreenToggle;
    public Slider sfxSlider;
    
    private void Start() 
    {
        audioManager.Play("Theme");
        fullscreenToggle.isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow);
        string filePath = @"C:\Users\sidc2\Umbra_ testing\Assets\_Scripts\StateManagers\volume.txt";
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine();
                if (float.TryParse(line, out float floatValue))
                {
                    foreach (Sound s in audioManager.sounds)
                    {
                        s.source.volume = floatValue;
                        sfxSlider.value = floatValue;   
                    }
                }
                reader.Close();
                reader.Dispose();
            }
        }
    }
    public void LoadGame() => SceneManager.LoadScene("Main");
    public void Quit() => Application.Quit();
    public void Options() => optionsMenu.SetActive(true);
    public void CloseOptions() => optionsMenu.SetActive(false);
    public void Controls() => controlsMenu.SetActive(true);
    public void CloseControls() => controlsMenu.SetActive(false);
    public void PlayButtonPressSound() => audioManager.Play("ButtonPress");
    public void SetVolume()
    {
        foreach (Sound s in audioManager.sounds)
        {
            s.source.volume = sfxSlider.value;
        }
        string filePath = @"C:\Users\sidc2\Umbra_ testing\Assets\_Scripts\StateManagers\volume.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(sfxSlider.value);
            writer.Close();
            writer.Dispose();
        }
    }

    public void ToggleFullScreen()
    {
        if (fullscreenToggle.isOn)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;
    }
}
