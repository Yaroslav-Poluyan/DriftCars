using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts.Managers
{
    public class PlayerResourcesManager : MonoBehaviour
    {
        [SerializeField] private PlayerResourcesData _playerResourcesData;

        private float Money
        {
            get => _playerResourcesData._money;
            set => _playerResourcesData._money = value;
        }

        private float TotalDriftScore
        {
            get => _playerResourcesData._totalDriftScore;
            set => _playerResourcesData._totalDriftScore = value;
        }

        public void AddMoney(float money)
        {
            Money += money;
        }

        public void RemoveMoney(float money)
        {
            Money -= money;
        }

        public void AddDriftScore(float driftScore)
        {
            TotalDriftScore += driftScore;
        }

        public void RemoveDriftScore(float driftScore)
        {
            TotalDriftScore -= driftScore;
        }

        public bool IsEnoughMoney(float price)
        {
            return Money >= price;
        }
    }
}