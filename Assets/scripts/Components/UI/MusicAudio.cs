using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAudio : MonoBehaviour
{
    public void MuteHandler(bool mute)
    {
        if (mute)
        {
            AudioListener.volume = 1;
            
            //Eğer düğme işaretliyse tüm sesleri sustur
        }
        else
        {
            AudioListener.volume = 0;
        }
    }
}
