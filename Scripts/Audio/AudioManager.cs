using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Slider sfxSlider;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        string filePath = @"C:\Users\sidc2\Umbra_ testing\Assets\_Scripts\StateManagers\volume.txt";
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine();
                if (float.TryParse(line, out float floatValue))
                {
                    foreach (Sound s in sounds)
                    {
                        if (s.name is "Theme" or "ButtonPress")
                            s.source.volume = sfxSlider.value - .05f;
                        else
                            s.source.volume = sfxSlider.value;
                        sfxSlider.value = floatValue;   
                    }
                }
                else
                {
                    Console.WriteLine("Invalid float value in the file.");
                }
                reader.Close();
                reader.Dispose();
            }
        }
        else
        {
            Console.WriteLine("File not found.");
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public Sound GetSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s;
    }
}
