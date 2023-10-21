using _Scripts.Truck;
using UnityEngine;

namespace _Scripts.Garage
{
    [CreateAssetMenu(fileName = "TruckPreset", menuName = "TruckPresets", order = 0)]
    public class TruckPresetData : ScriptableObject
    {
        [field: SerializeField] public bool IsBought { get; set; } = false;
        [field: SerializeField] public float Price { get; private set; } = 100f;
        [field: SerializeField] public int PrefabID { get; private set; }
        [field: SerializeField] public TruckController TruckPrefab { get; private set; }
    }
}