using _Scripts.IAPS;
using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    // ReSharper disable once InconsistentNaming
    public class IAPSInjection : MonoInstaller
    {
        [SerializeField] private IAPManager _iapManager;
        public override void InstallBindings()
        {
            Container.Bind<IAPManager>().FromInstance(_iapManager).AsSingle();
        }
        
    }
}