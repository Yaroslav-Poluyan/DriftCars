using _Scripts.Audio;
using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class AudioManagerInjection : MonoInstaller
    {
        [SerializeField] private AudioManager _audioManager;
        public override void InstallBindings()
        {
            Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle().NonLazy();
        }
    }
}