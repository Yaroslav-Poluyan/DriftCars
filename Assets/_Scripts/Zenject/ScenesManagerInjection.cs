using _Scripts.SceneManagement;
using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class ScenesManagerInjection : MonoInstaller
    {
        [SerializeField] private ScenesManager _scenesManager;

        public override void InstallBindings()
        {
            Container.Bind<ScenesManager>().FromInstance(_scenesManager).AsSingle().NonLazy();
        }
    }
}