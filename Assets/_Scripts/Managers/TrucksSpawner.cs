using System.Collections.Generic;
using _Scripts.Cameras;
using _Scripts.Truck;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace _Scripts.Managers
{
    public class TrucksSpawner : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TruckController _truckPrefab;
        [SerializeField] private List<Transform> _spawnPoints = new();
        [Inject] private CameraManager _cameraController;
        [Inject] private DriftManager _driftManager;
        [Inject] private InputManager.InputManager _inputManager;

        private void Start()
        {
            SpawnTrucks();
        }

        private void SpawnTrucks()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var players = PhotonNetwork.PlayerList;
            var i = 0;
            foreach (var player in players)
            {
                var spawnPoint = _spawnPoints[i];
                var truck = PhotonNetwork.Instantiate(_truckPrefab.name, spawnPoint.position, spawnPoint.rotation);

                // Call the RPC on all clients
                photonView.RPC(nameof(InitializeTruck), RpcTarget.AllBuffered, truck.GetPhotonView().ViewID,
                    player.ActorNumber);
                i++;
            }
        }

        [PunRPC]
        private void InitializeTruck(int truckViewID, int playerID)
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
            var truck = PhotonView.Find(truckViewID).gameObject;
            var controller = truck.GetComponent<TruckController>();
            controller.Initialize(_driftManager, _inputManager);
            controller.SetPlayerId(playerID);
            if (player.IsLocal)
            {
                _cameraController.AssignCameraToTruck(controller);
            }
        }
    }
}