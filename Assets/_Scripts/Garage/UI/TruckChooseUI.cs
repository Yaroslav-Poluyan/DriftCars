using System.Collections;
using System.Threading.Tasks;
using _Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Garage.UI
{
    public class TruckChooseUI : MonoBehaviour
    {
        [SerializeField] private Button _nextTruckButton;
        [SerializeField] private Button _previousTruckButton;
        [SerializeField] private Button _chooseTruckButton;
        [SerializeField] private Button _buyTruckButton;
        private Coroutine _waitForButtonInteractableCoroutine;
        [Inject] private GarageTruckChanger _garageTruckChanger;
        [Inject] private PlayerResourcesManager _playerResourcesManager;

        private void OnEnable()
        {
            _previousTruckButton.interactable = true;
            _nextTruckButton.interactable = true;
            if (_waitForButtonInteractableCoroutine != null)
                StopCoroutine(_waitForButtonInteractableCoroutine);
            _waitForButtonInteractableCoroutine = null;
        }

        public async void ShowNextTruck()
        {
            _garageTruckChanger.ChangeToNextTruckInGarage();
            _buyTruckButton.gameObject.SetActive(false);
            _chooseTruckButton.gameObject.SetActive(false);
            WaitForButtonInteractable();
            await Task.Delay(100);
        }

        public async void ShowPreviousTruck()
        {
            _garageTruckChanger.ChangeToPreviousTruckInGarage();
            _buyTruckButton.gameObject.SetActive(false);
            _chooseTruckButton.gameObject.SetActive(false);
            WaitForButtonInteractable();
            await Task.Delay(100);
        }

        public void ChooseTruck()
        {
            _garageTruckChanger.ChooseCurrentTruck();
        }

        public void BuyTruck()
        {
            _garageTruckChanger.BuyCurrentTruck();
            _buyTruckButton.gameObject.SetActive(false);
            _chooseTruckButton.gameObject.SetActive(true);
        }

        private void WaitForButtonInteractable()
        {
            if (_waitForButtonInteractableCoroutine != null)
            {
                StopCoroutine(_waitForButtonInteractableCoroutine);
                _waitForButtonInteractableCoroutine = null;
            }

            _waitForButtonInteractableCoroutine ??= StartCoroutine(WaitForButtonInteractableCoroutine());
        }

        private IEnumerator WaitForButtonInteractableCoroutine()
        {
            _previousTruckButton.interactable = false;
            _nextTruckButton.interactable = false;
            _chooseTruckButton.interactable = false;
            yield return new WaitForSeconds(_garageTruckChanger.ChangeDuration);
            var currentTruckPreset = _garageTruckChanger.GetCurrentTruckPreset();
            if (!currentTruckPreset.IsBought)
            {
                _buyTruckButton.gameObject.SetActive(true);
                _chooseTruckButton.gameObject.SetActive(false);
                _buyTruckButton.interactable = _playerResourcesManager.IsEnoughMoney(currentTruckPreset.Price);
                _buyTruckButton.image.color = _buyTruckButton.interactable ? Color.white : Color.red;
                _buyTruckButton.GetComponentInChildren<TextMeshProUGUI>().text = currentTruckPreset.Price + "$";
            }
            else
            {
                _buyTruckButton.gameObject.SetActive(false);
                _chooseTruckButton.gameObject.SetActive(true);
            }

            _chooseTruckButton.interactable = true;
            _previousTruckButton.interactable = true;
            _nextTruckButton.interactable = true;
        }
    }
}