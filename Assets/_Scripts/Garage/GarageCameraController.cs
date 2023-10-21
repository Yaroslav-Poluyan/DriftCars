using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Scripts.Garage
{
    public class GarageCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _garageCamera;
        [SerializeField] private Transform _garageCameraTarget;
        [SerializeField] private float _rotationSpeed = 100f; // Настройка скорости вращения
        [Inject] private InputManager.InputManager _inputManager;
        [Inject] private GarageTruckChanger _garageTruckChanger;
        private float _radius = 5f;
        private Vector3 _defaultPosition;
        private float _angle = 0f;
        private bool _ismoving = false;
        private bool _canBeMovedByTouch = true;

        private void Start()
        {
            _defaultPosition = _garageCamera.transform.position;
            _radius = -_garageCamera.transform.position.z;
        }

        private void Update()
        {
            if (!_canBeMovedByTouch) return;
            var horizontal = -_inputManager.GetHorizontalInput();
            _angle += horizontal * Time.deltaTime * _rotationSpeed;
            if (!_ismoving) UpdateCameraPosition();
            UpgradeCameraRotation();
        }

        private void UpdateCameraPosition()
        {
            // Пересчитываем позицию камеры на основе угла вокруг точки и радиуса
            var x = _garageCameraTarget.position.x + _radius * Mathf.Sin(Mathf.Deg2Rad * _angle);
            var z = _garageCameraTarget.position.z + _radius * Mathf.Cos(Mathf.Deg2Rad * _angle);
            _garageCamera.transform.position = new Vector3(x, _defaultPosition.y, z);
        }

        private void UpgradeCameraRotation()
        {
            _garageCamera.LookAt = _garageCameraTarget;
        }

        public void SetCameraToPartPosition(TruckUpgradeLabel truckUpgradeLabel)
        {
            var truck = _garageTruckChanger.CurrentTruck;
            var direction = Vector3.ProjectOnPlane(truckUpgradeLabel.transform.position - truck.transform.position,
                Vector3.up);
            var position = truck.transform.position + direction.normalized * _radius;
            position.y = _garageCamera.transform.position.y;
            SetCameraToPosition(position);
        }

        private void SetCameraToPosition(Vector3 position)
        {
            _ismoving = true;
            _defaultPosition = position;
            _angle = CalculateAngle(position);
            _garageCamera.transform.DOKill();
            SwitchTouchMove(false);
            _garageCamera.transform.DOMove(position, 2f).onComplete += () =>
            {
                _ismoving = false;
                SwitchTouchMove(true);
            };
        }

        public void SwitchTouchMove(bool canBeMovedByTouch)
        {
            _canBeMovedByTouch = canBeMovedByTouch;
        }

        private float CalculateAngle(Vector3 position)
        {
            var direction = Vector3.ProjectOnPlane(position - _garageCameraTarget.position, Vector3.up);
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }
    }
}