using System.Collections;
using UnityEngine;

namespace _Scripts.Truck
{
    public class EngineAudio : MonoBehaviour
    {
        [SerializeField] private TruckController _carController;
        [SerializeField] private AudioSource _runningSound;
        [SerializeField] private float _runningMaxVolume;
        [SerializeField] private float _runningMaxPitch;
        [SerializeField] private AudioSource _reverseSound;
        [SerializeField] private float _reverseMaxVolume;
        [SerializeField] private float _reverseMaxPitch;
        [SerializeField] private AudioSource _idleSound;
        [SerializeField] private float _idleMaxVolume;
        [SerializeField] private float _speedRatio;
        [SerializeField] private float _limiterSound = 1f;
        [SerializeField] private float _limiterFrequency = 3f;
        [SerializeField] private float _limiterEngage = 0.8f;
        [field: SerializeField] public bool IsEngineRunning { get; set; } = false;
        [SerializeField] private AudioSource _startingSound;
        private float _revLimiter;

        // Start is called before the first frame update
        private void Start()
        {
            _idleSound.volume = 0;
            _runningSound.volume = 0;
            _reverseSound.volume = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_carController.IsForceBrake)
            {
                //smoothy stop engine sound
                _runningSound.volume = Mathf.Lerp(_runningSound.volume, 0, Time.deltaTime * 5f);
                _reverseSound.volume = Mathf.Lerp(_reverseSound.volume, 0, Time.deltaTime * 5f);
                _idleSound.volume = Mathf.Lerp(_idleSound.volume, 0, Time.deltaTime * 5f);
                return;
            }
            float speedSign = 0;
            if (_carController)
            {
                speedSign = Mathf.Sign(_carController.GetSpeedRatio());
                _speedRatio = Mathf.Abs(_carController.GetSpeedRatio());
            }

            if (_speedRatio > _limiterEngage)
            {
                _revLimiter = (Mathf.Sin(Time.time * _limiterFrequency) + 1f) * _limiterSound *
                              (_speedRatio - _limiterEngage);
            }

            if (IsEngineRunning)
            {
                _idleSound.volume = Mathf.Lerp(0.1f, _idleMaxVolume, _speedRatio);
                if (speedSign > 0)
                {
                    _reverseSound.volume = 0;
                    _runningSound.volume = Mathf.Lerp(0.3f, _runningMaxVolume, _speedRatio);
                    _runningSound.pitch = Mathf.Lerp(0.3f, _runningMaxPitch, _speedRatio);
                }
                else
                {
                    _runningSound.volume = 0;
                    _reverseSound.volume = Mathf.Lerp(0f, _reverseMaxVolume, _speedRatio);
                    _reverseSound.pitch = Mathf.Lerp(0.2f, _reverseMaxPitch, _speedRatio);
                }
            }
            else
            {
                _idleSound.volume = 0;
                _runningSound.volume = 0;
            }
        }
        public void PlayStartingSound()
        {
            _startingSound.Play();
        }
    }
}