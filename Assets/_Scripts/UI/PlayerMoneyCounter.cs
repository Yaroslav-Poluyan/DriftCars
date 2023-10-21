using System.Globalization;
using _Scripts.Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Scripts.UI
{
    public class PlayerMoneyCounter : MonoBehaviour
    {
        private TextMeshProUGUI _moneyText;
        [Inject] private PlayerResourcesManager _playerResourcesManager;

        private void Start()
        {
            _moneyText = GetComponent<TextMeshProUGUI>();
            _moneyText.text = "0";
            PlayerResourcesManager.OnMoneyChanged += OnMoneyChanged;
            OnMoneyChanged(_playerResourcesManager.Money);
        }

        private void OnMoneyChanged(float value)
        {
            _moneyText.text = value.ToString(CultureInfo.InvariantCulture) + "$";
        }
    }
}