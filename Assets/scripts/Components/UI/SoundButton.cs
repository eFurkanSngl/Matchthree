using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SoundButton :MonoBehaviour
{
    [SerializeField] private AudioSource music;

    public void OnMusic()
    {
        music.Play();
    }

    public void StopMusic()
    {
        music.Stop();
    }

 
}
