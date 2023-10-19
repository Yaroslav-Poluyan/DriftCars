using UnityEngine;
using Zenject;

namespace _Scripts.Zenject
{
    public class InputManagerInjection : MonoInstaller
    {
        [SerializeField] private InputManager.InputManager _inputManager;
        
        public override void InstallBindings()
        {
            Container.Bind<InputManager.InputManager>().FromInstance(_inputManager).AsSingle().NonLazy();
        }
    }
}