using System.Collections.Generic;
using _Scripts.Cameras;
using _Scripts.Garage;
using _Scripts.Truck;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace _Scripts.Managers
{
    public class TrucksSpawner : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<Transform> _spawnPoints = new();
        [Inject] private CameraManager _cameraController;
        [Inject] private DriftManager _driftManager;
        [Inject] private InputManager.InputManager _inputManager;
        [Inject] private TrucksPrefabsManager _trucksPrefabsManager;
        private readonly HashSet<int> _readyPlayers = new();
        private bool _allTrucksCreated = false;
        private List<TruckController> _trucks = new();
        private readonly Dictionary<int, int> _truckViewIdToPlayerId = new();
        private readonly Dictionary<int, int> _playerIdToTruckPrefabId = new();

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AskForTruckPresets();
            }
        }

        private void AskForTruckPresets()
        {
            /*var currentTruckFromSave = ES3.Load("CurrentTruck", 0);
            _playerIdToTruckPrefabId.Add(PhotonNetwork.LocalPlayer.ActorNumber, currentTruckFromSave);*/
            photonView.RPC(nameof(SendTruckIDToMaster), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void SendTruckIDToMaster()
        {
            var currentTruckFromSave = ES3.Load("CurrentTruck", 0);
            photonView.RPC(nameof(ReceiveTruckIDFromClient), RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.ActorNumber,
                currentTruckFromSave);
        }

        [PunRPC]
        private void ReceiveTruckIDFromClient(int clientViewID, int truckPrefabID)
        {
            _playerIdToTruckPrefabId.TryAdd(clientViewID, truckPrefabID);
            if (_playerIdToTruckPrefabId.Count == PhotonNetwork.PlayerList.Length)
            {
                print("All trucks received, spawning...");
                SpawnTrucks();
            }
        }

        private void SpawnTrucks()
        {
            var players = PhotonNetwork.PlayerList;
            var i = 0;
            foreach (var player in players)
            {
                var spawnPoint = _spawnPoints[i];
                var truckPreset =
                    _trucksPrefabsManager.GetPlayerTruckPresetData(_playerIdToTruckPrefabId[player.ActorNumber]);
                var truck = PhotonNetwork.Instantiate(truckPreset.TruckPrefab.name, spawnPoint.position,
                    spawnPoint.rotation);
                photonView.RPC(nameof(InitializeTruck), RpcTarget.AllBuffered, truck.GetPhotonView().ViewID,
                    player.ActorNumber, truckPreset.PrefabID);
                //не забыть newTruck.TruckPrefabId = truckPreset.PrefabID;
                _truckViewIdToPlayerId.Add(truck.GetPhotonView().ViewID, player.ActorNumber);
                i++;
            }
        }

        [PunRPC]
        private void InitializeTruck(int truckViewID, int playerID, int truckPrefabID)
        {
            var truck = PhotonView.Find(truckViewID).gameObject;
            var controller = truck.GetComponent<TruckController>();
            controller.TruckPrefabId = truckPrefabID;
            _trucks.Add(controller);
            controller.Initialize(_driftManager, _inputManager);
            controller.SetPlayerId(playerID);
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
            {
                _cameraController.AssignCameraToTruck(controller);
            }
            else
            {
                controller.DisableAllPhysics();
            }

            // Inform the server about truck creation from all clients
            photonView.RPC(nameof(InformServerAboutTruckCreation), RpcTarget.MasterClient, playerID, truckViewID);
        }

        [PunRPC]
        private void InformServerAboutTruckCreation(int playerID, int truckViewID)
        {
            _readyPlayers.Add(playerID);
            _truckViewIdToPlayerId.Remove(truckViewID);

            if (_truckViewIdToPlayerId.Count == 0)
            {
                // All players' trucks are created, send an RPC to synchronize trucks
                photonView.RPC(nameof(SynchronizeTrucks), RpcTarget.All);
            }
        }

        [PunRPC]
        private void SynchronizeTrucks()
        {
            foreach (var truck in _trucks)
            {
                truck.UpgradeManager.Sync();
            }
        }
    }
}