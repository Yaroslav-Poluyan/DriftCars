using _Scripts.Truck;
using UnityEngine;

namespace _Scripts.Garage
{
    [CreateAssetMenu(fileName = "TruckPreset", menuName = "TruckPresets", order = 0)]
    public class TruckPresetData : ScriptableObject
    {
        [field: SerializeField] public int PrefabID { get; private set; }
        [field: SerializeField] public TruckController TruckPrefab { get; private set; }
    }
}