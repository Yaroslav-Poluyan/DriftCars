using _Scripts.SDK;
using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class SDKInjections : MonoInstaller
    {
        [SerializeField] private IronSourceManager _ironSourceManager;

        public override void InstallBindings()
        {
            Container.Bind<IronSourceManager>().FromInstance(_ironSourceManager).AsSingle().NonLazy();
        }
    }
}