using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.UI.Leaderboard
{
    public class LeaderboardManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private LeaderboardBlock _blockPrefab;
        [SerializeField] private Transform _content;

        private void RefreshInfo()
        {
            var data = GetPlayerScores();
            ClearLeaderboard();
            CreateLeaderboard(data);
        }

        public override void OnJoinedRoom()
        {
            RefreshInfo();
        }

        public override void OnJoinedLobby()
        {
            RefreshInfo();
        }

        public override void OnLeftLobby()
        {
            RefreshInfo();
        }

        public override void OnLeftRoom()
        {
            RefreshInfo();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            RefreshInfo();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RefreshInfo();
        }

        private static List<(string player, string score)> GetPlayerScores()
        {
            var playerList = PhotonNetwork.PlayerList;
            var playerScores = new List<(string player, string score)>();

            foreach (var player in playerList)
            {
                if (player.CustomProperties.TryGetValue("score", out var property))
                {
                    var score = property.ToString();
                    playerScores.Add((player.NickName, score));
                }
            }

            return playerScores;
        }

        private void ClearLeaderboard()
        {
            foreach (Transform child in _content)
            {
                Destroy(child.gameObject);
            }
        }

        private void CreateLeaderboard(List<(string player, string score)> valueTuples)
        {
            foreach (var (player, score) in valueTuples)
            {
                CreateNewPanel(player, score);
            }
        }

        private void CreateNewPanel(string playerName, string score)
        {
            var block = Instantiate(_blockPrefab, _content);
            block.Init($"{playerName} - {score}");
        }
    }
}