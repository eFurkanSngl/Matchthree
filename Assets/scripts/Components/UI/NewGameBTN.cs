using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Components.UI
{
    public class NewGameBTN : UIBTN
    {
        protected override void OnClick()
        {
            LoadMainScene();
            MainMenuEvents.NewGameBTN?.Invoke();
        }

        private void LoadMainScene()
        {
            SceneManager.LoadScene("Main");
        }
    }
}