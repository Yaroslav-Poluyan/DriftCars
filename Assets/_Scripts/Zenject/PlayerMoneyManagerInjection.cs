using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class PlayerMoneyManagerInjection : MonoInstaller
    {
        [SerializeField] private Managers.PlayerResourcesManager _playerResourcesManager;

        public override void InstallBindings()
        {
            Container.Bind<Managers.PlayerResourcesManager>().FromInstance(_playerResourcesManager).AsSingle().NonLazy();
        }
    }
}