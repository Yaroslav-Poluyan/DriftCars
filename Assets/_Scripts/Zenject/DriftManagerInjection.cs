using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class DriftManagerInjection : MonoInstaller
    {
        [SerializeField] private Managers.DriftManager _driftManager;
        public override void InstallBindings()
        {
            Container.Bind<Managers.DriftManager>().FromInstance(_driftManager).AsSingle().NonLazy();
        }
    }
}