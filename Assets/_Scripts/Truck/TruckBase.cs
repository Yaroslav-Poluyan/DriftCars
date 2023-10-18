using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Truck
{
    public class TruckBase : MonoBehaviour
    {
        [field: SerializeField] public TruckMover TruckMover { get; private set; }
    }
}