using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Managers;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Truck
{
    public class TruckUpgradeManager : MonoBehaviour
    {
        public enum Slot
        {
            Spoiler = 0,
            Color = 1
        }

        [Serializable]
        public class Part
        {
            public Slot _slot;
            public string _name;
            public Sprite _partSprite;
            public bool _isInstalled;
            public string _upgradePrefabsPath;
            public Transform _spawnPoint;
            public bool _isBought;
            public float _price;
        }

        [Serializable]
        public class PartsOfTypeContainer
        {
            public Slot _slot;
            public Part[] _parts;
        }

        [SerializeField] private List<PartsOfTypeContainer> _truckUpgrades = new();
        [SerializeField] private List<MeshRenderer> _forColoring = new();
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private TruckController _truckController;
        private PlayerResourcesManager _playerResourcesManager;
        private bool _synced;

        private void Start()
        {
#if UNITY_EDITOR
            //set directory of saved data to Resources folder
            ES3Settings.defaultSettings.path = @"C:\Work\UnityProjects\DriftCars\Assets\Resources\savedata.json";
#else
            ES3Settings.defaultSettings.path = Application.persistentDataPath + "/savedata.json";
#endif
            //Debug.LogError("Save path: " + ES3Settings.defaultSettings.path);
            if (!PhotonNetwork.InRoom) LoadAllUpgrades();
        }

        public void Sync()
        {
            LoadAllUpgrades();
        }

        private void LoadAllUpgrades()
        {
            var updates = Load();
            if (updates != null) UpdateCarState(updates);
        }

        public void Initialize(PlayerResourcesManager playerResourcesManager)
        {
            _playerResourcesManager = playerResourcesManager;
        }

        private void SetNewMaterial(Material mat)
        {
            foreach (var meshRenderer in _forColoring)
            {
                if (meshRenderer.materials.Length > 0)
                {
                    var newMaterials = meshRenderer.materials;
                    newMaterials[0] = mat;
                    meshRenderer.materials = newMaterials;
                }
            }
        }

        private static void SpawnNewPart(Transform part, Transform currentSpawnPoint)
        {
            //delete old part
            if (currentSpawnPoint.childCount > 0) Destroy(currentSpawnPoint.GetChild(0).gameObject);

            Instantiate(part, currentSpawnPoint.position, currentSpawnPoint.rotation, currentSpawnPoint);
        }

        private void ImplementUpgrade(Slot slot, int upgradeIndex)
        {
            List<Part> upgrades;
            var partsOfSlot = _truckUpgrades.FirstOrDefault(x => x._slot == slot)?._parts;
            var part = partsOfSlot?[upgradeIndex];
            if (part != null)
            {
                //set not installed for all parts of this type
                foreach (var partOfType in partsOfSlot) partOfType._isInstalled = false;

                part._isInstalled = true;
            }

            string currentPartPath;
            switch (slot)
            {
                case Slot.Spoiler:
                {
                    upgrades = _truckUpgrades.FirstOrDefault(x => x._slot == slot)?._parts.ToList();
                    currentPartPath = upgrades?[upgradeIndex]._upgradePrefabsPath;
                    var prefab = Resources.Load<Transform>(currentPartPath);
                    var currentSpawnPoint = upgrades?[upgradeIndex]._spawnPoint;
                    SpawnNewPart(prefab, currentSpawnPoint);
                }
                    break;
                case Slot.Color:
                    upgrades = _truckUpgrades.FirstOrDefault(x => x._slot == slot)?._parts.ToList();
                    currentPartPath = upgrades?[upgradeIndex]._upgradePrefabsPath;
                    var mat = Resources.Load<Material>(currentPartPath);
                    SetNewMaterial(mat);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        public IEnumerable<Part> GetAvailableParts(Slot slot)
        {
            return _truckUpgrades.FindAll(x => x._slot == slot).SelectMany(x => x._parts);
        }

        public Part GetInstalledPart(Slot slot)
        {
            return _truckUpgrades.FindAll(x => x._slot == slot).SelectMany(x => x._parts)
                .FirstOrDefault(x => x._isInstalled);
        }

        public void BuyPart(Part linkedPart)
        {
            linkedPart._isBought = true;
            _playerResourcesManager.RemoveMoney(linkedPart._price);
        }

        public void ImplementUpgrade(Part linkedPart)
        {
            var upgradeIndex = _truckUpgrades.FindAll(x => x._slot == linkedPart._slot).
                SelectMany(x => x._parts).ToList().IndexOf(linkedPart);
            ImplementUpgrade(linkedPart._slot, upgradeIndex);
        }

        public void Save()
        {
            if (PhotonNetwork.InRoom)
                if (!_photonView.IsMine)
                    return;

            var statesList = new List<(Slot slot, int level, bool isInstalled, bool isBought, int truckDataID)>();
            foreach (var partsOfTypeContainer in _truckUpgrades)
            {
                foreach (var part in partsOfTypeContainer._parts)
                {
                    statesList.Add((partsOfTypeContainer._slot, Array.IndexOf(partsOfTypeContainer._parts, part),
                        part._isInstalled, part._isBought, _truckController.TruckPrefabId));
                }
            }

            ES3.Save("TruckUpgrades_" + _truckController.TruckPrefabId, statesList);
        }

        private List<(Slot slot, int level, bool isInstalled, bool isBought, int truckDataID)> Load()
        {
            if (!ES3.KeyExists("TruckUpgrades_" + _truckController.TruckPrefabId)) return null;
            var statesList = ES3.Load(("TruckUpgrades_" + _truckController.TruckPrefabId),
                new List<(Slot slot, int level, bool isInstalled, bool isBought, int truckDataID)>());
            return statesList;
        }

        private void ImplementUpgrades(
            List<(Slot slot, int level, bool isInstalled, bool isBought, int truckDataID)> values)
        {
            foreach (var truckUpgrade in _truckUpgrades)
            foreach (var part in truckUpgrade._parts)
            {
                var state = values.FirstOrDefault(x =>
                    x.slot == truckUpgrade._slot &&
                    x.level == Array.IndexOf(truckUpgrade._parts, part));
                if (state != default)
                {
                    part._isInstalled = state.isInstalled;
                    part._isBought = state.isBought;
                    if (part._isInstalled)
                        ImplementUpgrade(truckUpgrade._slot,
                            Array.IndexOf(truckUpgrade._parts, part));
                }
            }
        }

        private void UpdateCarState(
            List<(Slot slot, int level, bool isInstalled, bool isBought, int truckDataID)> values)
        {
            var objects = values
                .Select(value =>
                    $"{(int) value.slot},{value.level},{value.isInstalled},{value.isBought}, {value.truckDataID}")
                .ToArray();
            // if is in room
            if (PhotonNetwork.InRoom && _photonView.IsMine)
            {
                _photonView.RPC(nameof(UpdateCarStateOnClients), RpcTarget.All, new object[] {objects},
                    _photonView.ViewID);
            }
            else
            {
                UpdateCarStateOnClients(new object[] {objects}, _photonView.ViewID);
            }
        }

        [PunRPC]
        private void UpdateCarStateOnClients(object[] valuesArray, int viewID)
        {
            if (_photonView.ViewID != viewID) return;
            var values = new List<(Slot slot, int level, bool isInstalled, bool isBought, int truckDataID)>();

            foreach (var valuesObj in valuesArray)
            {
                var parts = (string[]) valuesObj;
                foreach (var part in parts)
                {
                    var stringData = part.Split(',');
                    var slot = (Slot) int.Parse(stringData[0]);
                    var level = int.Parse(stringData[1]);
                    var isInstalled = bool.Parse(stringData[2]);
                    var isBought = bool.Parse(stringData[3]);
                    var truckDataID = int.Parse(stringData[4]);
                    values.Add((slot, level, isInstalled, isBought, truckDataID));
                }
            }

            var parentTruckController = GetComponentInParent<TruckController>();
            Debug.Log("UpdateCarStateOnClients recieved values for " + parentTruckController.name);
            var log = values.Aggregate("Values: ", (current, value) =>
                current +
                $"Slot: \n {value.slot} \n Level: {value.level} \n IsInstalled: {value.isInstalled} \n IsBought: {value.isBought} \n");
            Debug.Log(log);
            // Update local state
            ImplementUpgrades(values);
        }
    }
}