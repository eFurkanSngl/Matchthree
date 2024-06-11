using Events;
using UnityEngine.SceneManagement;
using Zenject;
using Envar;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        private ProjectEvents _projectEvents;
        private InputEvents _inputEvents;
        

        public override void InstallBindings()   //Zenject ile beraber gelir kullanmak zorunludur.
        {
            _projectEvents = new ProjectEvents();
            Container.BindInstance(_projectEvents).AsSingle();
            
            _inputEvents = new InputEvents();
            Container.BindInstance(_inputEvents).AsSingle(); // burada container e tekli olarak ekledik

           
        }

        private void Awake()
        {
            RegisterEvents();
        }

        public override void Start()
        {
            _projectEvents.ProjectStarted?.Invoke();
        }

        private static void LoadScene(string sceneName) {SceneManager.LoadScene(sceneName);}

        private void RegisterEvents()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene loadedScene, LoadSceneMode arg1)
        {
            if(loadedScene.name == EnVar.LoginSceneName)
            {
                LoadScene(EnVar.MainSceneName);
            }
        }
    }
}
  