using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class ProjectInstaller : MonoInstaller<ProjectInstaller>  //Zenject Klassıdır.MonoInstallerın BuildInstaller Kullanma zorunluluğu vardır.
{
        private ProjectEvents _projectEvents;
        public override void InstallBindings()  // Kullanmak zorunludur.Override etmezsek Hata verir.
        {
            //Burada yeni projectEvents atıyoruz ve onu Containere atıyoruz. Assingle İle Single yapıyoruz
            _projectEvents = new ();
            Container.BindInstance(_projectEvents).AsSingle();
        }

        private void Awake()
        {
            RegisterEvents();
        }
        public override void Start()
        {   
            _projectEvents.ProjectStarted?.Invoke();
        }

        private static void LoadScene(string sceneName)
 
        {
            SceneManager.LoadScene(sceneName);
         }
        private void RegisterEvents()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene LoadedScene, LoadSceneMode arg1)
        {
            if (LoadedScene.name == "Login")
            {
             LoadScene("Main");   
            }
        }
}
  