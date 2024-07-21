using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSound : MonoBehaviour
{
   public AudioSource clickSound;
   
   public void playThisSFX()
   {
      clickSound.Play();
   }
}
