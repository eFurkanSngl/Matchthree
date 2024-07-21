// using Events;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
// using Utils;
//
// namespace Components.UI
// {
//     public class SettingsBTN:UIBTN
//     {
//         private Button _settingsBTN;
//         public GameObject SettingsPanel;
//         public void Start()
//         {
//             if (_settingsBTN != null)
//             {
//                 _settingsBTN.onClick.AddListener(OnClick);
//             }
//
//             if (_settingsBTN !=null)
//             {
//                 _settingsBTN.onClick.AddListener(OnSettingsBtnClick);
//             }
//
//             if (_settingsBTN != null)
//             {
//                 SettingsPanel.SetActive(false);   
//             }
//         }
//
//         protected override void OnClick()
//         {
//             MainUIEvents.SettingsBTN?.Invoke();
//             
//         }
//
//        protected  void OnSettingsBtnClick()
//         {
//             if (_settingsBTN != null)
//             {
//                 SettingsPanel.SetActive(!SettingsPanel.activeSelf);
//                 //  Kapalı settings Paneli  active ediyoruz.
//             }
//         }
//     }
// }