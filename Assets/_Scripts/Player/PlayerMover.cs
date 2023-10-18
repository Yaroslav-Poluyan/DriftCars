using System;
using _Scripts.Truck;
using Dreamteck.Splines;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [field: SerializeField] private SplineFollower SplineFollower { get; set; }
        [field: SerializeField] private float Speed { get; set; } = 1f;
        private PlayerBase _playerBase;

        private void Start()
        {
            _playerBase = GetComponent<PlayerBase>();
        }
    }
}