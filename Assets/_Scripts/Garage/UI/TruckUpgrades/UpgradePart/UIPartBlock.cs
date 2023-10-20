using System;
using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Truck;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Garage.UI.TruckUpgrades.UpgradePart
{
    public class UIPartBlock : MonoBehaviour
    {
        [SerializeField] private Image _spriteRenderer;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Button _installButton;
        [SerializeField] private Button _buyButton;
        private TruckUpgradeManager.Part _linkedPart;
        private PartUpgradePanel _partUpgradePanel;
        private PlayerResourcesManager _playerResourcesManager;
        private GarageManager _garageManager;

        public void Init(TruckUpgradeManager.Part thisPart, PartUpgradePanel partUpgradePanel,
            PlayerResourcesManager playerResourcesManager, GarageManager garageManager)
        {
            _partUpgradePanel = partUpgradePanel;
            _linkedPart = thisPart;
            _spriteRenderer.sprite = thisPart._partSprite;
            _nameText.text = thisPart._name;
            _playerResourcesManager = playerResourcesManager;
            _garageManager = garageManager;
            SetButtons();
        }

        private void SetButtons()
        {
            if (_linkedPart._isBought)
            {
                _buyButton.gameObject.SetActive(false);
                _installButton.gameObject.SetActive(true);
                if (_linkedPart._isInstalled)
                {
                    _installButton.GetComponentInChildren<TextMeshProUGUI>().text = "INSTALLED";
                    _installButton.interactable = false;
                }
                else
                {
                    _installButton.GetComponentInChildren<TextMeshProUGUI>().text = "INSTALL";
                    _installButton.interactable = true;
                }
            }
            else
            {
                _buyButton.gameObject.SetActive(true);
                _installButton.gameObject.SetActive(false);
                _buyButton.interactable =
                    _playerResourcesManager.IsEnoughMoney(_linkedPart._price);
                _buyButton.GetComponentInChildren<TextMeshProUGUI>().text = _linkedPart._price.ToString();
            }
        }

        public void OnPartsChanged()
        {
            SetButtons();
        }

        public void BuyButtonPressedHandler()
        {
            _playerResourcesManager.RemoveMoney(_linkedPart._price);
            _garageManager.CurrentTruckController.UpgradeManager.BuyPart(_linkedPart);
            _partUpgradePanel.OnPartsChanged();
        }

        public void InstallButtonPressedHandler()
        {
            _garageManager.CurrentTruckController.UpgradeManager.ImplementUpgrade(_linkedPart);
            _partUpgradePanel.OnPartsChanged();
        }
    }
}