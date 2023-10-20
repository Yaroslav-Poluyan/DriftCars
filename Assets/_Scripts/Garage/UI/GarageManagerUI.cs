using UnityEngine;

namespace _Scripts.Garage.UI
{
    public class GarageManagerUI : MonoBehaviour
    {
        [SerializeField] private TruckUpgradeUI _truckUpgradeUI;
        [SerializeField] private Transform _garageUI;

        public void OpenUpgradeUI()
        {
            //_truckUpgradeUI.gameObject.SetActive(true);
            _garageUI.gameObject.SetActive(false);
        }

        public void CloseUpgradeUI()
        {
            _truckUpgradeUI.gameObject.SetActive(false);
            _garageUI.gameObject.SetActive(true);
        }
    }
}