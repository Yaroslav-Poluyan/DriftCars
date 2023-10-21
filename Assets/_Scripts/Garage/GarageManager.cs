using System.Collections.Generic;
using _Scripts.Truck;
using UnityEngine;

namespace _Scripts.Garage
{
    public class GarageManager : MonoBehaviour
    {
        [SerializeField] private List<TruckUpgradeLabel> _truckUpgradeLabels;
        private TruckController _currentTruckController;
        public void SetLabelsState(bool state)
        {
            foreach (var label in _truckUpgradeLabels)
            {
                label.SetActive(state);
            }
        }
    }
}