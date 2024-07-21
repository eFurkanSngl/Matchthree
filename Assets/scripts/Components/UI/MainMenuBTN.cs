using System;
using System.Collections;
using System.Collections.Generic;
using Components.UI;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class MainMenuBTN : UIBTN
{
    protected override void OnClick()
    {
        LoadMainScene();
        MainUIEvents.MainMenuBTN?.Invoke();
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
