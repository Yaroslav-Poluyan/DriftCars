using System;
using System.Collections;
using _Scripts.UI;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers
{
    public class TimerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private FinishPanel _finishPanel;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _startTimerText;
        [SerializeField] private double _beforeStartTimer = 3f;
        [SerializeField] private double _gameTimer = 20f;
        private PhotonView _photonView;
        private Coroutine _timerCoroutine;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(StartTimer(_startTimerText, _beforeStartTimer, "GO!", TimerStartInitialization,
                    TimerStartCompletion));
            }
        }

        private void TimerStartInitialization()
        {
            Pause.IsPaused = true;
            _photonView.RPC(nameof(RPCTimerStart), RpcTarget.All);
        }

        [PunRPC]
        private void RPCTimerStart()
        {
            Pause.IsPaused = true;
            StartCoroutine(StartTimer(_startTimerText, _beforeStartTimer, "GO!", null, RPCTimerStartCompleted));
        }

        private void TimerStartCompletion()
        {
            Pause.IsPaused = false;
            _photonView.RPC(nameof(RPCTimerStartCompleted), RpcTarget.All);
        }

        [PunRPC]
        private void RPCTimerStartCompleted()
        {
            Pause.IsPaused = false;
            StartCoroutine(StartTimer(_timerText, _gameTimer, "end", GameTimerStartInitialization,
                GameTimerStartCompletion));
        }

        private void GameTimerStartInitialization()
        {
            print("Game started");
            _photonView.RPC(nameof(RPCGameTimerStart), RpcTarget.All);
        }

        [PunRPC]
        private void RPCGameTimerStart()
        {
            print("Game started");
        }

        private void GameTimerStartCompletion()
        {
            print("Game ended");
            _photonView.RPC(nameof(RPCGameTimerComplete), RpcTarget.All);
        }

        [PunRPC]
        private void RPCGameTimerComplete()
        {
            print("Game ended");
            _finishPanel.Open();
        }

        private static IEnumerator StartTimer(TextMeshProUGUI timerText, double time, string onendMessage, Action onTimerStart,
            Action onTimerEnd)
        {
            var endTime = PhotonNetwork.Time + time;

            timerText.gameObject.SetActive(true);
            onTimerStart?.Invoke();

            while (PhotonNetwork.Time < endTime)
            {
                var timer = endTime - PhotonNetwork.Time;
                timerText.text = timer.ToString("F2");
                yield return null;
            }

            onTimerEnd?.Invoke();
            timerText.text = onendMessage;
            yield return new WaitForSeconds(1f);
            timerText.gameObject.SetActive(false);
        }
    }
}