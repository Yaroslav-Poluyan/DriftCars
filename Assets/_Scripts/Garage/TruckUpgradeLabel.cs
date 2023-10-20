using _Scripts.Truck;
using UnityEngine;
using Zenject;

namespace _Scripts.Garage
{
    public class TruckUpgradeLabel : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TruckUpgradeManager.Slot _slot;
        [Inject] private TruckUpgradeUI _truckUpgradeUi;
        [Inject] private GarageCameraController _garageCamerasManager;
        [Inject] private GarageManager _garageManager;
        private Camera _camera;
        private TruckUpgradeManager TruckUpgradeManager => _garageManager.CurrentTruckController.UpgradeManager;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void OnMouseDown()
        {
            print("OnMouseDown");
            if (TruckUpgradeManager != null)
            {
                _garageCamerasManager.SetCameraToPartPosition(this);
                _truckUpgradeUi.OpenPartUpgradePanel(_slot);
            }
        }

        private void Update()
        {
            //rotate towards camera
            transform.LookAt(_camera.transform);
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}