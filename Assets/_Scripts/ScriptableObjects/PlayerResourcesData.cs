using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerResources", menuName = "Datas/PlayerResourcesData", order = 0)]
    public class PlayerResourcesData : ScriptableObject
    {
        public float _money;
        public float _totalDriftScore;

    }
}