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
        [SerializeField] private float _beforeStartTimer = 3f;
        [SerializeField] private float _gameTimer = 120f;
        private PhotonView _photonView;
        private Coroutine _timerCoroutine;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            if (PhotonNetwork.IsMasterClient)
            {
                _photonView.RPC(nameof(StartInLaunchTimerRPC), RpcTarget.All, _beforeStartTimer);
            }
        }

        [PunRPC]
        private void StartInLaunchTimerRPC(float time)
        {
            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
            _timerCoroutine = StartCoroutine(StartTimer(time, "GO!", OnTimerStart, OnTimerEnd));
            return;

            void OnTimerEnd()
            {
                Pause.IsPaused = false;
                if (PhotonNetwork.IsMasterClient) _photonView.RPC(nameof(StartGameTimerRPC), RpcTarget.All, _gameTimer);
            }

            void OnTimerStart()
            {
                Pause.IsPaused = true;
            }
        }

        [PunRPC]
        private void StartGameTimerRPC(float time)
        {
            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
            _timerCoroutine = StartCoroutine(StartTimer(time, "end", OnTimerStart, OnTimerEnd));
            return;

            void OnTimerStart()
            {
                print("Game started");
            }

            void OnTimerEnd()
            {
                print("Game ended");
            }
        }

        private IEnumerator StartTimer(float time, string onendMessage, Action onTimerStart, Action onTimerEnd)
        {
            var timer = time;
            _timerText.gameObject.SetActive(true);
            onTimerStart?.Invoke();
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                _timerText.text = timer.ToString("F2");
                yield return null;
            }

            onTimerEnd?.Invoke();
            _timerText.text = onendMessage;
            yield return new WaitForSeconds(1f);
            _timerText.gameObject.SetActive(false);
        }
    }
}