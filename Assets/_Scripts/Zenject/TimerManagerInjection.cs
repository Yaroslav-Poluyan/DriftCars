using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class TimerManagerInjection : MonoInstaller
    {
        [SerializeField] private Managers.TimerManager _timerManager;
        public override void InstallBindings()
        {
            Container.Bind<Managers.TimerManager>().FromInstance(_timerManager).AsSingle().NonLazy();
        }
    }
}