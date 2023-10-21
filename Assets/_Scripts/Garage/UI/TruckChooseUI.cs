using System.Collections;
using System.Threading.Tasks;
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
        private Coroutine _waitForButtonInteractableCoroutine;
        [Inject] GarageTruckChanger _garageTruckChanger;

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
            WaitForButtonInteractable();
            await Task.Delay(100);
        }

        public async void ShowPreviousTruck()
        {
            _garageTruckChanger.ChangeToPreviousTruckInGarage();
            WaitForButtonInteractable();
            await Task.Delay(100);
        }

        public void ChooseTruck()
        {
            _garageTruckChanger.ChooseCurrentTruck();
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
            _chooseTruckButton.interactable = true;
            _previousTruckButton.interactable = true;
            _nextTruckButton.interactable = true;
        }
    }
}