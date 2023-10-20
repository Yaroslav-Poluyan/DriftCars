using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers
{
    public class TimerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _startTimerText;
        [SerializeField] private float _beforeStartTimer = 3f;
        [SerializeField] private float _gameTimer = 120f;
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
            _photonView.RPC(nameof(RPCTimerStart), RpcTarget.Others);
        }

        [PunRPC]
        private void RPCTimerStart()
        {
            Pause.IsPaused = true;
        }

        private void TimerStartCompletion()
        {
            Pause.IsPaused = false;
            _photonView.RPC(nameof(RPCTimerStartCompleted), RpcTarget.Others);
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(StartTimer(_timerText, _gameTimer, "end", GameTimerStartInitialization,
                    GameTimerStartCompletion));
            }
        }

        [PunRPC]
        private void RPCTimerStartCompleted()
        {
            Pause.IsPaused = false;
        }

        private void GameTimerStartInitialization()
        {
            print("Game started");
            _photonView.RPC(nameof(RPCGameTimerStart), RpcTarget.Others);
        }

        [PunRPC]
        private void RPCGameTimerStart()
        {
            print("Game started");
        }

        private void GameTimerStartCompletion()
        {
            print("Game ended");
            _photonView.RPC(nameof(RPCGameTimerComplete), RpcTarget.Others);
        }

        [PunRPC]
        private void RPCGameTimerComplete()
        {
            print("Game ended");
        }

        private IEnumerator StartTimer(TextMeshProUGUI timerText, float time, string onendMessage, Action onTimerStart,
            Action onTimerEnd)
        {
            var timer = time;
            timerText.gameObject.SetActive(true);
            onTimerStart?.Invoke();
            while (timer > 0)
            {
                timer -= Time.deltaTime;
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