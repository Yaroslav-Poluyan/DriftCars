using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Audio
{
    public class BackgroundMusic : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<AudioClip> _backgroundSound = new();
        private Coroutine _playCoroutine;

        private void Awake()
        {
            if (_backgroundSound.Count > 0)
                _playCoroutine ??= StartCoroutine(PlayCoroutine());
        }

        private IEnumerator PlayCoroutine()
        {
            yield return new WaitForSeconds(.5f);
            while (this)
            {
                _audioSource.clip = _backgroundSound[Random.Range(0, _backgroundSound.Count)];
                _audioSource.Play();
                yield return new WaitForSeconds(_audioSource.clip.length);
            }
        }
    }
}