using Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Components.UI
{
    public class MainExitBTN:UIBTN
    {
        [SerializeField] Button extBtn;

        public void Start()
        {
            if (extBtn != null)
            {
                extBtn.onClick.AddListener(OnClick);
            }
        }
        protected override void OnClick()
        {
            MainUIEvents.ExitBTN?.Invoke();
            SceneManager.LoadScene("MainMenu");
        }

      
    }
}

