using System.Collections.Generic;
using _Scripts.Managers;
using _Scripts.Truck;
using UnityEngine;
using Zenject;

namespace _Scripts.Garage
{
    public class GarageManager : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private TruckController _carPrefab;
        [SerializeField] private List<TruckUpgradeLabel> _truckUpgradeLabels;
        private TruckController _currentTruckController;
        [Inject] private PlayerResourcesManager _playerResourcesManager;

        public TruckController CurrentTruckController => _currentTruckController;

        private void Start()
        {
            SpawnCar();
        }

        private void SpawnCar()
        {
            _currentTruckController = Instantiate(_carPrefab, _spawnPoint.position, _spawnPoint.rotation);
            _currentTruckController.UpgradeManager.Initialize(_playerResourcesManager);
            CurrentTruckController.GetComponent<Rigidbody>().isKinematic = true;
            CurrentTruckController.enabled = false;
        }

        public void SetLabelsState(bool state)
        {
            foreach (var label in _truckUpgradeLabels)
            {
                label.SetActive(state);
            }
        }
    }
}