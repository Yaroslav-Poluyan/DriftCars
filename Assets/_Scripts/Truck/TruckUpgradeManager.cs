using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Managers;
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
        private PlayerResourcesManager _playerResourcesManager;

        private void Awake()
        {
            Load();
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

        private void SpawnNewPart(Transform part, Transform currentSpawnPoint)
        {
            //delete old part
            if (currentSpawnPoint.childCount > 0)
            {
                Destroy(currentSpawnPoint.GetChild(0).gameObject);
            }

            Instantiate(part, currentSpawnPoint.position, currentSpawnPoint.rotation, currentSpawnPoint);
        }

        private void ImplementUpgrade(Slot slot, int upgradeIndex)
        {
            List<Part> upgrades;
            var part = _truckUpgrades.FirstOrDefault(x => x._slot == slot)?._parts[upgradeIndex];
            if (part != null)
            {
                part._isInstalled = true;
                Save();
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
            Save();
        }

        public void ImplementUpgrade(Part linkedPart)
        {
            var upgradeIndex = _truckUpgrades.FindAll(x => x._slot == linkedPart._slot).SelectMany(x => x._parts)
                .ToList().IndexOf(linkedPart);
            ImplementUpgrade(linkedPart._slot, upgradeIndex);
        }

        private void Save()
        {
            var statesList = new List<(Slot slot, int level, bool isInstalled, bool isBought)>();
            foreach (var partsOfTypeContainer in _truckUpgrades)
            {
                foreach (var part in partsOfTypeContainer._parts)
                {
                    statesList.Add((partsOfTypeContainer._slot, Array.IndexOf(partsOfTypeContainer._parts, part),
                        part._isInstalled, part._isBought));
                }
            }

            ES3.Save("TruckUpgrades", statesList);
        }

        private void Load()
        {
            if (!ES3.KeyExists("TruckUpgrades")) return;
            var statesList = ES3.Load("TruckUpgrades",
                new List<(Slot slot, int level, bool isInstalled, bool isBought)>());
            foreach (var partsOfTypeContainer in _truckUpgrades)
            {
                foreach (var part in partsOfTypeContainer._parts)
                {
                    var state = statesList.FirstOrDefault(x =>
                        x.slot == partsOfTypeContainer._slot &&
                        x.level == Array.IndexOf(partsOfTypeContainer._parts, part));
                    if (state != default)
                    {
                        part._isInstalled = state.isInstalled;
                        part._isBought = state.isBought;
                        if (part._isInstalled)
                        {
                            ImplementUpgrade(partsOfTypeContainer._slot,
                                Array.IndexOf(partsOfTypeContainer._parts, part));
                        }
                    }
                }
            }
        }
    }
}