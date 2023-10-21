using System;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace _Scripts.IAPS
{
    public enum Product
    {
        FiveHundredDollars,
        OneThousandAndFiveHundredDollars,
        FiveThousandDollars,
        TenThousandDollars,
    }

    public interface IProductPurchaseHandler
    {
        void OnPurchase(Product product);
        void OnPurchaseFail(Product product, string reason);
    }

    public class IAPManager : MonoBehaviour, IStoreListener
    {
        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;
        [Inject] private PlayerResourcesManager _playerResourcesManager;

        private void Start()
        {
            InitializePurchasing();
        }

        private void InitializePurchasing()
        {
            if (IsInitialized())
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in Enum.GetNames(typeof(Product)))
            {
                builder.AddProduct(product, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            return _storeController != null && _storeExtensionProvider != null;
        }

        public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
        {
            _playerResourcesManager?.OnPurchaseFail((Product) Enum.Parse(typeof(Product), product.definition.id),
                failureReason.ToString());
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Failed to initialize: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Failed to initialize: {error} {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            if (Enum.TryParse(e?.purchasedProduct?.definition?.id, out Product product))
            {
                _playerResourcesManager?.OnPurchase(product);
            }
            else
            {
                Debug.LogError($"Purchase Failed: Unrecognized product: {e.purchasedProduct.definition.id}");
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _playerResourcesManager?.OnPurchaseFail(product, $"Purchase of {product} failed due to {failureReason}");
        }
    }
}