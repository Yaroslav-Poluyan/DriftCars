using System;
using _Scripts.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Network
{
    public class PhotonConnectionManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField _playerNameInputField;
        [SerializeField] private TMP_InputField _createRoomNameInputField;
        [SerializeField] private TMP_InputField _joinRoomNameInputField;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _joinRoomButton;
        [Inject] private ScenesManager _scenesManager;

        private void Start()
        {
            _createRoomButton.onClick.AddListener(CreateRoom);
            _joinRoomButton.onClick.AddListener(JoinRoom);
        }

        private void CreateRoom()
        {
            PhotonNetwork.NickName = _playerNameInputField.text;
            var roomName = _createRoomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("Room name is empty.");
            }
            else
            {
                var roomOptions = new RoomOptions {MaxPlayers = 4}; // Change max players as per requirement
                PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
                _scenesManager.LoadScene(ScenesManager.SceneType.Lobby);
                Debug.Log("Room created: " + roomName);
            }
        }

        private void JoinRoom()
        {
            PhotonNetwork.NickName = _playerNameInputField.text;
            var roomName = _joinRoomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("Room name is empty.");
            }
            else
            {
                PhotonNetwork.JoinRoom(roomName);
                _scenesManager.LoadScene(ScenesManager.SceneType.Lobby);
                Debug.Log("Room joined: " + roomName);
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"Disconnected due to {cause}");
        }
    }
}