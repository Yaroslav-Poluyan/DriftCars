using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;

        public void SetMusicState(float value)
        {
            _audioMixer.SetFloat("MusicVolume", value);
        }

        public void SetEffectsState(float value)
        {
            _audioMixer.SetFloat("EffectsVolume", value);
        }
    }
}