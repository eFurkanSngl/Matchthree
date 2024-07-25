using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaded : MonoBehaviour
{
    public void LoadedScene()
    {
        SceneManager.LoadScene("Level2");
    }
}   