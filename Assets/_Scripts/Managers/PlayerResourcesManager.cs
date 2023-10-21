using System;
using _Scripts.IAPS;
using _Scripts.ScriptableObjects;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Managers
{
    public class PlayerResourcesManager : MonoBehaviour, IProductPurchaseHandler
    {
        public static event Action<float> OnMoneyChanged;
        public static event Action<float> OnDriftScoreChanged;
        [SerializeField] private PlayerResourcesData _playerResourcesData;

        private void Awake()
        {
            UpdatePlayerScore(TotalDriftScore);
        }

        public float Money
        {
            get => _playerResourcesData._money;
            private set
            {
                _playerResourcesData._money = value;
                OnMoneyChanged?.Invoke(value);
            }
        }

        public float TotalDriftScore
        {
            get => _playerResourcesData._totalDriftScore;
            private set
            {
                _playerResourcesData._totalDriftScore = value;
                UpdatePlayerScore(value);
                OnDriftScoreChanged?.Invoke(value);
            }
        }

        private static void UpdatePlayerScore(float score)
        {
            var customProperties = new ExitGames.Client.Photon.Hashtable
            {
                ["score"] = score
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
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

        public bool CheckIsEnoughMoney(float presetPrice)
        {
            return Money >= presetPrice;
        }

        public void OnPurchase(Product product)
        {
            print($"Purchased {product}");
            switch (product)
            {
                case Product.FiveHundredDollars:
                    AddMoney(500);
                    break;
                case Product.OneThousandAndFiveHundredDollars:
                    AddMoney(1500);
                    break;
                case Product.FiveThousandDollars:
                    AddMoney(5000);
                    break;
                case Product.TenThousandDollars:
                    AddMoney(10000);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(product), product, null);
            }
        }

        public void OnPurchaseFail(Product product, string reason)
        {
            Debug.LogError($"Failed to purchase {product} because of {reason}");
        }
    }
}