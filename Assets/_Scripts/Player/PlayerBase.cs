using System;
using _Scripts.Truck;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerBase : MonoBehaviour
    {
        [field: SerializeField] public PlayerMover PlayerMover { get; private set; }
        [field: SerializeField] public TruckBase TruckBase { get; private set; }
    }
}