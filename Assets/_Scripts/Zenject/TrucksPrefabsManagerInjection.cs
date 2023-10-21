using _Scripts.Garage;
using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class TrucksPrefabsManagerInjection : MonoInstaller
    {
        [SerializeField] private TrucksPrefabsManager _truckPrefabsManager;
        
        public override void InstallBindings()
        {
            Container.Bind<TrucksPrefabsManager>().FromInstance(_truckPrefabsManager).AsSingle().NonLazy();
        }
    }
}