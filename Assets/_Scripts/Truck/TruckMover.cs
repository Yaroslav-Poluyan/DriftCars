using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using Zenject;

namespace _Scripts.Truck
{
    public class TruckMover : MonoBehaviour
    {
        [field: SerializeField] public List<Transform> Wheels { get; private set; } = new();
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _acceleration = 1f;
        [SerializeField] private float _deceleration = 1f;
        [Inject] private InputManager.InputManager _inputManager;

        private void FixedUpdate()
        {
            TurnLogic();
        }

        private void TurnLogic()
        {
            var horizontalInput = _inputManager.HorizontalInput;
            switch (horizontalInput)
            {
                case > 0:
                    TurnRight();
                    break;
                case < 0:
                    TurnLeft();
                    break;
                default:
                    Stabilize();
                    break;
            }
        }

        private void Stabilize()
        {
            throw new NotImplementedException();
        }

        private void TurnLeft()
        {
            throw new NotImplementedException();
        }

        private void TurnRight()
        {
            throw new NotImplementedException();
        }
    }
}