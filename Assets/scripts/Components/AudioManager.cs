using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  [SerializeField] private AudioSource _musicSource;
  [SerializeField] private AudioSource _SFXSource;
  
  private void Start()
  {
    _musicSource.Play();
  }
  
  

}

