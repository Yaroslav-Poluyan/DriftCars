using _Scripts.Managers;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace _Scripts.IAPS
{
    public class IAPBlock : MonoBehaviour
    {
        [Inject] private PlayerResourcesManager _playerResourcesManager;
        public void Buy(PurchaseEventArgs ars)
        {
            Debug.Log($"Purchased: {ars.purchasedProduct.definition.id}");
        }
    }
}