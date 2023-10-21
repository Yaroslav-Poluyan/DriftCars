using System;
using _Scripts.Managers;
using _Scripts.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.UI
{
    public class FinishPanel : MonoBehaviour
    {
        public static Action OnFinishPanelOpened;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _getMoney;
        [SerializeField] private Button _doubleMoneyViaAd;
        [Inject] private DriftManager _driftManager;
        [Inject] private PlayerResourcesManager _playerResourcesManager;
        [Inject] private ScenesManager _sceneLoader;

        private void Start()
        {
            _getMoney.onClick.AddListener(GetMoneyButtonPressedHandler);
            _doubleMoneyViaAd.onClick.AddListener(WatchAd);
            IronSourceEvents.onRewardedVideoAdRewardedEvent += VideoAdWatchedHandler;
        }

        private void GetMoneyButtonPressedHandler()
        {
            var localScore = _driftManager.GetLocalPlayerScore();
            _playerResourcesManager.AddMoney(localScore);
            _playerResourcesManager.AddDriftScore(localScore);
            _sceneLoader.LoadScene(ScenesManager.SceneType.MainMenu);
        }

        private static void WatchAd()
        {
            Debug.Log("unity-script: ShowRewardedVideoButtonClicked");
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
            }
        }

        private void VideoAdWatchedHandler(IronSourcePlacement ironSourcePlacement)
        {
            var localScore = _driftManager.GetLocalPlayerScore();
            _playerResourcesManager.AddMoney(localScore * 2);
            _playerResourcesManager.AddDriftScore(localScore);
            _sceneLoader.LoadScene(ScenesManager.SceneType.MainMenu);
        }

        public void Open()
        {
            OnFinishPanelOpened?.Invoke();
            gameObject.SetActive(true);
            var localScore = _driftManager.GetLocalPlayerScore();
            _scoreText.text = $"Your score: {localScore}";
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}