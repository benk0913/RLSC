using EdgeworldBase;
using UnityEngine;
using UnityEngine.Purchasing;

public class EQIapManager : IStoreListener {

    private static IStoreController Controller;
    private static IExtensionProvider Extensions;

    public static bool IsInitialized= false;

    public EQIapManager () {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for(int i=0;i< CORE.Instance.Data.content.CashShop.Prices.Count;i++)
        {
            builder.AddProduct(i.ToString(), ProductType.Consumable, new IDs
            {
                {i+"_google", GooglePlay.Name},
                {i+"_mac", MacAppStore.Name}
            });
        }

        UnityPurchasing.Initialize (this, builder);
    }

    public static void PurchaseItem(string id)
    {
        Controller.InitiatePurchase(id);
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
    {
        Controller = controller;
        Extensions = extensions;
        IsInitialized = true;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed (InitializationFailureReason error)
    {
        CORE.Instance.LogMessageError("EQIapManager ERROR - "+error.ToString());
        IsInitialized = false;
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
    {
        CashShopWindowUI.Instance.OnInAppPurchaseResponse(e.purchasedProduct.definition.id,e.purchasedProduct.transactionID,e.purchasedProduct.hasReceipt
        ,()=>
        {
            Controller.ConfirmPendingPurchase(e.purchasedProduct);
        });

        return PurchaseProcessingResult.Pending;
    }
    

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed (Product i, PurchaseFailureReason p)
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);
        CORE.Instance.LogMessageError("EQIapManager Purchase ERROR - "+i.ToString()+" | "+p.ToString());
    }
}