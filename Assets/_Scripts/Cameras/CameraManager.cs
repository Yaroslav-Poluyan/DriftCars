using _Scripts.Truck;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Cameras
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _mainCamera;

        public void AssignCameraToTruck(TruckController controller)
        {
            _mainCamera.Follow = controller.transform;
            _mainCamera.LookAt = controller.transform;
        }
    }
}