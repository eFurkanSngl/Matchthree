using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileSoundEffect : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        // başlangıçta Compenenti AudioSource Olanı alıyorum
    }

    private void OnMouseDown()
    {
        if (_audioSource !=null && _audioSource.clip != null)
            // eğer AudioSource ya da clip boş değilse
        {
            _audioSource.PlayOneShot(_audioSource.clip);
            // calıştırıyorum
        }
    }
}
