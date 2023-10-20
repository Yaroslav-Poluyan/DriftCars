using _Scripts.Garage;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class GarageInjections : MonoInstaller
    {
        [SerializeField] private TruckUpgradeUI _truckUpgradeUi;
        [SerializeField] private GarageManager _garageManager;
        [SerializeField] private GarageCameraController _garageCameraController;
        
        public override void InstallBindings()
        {
            Container.Bind<TruckUpgradeUI>().FromInstance(_truckUpgradeUi).AsSingle().NonLazy();
            Container.Bind<GarageManager>().FromInstance(_garageManager).AsSingle().NonLazy();
            Container.Bind<GarageCameraController>().FromInstance(_garageCameraController).AsSingle().NonLazy();
        }
    }
}