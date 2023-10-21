using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.SceneManagement
{
    public class ScenesManager : MonoBehaviour
    {
        private static SceneReference _currentScene;
        [field: SerializeField] public SceneReference InitialScene { get; private set; }
        [field: SerializeField] public SceneReference ConnectScene { get; private set; }
        [field: SerializeField] public SceneReference LobbyScene { get; private set; }
        [field: SerializeField] public SceneReference MainMenuScene { get; private set; }
        [field: SerializeField] public SceneReference GarageScene { get; private set; }
        [field: SerializeField] public SceneReference StoreScene { get; private set; }
        [field: SerializeField] public List<SceneReference> Levels { get; private set; }

        public enum SceneType
        {
            Initial = 0,
            Connect = 1,
            Lobby = 2,
            MainMenu = 4,
            Garage = 5,
            Store = 6,
        }

        public void LoadScene(SceneType sceneType)
        {
            _currentScene = sceneType switch
            {
                SceneType.Initial => InitialScene,
                SceneType.Connect => ConnectScene,
                SceneType.Lobby => LobbyScene,
                SceneType.MainMenu => MainMenuScene,
                SceneType.Garage => GarageScene,
                SceneType.Store => StoreScene,
                _ => _currentScene
            };
            SceneManager.LoadScene(_currentScene.ScenePath);
        }

        public void LoadLevel(int levelIdx)
        {
            _currentScene = Levels[levelIdx];
            SceneManager.LoadScene(_currentScene.ScenePath);
        }

        public string GetLevelPath(int levelIdx)
        {
            return Levels[levelIdx].ScenePath;
        }
    }
}