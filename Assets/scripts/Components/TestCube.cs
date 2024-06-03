using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using Zenject;

public class TestCube : MonoBehaviour
{
    [Inject] private ProjectEvents ProjectEvents{get; set; }

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnRegisterEvents();
    }
private void RegisterEvents()
    {
        ProjectEvents.ProjectStarted += OnProjectInstall;
    }
 private void OnProjectInstall()
    {
        Debug.LogWarning("Var");
    }
    private void UnRegisterEvents()
    {
        ProjectEvents.ProjectStarted -= OnProjectInstall;
    }
    
}

