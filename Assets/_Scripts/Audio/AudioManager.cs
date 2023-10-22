using System;
using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;

        private void Start()
        {
            _audioMixer.SetFloat("MusicVolume", ES3.Load<float>("MusicVolume", 0));
            _audioMixer.SetFloat("EffectsVolume", ES3.Load<float>("EffectsVolume", 0));
        }

        public void SetMusicState(float value)
        {
            _audioMixer.SetFloat("MusicVolume", value);
            ES3.Save("MusicVolume", value);
        }

        public void SetEffectsState(float value)
        {
            _audioMixer.SetFloat("EffectsVolume", value);
            ES3.Save("EffectsVolume", value);
        }

        public float GetMusicValue()
        {
            _audioMixer.GetFloat("MusicVolume", out var value);
            return value;
        }

        public float GetEffectsValue()
        {
            _audioMixer.GetFloat("EffectsVolume", out var value);
            return value;
        }
    }
}