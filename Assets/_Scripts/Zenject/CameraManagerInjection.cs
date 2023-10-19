using _Scripts.Cameras;
using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class CameraManagerInjection : MonoInstaller
    {
        [SerializeField] private CameraManager _cameraManager;

        public override void InstallBindings()
        {
            Container.Bind<CameraManager>().FromInstance(_cameraManager).AsSingle().NonLazy();
        }
    }
}