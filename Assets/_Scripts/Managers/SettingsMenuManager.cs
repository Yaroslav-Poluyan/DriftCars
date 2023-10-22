using _Scripts.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Managers
{
    public class SettingsMenuManager : MonoBehaviour
    {
        [SerializeField] private Slider _effectsSlider;
        [SerializeField] private Slider _musicSlider;
        [Inject] private AudioManager _audioManager;

        private void Start()
        {
            _effectsSlider.onValueChanged.AddListener(EffectsChanged);
            _musicSlider.onValueChanged.AddListener(MusicChanged);
            _effectsSlider.value = _audioManager.GetEffectsValue();
            _musicSlider.value = _audioManager.GetMusicValue();
        }

        private void MusicChanged(float value)
        {
            _audioManager.SetMusicState(value);
        }

        private void EffectsChanged(float value)
        {
            _audioManager.SetEffectsState(value);
        }
    }
}