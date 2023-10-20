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
            private readonly HashSet<int> _readyPlayers = new();
            private bool _allTrucksCreated = false;
            private List<TruckController> _trucks = new();
            private readonly Dictionary<int, int> truckViewIdToPlayerId = new();

            private void Start()
            {
                if (PhotonNetwork.IsMasterClient)
                {
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
                    var truck = PhotonNetwork.Instantiate(_truckPrefab.name, spawnPoint.position, spawnPoint.rotation);
                    photonView.RPC(nameof(InitializeTruck), RpcTarget.AllBuffered, truck.GetPhotonView().ViewID,
                        player.ActorNumber);
                    truckViewIdToPlayerId.Add(truck.GetPhotonView().ViewID, player.ActorNumber);
                    i++;
                }
            }

            [PunRPC]
            private void InitializeTruck(int truckViewID, int playerID)
            {
                var truck = PhotonView.Find(truckViewID).gameObject;
                var controller = truck.GetComponent<TruckController>();
                _trucks.Add(controller);
                controller.Initialize(_driftManager, _inputManager);
                controller.SetPlayerId(playerID);
                if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
                {
                    _cameraController.AssignCameraToTruck(controller);
                }
                
                // Inform the server about truck creation from all clients
                photonView.RPC(nameof(InformServerAboutTruckCreation), RpcTarget.MasterClient, playerID, truckViewID);
            }

            [PunRPC]
            private void InformServerAboutTruckCreation(int playerID, int truckViewID)
            {
                _readyPlayers.Add(playerID);
                truckViewIdToPlayerId.Remove(truckViewID);

                if (truckViewIdToPlayerId.Count == 0)
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