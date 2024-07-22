using System;
using System.Collections;
using System.Collections.Generic;
using Components.UI;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class RetryMainBTN : UIBTN
{
    protected override void OnClick()
    {
        LoadMainScene();
        // LoadMainScene Methodu olmadan da calışır zaten Invoke edilip UnityAction sağlanıyor.
        MainUIEvents.RetryBTN?.Invoke();
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("Main");
    }
    
}

