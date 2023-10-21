using _Scripts.Managers;
using _Scripts.Truck;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Scripts.Garage
{
    public class GarageTruckChanger : MonoBehaviour
    {
        [SerializeField] private Transform _disappearPoint;
        [SerializeField] private Transform _middlePoint;
        [SerializeField] private Transform _appearPoint;
        [SerializeField] private Transform _platePrefab;
        [SerializeField] private float _changeDuration = 2f;
        private Transform _currentPlate;
        private TruckController _currentTruck;
        private Transform _previousPlate;
        private TruckPresetData _currentTruckPreset;
        [Inject] private TrucksPrefabsManager _trucksPrefabsManager;
        [Inject] private PlayerResourcesManager _playerResourcesManager;
        private readonly Vector3 _localTruckPos = new(0, 1.4f, 0);

        public float ChangeDuration => _changeDuration;

        public TruckController CurrentTruck => _currentTruck;

        private void Start()
        {
            var currentTruckFromSave = ES3.Load("CurrentTruck", 0);
            var currentTruckPreset = _trucksPrefabsManager.GetPlayerTruckPresetData(currentTruckFromSave);
            ChangeTruckInGarage(currentTruckPreset, true);
        }

        public void ChangeToNextTruckInGarage()
        {
            var nextTruck = _trucksPrefabsManager.GetNextPlayerTruckPresetData(_currentTruckPreset);
            ChangeTruckInGarage(nextTruck, true);
        }

        public void ChangeToPreviousTruckInGarage()
        {
            var previousTruck = _trucksPrefabsManager.GetPreviousPlayerTruckPresetData(_currentTruckPreset);
            ChangeTruckInGarage(previousTruck, false);
        }

        private void ChangeTruckInGarage(TruckPresetData truckPreset, bool fromRight)
        {
            var appearPoint = fromRight ? _appearPoint.position : _disappearPoint.position;
            var disappearPoint = fromRight ? _disappearPoint.position : _appearPoint.position;
            //
            var newPlate = Instantiate(_platePrefab, appearPoint, Quaternion.identity);
            var newTruck = Instantiate(truckPreset.TruckPrefab, newPlate.position, Quaternion.identity);
            //
            newTruck.UpgradeManager.Initialize(_playerResourcesManager);
            newTruck.GetComponent<Rigidbody>().isKinematic = true;
            newTruck.GetComponentInChildren<TruckEffects>().enabled = false;
            newTruck.TruckPrefabId = truckPreset.PrefabID;
            newTruck.enabled = false;
            //
            newTruck.transform.SetParent(newPlate);
            newTruck.transform.localPosition = _localTruckPos;
            //newTruck.MonsterTruckInputSystemBase.MonsterTruckController.SetInGarageState(true);
            if (_currentPlate != null)
            {
                _currentPlate.DOMove(disappearPoint, ChangeDuration).SetDelay(.1f).onComplete += () =>
                {
                    Destroy(_previousPlate.gameObject);
                };
            }

            if (CurrentTruck != null)
            {
                CurrentTruck.transform.DOLocalMove(_localTruckPos, ChangeDuration).SetDelay(.1f);
            }

            _previousPlate = _currentPlate;
            _currentPlate = newPlate;
            _currentTruck = newTruck;
            _currentTruckPreset = truckPreset;

            newTruck.transform.DOLocalMove(_localTruckPos, ChangeDuration).SetDelay(.1f);
            newPlate.DOMove(_middlePoint.position, ChangeDuration).SetDelay(.1f);
        }

        public void ChooseCurrentTruck()
        {
            ES3.Save("CurrentTruck", _currentTruckPreset.PrefabID);
        }

        public void BuyCurrentTruck()
        {
            var currentTruckPrefabID = _currentTruckPreset.PrefabID;
            var preset = _trucksPrefabsManager.GetPlayerTruckPresetData(currentTruckPrefabID);
            if (_playerResourcesManager.CheckIsEnoughMoney(preset.Price))
            {
                _playerResourcesManager.RemoveMoney(preset.Price);
                preset.IsBought = true;
            }
        }

        public TruckPresetData GetCurrentTruckPreset()
        {
            return _currentTruckPreset;
        }
    }
}