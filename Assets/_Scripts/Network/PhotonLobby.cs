using _Scripts.SceneManagement;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts.Network
{
    public class PhotonLobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private TextMeshProUGUI _playerListText;
        [SerializeField] private UnityEngine.UI.Button _leaveRoomButton;
        [SerializeField] private UnityEngine.UI.Button _startGameButton;
        [Inject] private ScenesManager _scenesManager;

        private void Start()
        {
            _leaveRoomButton.onClick.AddListener(LeaveRoom);
            _startGameButton.onClick.AddListener(StartGame);
            RefreshInfo();
        }

        private void CheckForButtons()
        {
            _startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnJoinedRoom()
        {
            RefreshInfo();
            CheckForButtons();
        }

        public override void OnJoinedLobby()
        {
            RefreshInfo();
            CheckForButtons();
        }

        public override void OnLeftLobby()
        {
            RefreshInfo();
            CheckForButtons();
        }

        public override void OnLeftRoom()
        {
            RefreshInfo();
            CheckForButtons();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            RefreshInfo();
            CheckForButtons();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RefreshInfo();
            CheckForButtons();
        }

        private void RefreshInfo()
        {
            if (PhotonNetwork.CurrentRoom != null) _roomNameText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name;
            _playerListText.text = "";
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.IsMasterClient)
                {
                    _playerListText.text += "Host: ";
                }

                if (player.IsLocal)
                {
                    _playerListText.text += "You: ";
                }

                _playerListText.text += player.NickName + "\n";
            }
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            _scenesManager.LoadScene(ScenesManager.SceneType.Connect);
        }

        private void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(_scenesManager.GameScene);
            }
        }
    }
}