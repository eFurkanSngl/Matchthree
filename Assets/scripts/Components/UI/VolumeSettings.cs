using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _myMixer;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private AudioSource _mainMenuAudio;

    private void Start()
    {
        // Mixer'daki mevcut ses seviyesini al ve slider'a uygula
        if (_myMixer.GetFloat("BGsound", out float volume))
        {
            _soundSlider.value = volume;
        }

        // Slider değeri değiştiğinde SetMusicVolume fonksiyonunu çağır
        _soundSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
    }

    public void SetMusicVolume()
    {
        float volume = _soundSlider.value;
        _myMixer.SetFloat("BGsound", volume);
        
        // MainMenuAudio'nun sesini de güncelle
        if (_mainMenuAudio != null)
        {
            _mainMenuAudio.volume = _soundSlider.normalizedValue;
        }
    }
}