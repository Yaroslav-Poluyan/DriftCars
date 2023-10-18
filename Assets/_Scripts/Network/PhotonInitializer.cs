using _Scripts.SceneManagement;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace _Scripts.Network
{
    public class PhotonInitializer : MonoBehaviourPunCallbacks
    {
        #region Variables : PhotonSettings

        [SerializeField] private string _region = "eu";
        [SerializeField] private string _gameVersion = "1";
        [Inject] private ScenesManager _scenesManager;

        #endregion

        private void Start()
        {
            PhotonNetwork.GameVersion = _gameVersion;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.ConnectToRegion(_region);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master " + PhotonNetwork.CloudRegion);
            _scenesManager.LoadScene(ScenesManager.SceneType.Connect);
        }
    }
}