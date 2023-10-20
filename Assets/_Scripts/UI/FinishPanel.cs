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
            /*if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("Rewarded video is not available yet");
            }*/
        }

        public void Open()
        {
            var localScore = _driftManager.GetLocalPlayerScore();
            _scoreText.text = $"Your score: {localScore}";
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}