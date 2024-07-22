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
            // LoadMainScene Methodu olmadan da calışır zaten Invoke edilip UnityAction sağlanıyor.
            MainMenuEvents.NewGameBTN?.Invoke();
        }

        private void LoadMainScene()
        {
            SceneManager.LoadScene("Main");
        }
    }
}