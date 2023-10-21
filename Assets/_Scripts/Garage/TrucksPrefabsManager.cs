using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Garage
{
    public class TrucksPrefabsManager : MonoBehaviour
    {
        [SerializeField] private List<TruckPresetData> _truckPrefabs = new();

        public TruckPresetData GetNextPlayerTruckPresetData(TruckPresetData currentTruckPresetData)
        {
            var currentIndex = _truckPrefabs.IndexOf(currentTruckPresetData);
            if (currentIndex == _truckPrefabs.Count - 1)
            {
                return _truckPrefabs[0];
            }

            return _truckPrefabs[currentIndex + 1];
        }

        public TruckPresetData GetPreviousPlayerTruckPresetData(TruckPresetData currentTruckPresetData)
        {
            var currentIndex = _truckPrefabs.IndexOf(currentTruckPresetData);
            return currentIndex switch
            {
                0 => _truckPrefabs[^1],
                -1 => _truckPrefabs[0],
                _ => _truckPrefabs[currentIndex - 1]
            };
        }

        public TruckPresetData GetPlayerTruckPresetData(int idx)
        {
            if (idx >= 0 && idx < _truckPrefabs.Count) return _truckPrefabs[idx];
            throw new System.Exception("TruckPresetData with index " + idx + " not found");
        }

        public int GetPlayerTruckPresetDataIndex(TruckPresetData currentTruckPreset)
        {
            return _truckPrefabs.IndexOf(currentTruckPreset);
        }

        public TruckPresetData GetPreset(int id)
        {
            return _truckPrefabs.FirstOrDefault(truckPreset => truckPreset.PrefabID == id);
        }
    }
}