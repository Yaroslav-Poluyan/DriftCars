using System;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Interactables.Collectables
{
    public abstract class CollectableBase : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _onCollectParticles;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnTriggerHandler();
            }
        }

        private void OnTriggerHandler()
        {
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => Destroy(gameObject));
            PlayParticles();
            OnCollectedAction();
        }

        private void PlayParticles()
        {
            foreach (var particle in _onCollectParticles)
            {
                particle.Play();
            }
        }

        protected abstract void OnCollectedAction();
    }
}