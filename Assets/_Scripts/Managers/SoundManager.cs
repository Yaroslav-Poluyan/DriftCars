using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Scripts.Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private float _defaultSoundVolume = 0.5f;
        [SerializeField] private float _defaultMusicVolume = 0.5f;
        [field: SerializeField] public AudioClip WheelSkid { get; private set; }
        [field: SerializeField] public AudioClip WheelsOnDirt { get; private set; }
        [field: SerializeField] public AudioClip WheelsOnWater { get; private set; }
        [field: SerializeField] public AudioClip ObstacleCollision { get; private set; }
        [field: SerializeField] public AudioClip Idling { get; private set; }
        [field: SerializeField] public AudioClip LowMediumThrottle { get; private set; }
        [field: SerializeField] public AudioClip HighThrottle { get; private set; }
        [field: SerializeField] public AudioClip GasPressed { get; private set; }
        [field: SerializeField] public List<AudioClip> TruckExplosion { get; private set; }
        [field: SerializeField] public AudioClip WheelBreak { get; private set; }
        [field: SerializeField] public AudioClip WheelGoingAway { get; private set; }
        [field: SerializeField] public AudioClip Flying { get; private set; }
        [field: SerializeField] public AudioClip Landing { get; private set; }

        public void SetMusicActiveState(bool state)
        {
            _audioMixer.SetFloat("MusicVolume", state ? _defaultMusicVolume : -80);
        }

        public void SetEffectsSoundActiveState(bool state)
        {
            _audioMixer.SetFloat("EffectsVolume", state ? _defaultSoundVolume : -80);
        }
    }
}