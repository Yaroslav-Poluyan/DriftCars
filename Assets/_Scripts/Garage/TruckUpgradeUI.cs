using _Scripts.Garage.UI.TruckUpgrades.UpgradePart;
using _Scripts.Truck;
using UnityEngine;

namespace _Scripts.Garage
{
    public class TruckUpgradeUI : MonoBehaviour
    {
        [SerializeField] private PartUpgradePanel _partUpgradePanel;

        public void OpenPartUpgradePanel(TruckUpgradeManager.Slot slot)
        {
            gameObject.SetActive(true);
            _partUpgradePanel.SetVisibility(true);
            _partUpgradePanel.Init(slot);
        }
    }
}