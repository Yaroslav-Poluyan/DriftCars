using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Truck;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts.Garage.UI.TruckUpgrades.UpgradePart
{
    public class PartUpgradePanel : MonoBehaviour
    {
        [SerializeField] private UIPartBlock _partBlockPrefab;
        [SerializeField] private TextMeshProUGUI _partNameText;
        [SerializeField] private Transform _partBlockContainer;
        private readonly List<UIPartBlock> _createdPartBlocks = new();
        [Inject] private GarageTruckChanger _garageTruckChanger;
        [Inject] private PlayerResourcesManager _playerResourcesManager;

        public void SetVisibility(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Init(TruckUpgradeManager.Slot slot)
        {
            _partNameText.text = slot.ToString();
            var listPrefabParts =
                _garageTruckChanger.CurrentTruck.UpgradeManager.GetAvailableParts(slot);

            foreach (Transform child in _partBlockContainer.transform)
            {
                Destroy(child.gameObject);
            }

            _createdPartBlocks.Clear();
            if (listPrefabParts == null) return;
            foreach (var part in listPrefabParts)
            {
                var partBlock = Instantiate(_partBlockPrefab, _partBlockContainer);
                partBlock.Init(part, this, _playerResourcesManager, _garageTruckChanger);
                _createdPartBlocks.Add(partBlock);
            }
        }

        public void OnPartsChanged()
        {
            foreach (var block in _createdPartBlocks)
            {
                block.OnPartsChanged();
            }
        }
    }
}