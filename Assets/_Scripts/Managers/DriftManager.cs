using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers
{
    public class DriftManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _driftPointsText;
        private readonly Dictionary<int, float> _playersDriftPoints = new();
        private PhotonView _photonView;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            var playersList = PhotonNetwork.PlayerList;
            foreach (var player in playersList)
            {
                _playersDriftPoints.Add(player.ActorNumber, 0);
            }
        }

        [PunRPC]
        public void RPC_AddDriftPoints(int playerId, float driftPoints)
        {
            if (_playersDriftPoints.ContainsKey(playerId))
            {
                _playersDriftPoints[playerId] += driftPoints;
            }
            else // если не был зарегистрирован
            {
                _playersDriftPoints.Add(playerId, driftPoints);
            }

            UpdateDriftPointsText();
        }

        public void AddDriftPoints(int playerId, float driftPoints)
        {
            _photonView.RPC(nameof(RPC_AddDriftPoints), RpcTarget.All, playerId, driftPoints);
        }

        private void UpdateDriftPointsText()
        {
            _driftPointsText.text = "DRIFT POINTS:\n";
            foreach (var (id, value) in _playersDriftPoints)
            {
                _driftPointsText.text += $"Player {id}: {value}\n";
            }
        }
    }
}