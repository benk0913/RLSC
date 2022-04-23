
using System;
using System.Collections.Generic;
using EdgeworldBase;
using SimpleJSON;
#if !UNITY_ANDROID && !UNITY_IOS
using Steamworks;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CashShopWindowUI : MonoBehaviour, WindowInterface
{
    public static CashShopWindowUI Instance;


    [SerializeField]
    Canvas CameraCanvas;

    [SerializeField]
    GameObject DisplayActorPanel;


    [SerializeField]
    TextBubbleUI DisplayTextBubble;

    [SerializeField]
    DisplayCharacterUI DisplayActor;


    [SerializeField]
    Button BuyButton;

    [SerializeField]
    TextMeshProUGUI BuyButtonMouseLabel;

    [SerializeField]
    TextMeshProUGUI BuyButtonJoystickLabel;

    [SerializeField]
    Animator Animer;



    [SerializeField]
    TextMeshProUGUI EQPLabel;

    [SerializeField]
    List<TextMeshProUGUI> EQPCostLabels;

    [SerializeField]
    List<TextMeshProUGUI> EQPValueLabels;

    [SerializeField]
    Transform CashItemsInventoryContainer;

     [SerializeField]
    Transform CurrentFocusedStoreContainer;

    [SerializeField]
    string CurrentFocusedStoreKey;
    
    [SerializeField]
    SelectionGroupUI SelectionGroup;

    [SerializeField]
    ScrollRect CashItemsInventoryScroll;

    CashShopProductUI SelectedProduct;


    public bool IsOpen;

    public string OpenSound;
    public string HideSound;


    public UnityEvent OnHide;

    float shortClickTimer = 0;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void OnEnable()
    {
        if(SimpleTouchController.Instance != null)
        {
            SimpleTouchController.Instance.gameObject.SetActive(false);
        }
    }
    void OnDisable()
    {
        if(SimpleTouchController.Instance != null)
        {
            SimpleTouchController.Instance.gameObject.SetActive(true);
        }

        if(ResourcesLoader.Instance != null && ResourcesLoader.Instance.LoadingWindowObject.gameObject.activeInHierarchy)
        {
            ResourcesLoader.Instance.LoadingWindowObject.gameObject.SetActive(false);
        }

        OnHide?.Invoke();

        if(CashShopWarningWindowUI.Instance != null)
            CashShopWarningWindowUI.Instance.Hide(false);
    }

    void Start()
    {
        if(Instance == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("Disconnect", Hide);


        RefreshUI();
    }

    void Update()
    {
        if(shortClickTimer > 0)
        {
            shortClickTimer -= Time.deltaTime;
        }
    }


    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        #if UNITY_ANDROID || UNITY_IOS
        transform.localScale = Vector3.one * 1.25f;
        #endif
        CORE.Instance.SubscribeToEvent("CashShopUpdated", RefreshUI);
        CORE.Instance.SubscribeToEvent("InventoryUpdated",RefreshEQPState);

        IsOpen = true;

        CameraCanvas.worldCamera = Camera.main;

        CORE.IsMachinemaMode = true;
        CORE.Instance.InvokeEvent("MachinemaModeRefresh");

        AudioControl.Instance.SetMusic("music_CashShopBaP");
        AudioControl.Instance.SetSoundscape("");

        CameraCanvas.gameObject.SetActive(true);

        DeselectProduct();
        
        
        AudioControl.Instance.Play(OpenSound);

        Animer.SetTrigger("Main");

        RefreshUI();

        RefreshSelectionGroup();
        RefreshEQPState();
    }

    public void ShowDisplayActor()
    {
        DisplayActorPanel.SetActive(true);

        ResetActorClone();

        RefreshEQPState();

        CORE.Instance.DelayedInvokation(0.1f,()=>{CashItemsInventoryScroll.verticalNormalizedPosition = 1f;});
    }

    public void ResetActorClone()
    {
        DisplayActor.AttachedCharacter.SetActorInfo(CORE.PlayerActor.Clone());

        DisplayActor.AttachedCharacter.Skin.StopEmote();
    }

    public void StripActorClone()
    {
        DisplayActor.AttachedCharacter.State.Data.equips.Clear();

        DisplayActor.AttachedCharacter.RefreshLooks();
    }

    public void HideDisplayActor()
    {
        DisplayActorPanel.SetActive(false);
    }

    public void RefreshEQPState()
    {
        Debug.LogError("CASH REFRESH");

        EQPLabel.text = System.String.Format("{0:n0}", SocketHandler.Instance.CurrentUser.info.cashPoints);

        for (int i = 0; i < CORE.Instance.Data.content.CashShop.Prices.Count; i++)
        {
            EQPCostLabels[i].text = CORE.QuickTranslate("For") +" "+ System.String.Format("{0:n0}", CORE.Instance.Data.content.CashShop.Prices[i].CostInUSD) +"<size=15>$</size>";
            EQPValueLabels[i].text = System.String.Format("{0:n0}", CORE.Instance.Data.content.CashShop.Prices[i].EQPValue);
        }

        CORE.ClearContainer(CashItemsInventoryContainer);

        CORE.Instance.DelayedInvokation(0.5f, () => 
        {
            for (int i = 0; i < SocketHandler.Instance.CurrentUser.info.cashItems.Count; i++)
            {
                InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUIUninteractable").GetComponent<InventorySlotUI>();
                slot.SetItem(SocketHandler.Instance.CurrentUser.info.cashItems[i], null);
                slot.transform.SetParent(CashItemsInventoryContainer, false);
                slot.transform.localScale = Vector3.one;
                slot.transform.position = Vector3.zero;
            }

            RefreshSelectionGroup();
        });
    }

    
    public void SelectProduct(CashShopProductUI product)
    {
        if(product == SelectedProduct && shortClickTimer > 0f)
        {
            BuyProduct();
            return;
        }

        shortClickTimer = 1f;

        if (SelectedProduct != null)
        {
            DeselectProduct(product.CurrentItem.Type == SelectedProduct.CurrentItem.Type);
        }

        product.SetSelected();
        SelectedProduct = product;

        Item itemInstance = new Item();
        itemInstance.Data = product.CurrentItem;


        BuyButton.gameObject.SetActive(true);


        if(itemInstance.Data.Type.name == "Chat Bubble")
        {
            DisplayTextBubble.gameObject.SetActive(true);
            DisplayTextBubble.Show(DisplayTextBubble.transform,"Hello",null,false,itemInstance.Data.Icon);
        }
        else
        {
            DisplayTextBubble.gameObject.SetActive(false);
        }

        if(itemInstance.Data.Type.name == "Emote")
        {
            if(DisplayActor.AttachedCharacter.Skin.EmoteRoutineInstance != null)
            {
                StopCoroutine(DisplayActor.AttachedCharacter.Skin.EmoteRoutineInstance);
                DisplayActor.AttachedCharacter.Skin.EmoteRoutineInstance = null;
            }

            DisplayActor.AttachedCharacter.Emote(CORE.Instance.Data.content.Emotes.Find(X=>X.name == itemInstance.Data.name));
            return;//Required so RefreshLooks wont wipe the emote...
        }

        if(itemInstance.Data.Type.name == "Consume" || itemInstance.Data.Type.name == "Use")
        {
            CORE.Instance.ActivateParams(itemInstance.Data.OnUseParams,DisplayActor.AttachedCharacter);
            // return;//Required so RefreshLooks wont wipe the consumable...
        }

        if(!DisplayActor.AttachedCharacter.State.Data.equips.ContainsKey(product.CurrentItem.Type.name))
        {
            DisplayActor.AttachedCharacter.State.Data.equips.Add(product.CurrentItem.Type.name, itemInstance);
        }
        else
        {
            DisplayActor.AttachedCharacter.State.Data.equips[product.CurrentItem.Type.name] = itemInstance;
        }
        
        DisplayActor.AttachedCharacter.RefreshLooks();

        if(CORE.Instance.DEBUG)
        {
            string TEST = "";

            foreach(string key in DisplayActor.AttachedCharacter.State.Data.equips.Keys)
            {
                Item item = DisplayActor.AttachedCharacter.State.Data.equips[key];
                TEST += key;

                if(item != null)
                    TEST += " " + item.Data.name  + " | ";
                else
                        TEST += " | ";
            }

            CORE.Instance.LogMessage(TEST);
        }

        RefreshSelectionGroup();
    }

    public void DeselectProduct(bool undress = false)
    {
        if(SelectedProduct != null)
        {
            if (undress)
            {
                DisplayActor.AttachedCharacter.State.Data.equips.Remove(SelectedProduct.CurrentItem.Type.name);
                DisplayActor.AttachedCharacter.RefreshLooks();
            }
            
            SelectedProduct.SetDeselected();
            SelectedProduct = null;
        }

        BuyButton.gameObject.SetActive(false);

        RefreshSelectionGroup();
    }

    public void BuyProduct()
    {
        if(SelectedProduct == null)
        {
            return;
        }



        CashShopWarningWindowUI.Instance.Show(CORE.QuickTranslate("Buy")+" "+ CORE.QuickTranslate(SelectedProduct.CurrentItem.DisplayName)+" "+CORE.QuickTranslate("for")+" "+SelectedProduct.CurrentItem.CashItemPrice+" SWP?",()=>
        {     
            if(SocketHandler.Instance.CurrentUser.info.cashPoints < SelectedProduct.CurrentItem.CashItemPrice)
            {
                CashShopWarningWindowUI.Instance.Show(CORE.QuickTranslate("You don't have enough SWP") +"! (" +SocketHandler.Instance.CurrentUser.info.cashPoints+"/"+SelectedProduct.CurrentItem.CashItemPrice+")",()=>
                {
                    DeselectProduct();
                    HideDisplayActor();
                    Animer.SetTrigger("Main");
                    Animer.SetTrigger("MorePoints");
                });

                return;
            }
            
            JSONClass data = new JSONClass();
            data["item"] = SelectedProduct.CurrentItem.name;
            SocketHandler.Instance.SendEvent("cashshop_buy_item",data);
        });
    }

    public void SetFocusedStoreContainer(Transform container)
    {
        CurrentFocusedStoreContainer = container;
    }
    public void PopulateContainerWithStore(string storeKey)
    {
        BuyButton.gameObject.SetActive(false);

        CurrentFocusedStoreKey = storeKey;

        if(CurrentFocusedStoreContainer == null)
        {
            CORE.Instance.LogMessageError("NO STORE CONTAINER SET!");
            return;
        }

        AudioControl.Instance.Play("CashShopSubCategoryClick");

        CORE.ClearContainer(CurrentFocusedStoreContainer);

        CORE.Instance.DelayedInvokation(0.01f,()=>
        {
            CashShopDatabase.CashShopStore currentStore = CORE.Instance.Data.content.CashShop.CashShopStores.Find(x=>x.StoreKey == storeKey);

            if(currentStore == null)
            {
                CORE.Instance.LogMessageError("No store with key "+storeKey);
                return;
            }

            foreach(ItemData item in currentStore.StoreItems)
            {
                CashShopProductUI product = ResourcesLoader.Instance.GetRecycledObject("CashShopProductUI").GetComponent<CashShopProductUI>();
                product.transform.SetParent(CurrentFocusedStoreContainer, false);
                product.transform.localScale = Vector3.one;
                product.transform.position = Vector3.zero;
                product.SetInfo(item);
            }

            RefreshSelectionGroup();
        }); 
    }   

    public void RefreshSelectionGroup()
    {
        CORE.Instance.DelayedInvokation(0.3f,()=>SelectionGroup.RefreshGroup());
    }

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
    public void Hide()
    {
        if (CORE.Instance != null)
        {
            CORE.Instance.UnsubscribeFromEvent("CashShopUpdated", RefreshUI);
            CORE.Instance.UnsubscribeFromEvent("InventoryUpdated", RefreshEQPState);

            if (CORE.PlayerActor.ActorEntity != null)
            {
                CORE.IsMachinemaMode = false;
                CORE.Instance.InvokeEvent("MachinemaModeRefresh");
                CORE.Instance.RefreshSceneInfo();

                AudioControl.Instance.Play(HideSound);
            }
        }

        IsOpen = false;
        CameraCanvas.gameObject.SetActive(false);

    }
    
    public void RefreshUI()
    {
       
        
        if (!IsOpen)
        {
            return;
        }


        RefreshSelectionGroup();

        BuyButtonJoystickLabel.gameObject.SetActive(CORE.Instance.IsUsingJoystick);
        BuyButtonMouseLabel.gameObject.SetActive(!CORE.Instance.IsUsingJoystick);

    }

#region  IN APP PURCHASE 

    public Action OnCompletePendingPurchase;
    public void BuyEQP(int dealIndex = 0)
    {
        ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

        JSONNode node = new JSONClass();
        node["dealIndex"].AsInt = dealIndex;
        SocketHandler.Instance.SendEvent("buy_eq", node);
        
#if !UNITY_ANDROID
        RegisterSteamInAppWindow();
#else
        EQIapManager.PurchaseItem(dealIndex.ToString());
#endif

        
    }


#if !UNITY_ANDROID && !UNITY_IOS
    protected Callback<MicroTxnAuthorizationResponse_t> MicroTxnAuthorizationCallbackContainer;
    void RegisterSteamInAppWindow()
    {
        if(MicroTxnAuthorizationCallbackContainer == null)
            MicroTxnAuthorizationCallbackContainer = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
    }
    void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t pCallback) 
    {
        OnInAppPurchaseResponse(pCallback.m_ulOrderID.ToString(),pCallback.m_ulOrderID.ToString(),pCallback.m_bAuthorized == 1);
    }
#endif

    public void OnInAppPurchaseResponse(string orderID, string transactionID, bool authorized, Action onCompletePendingPurchase = null)
    {

        CORE.Instance.ConditionalInvokation((x)=>{return CORE.PlayerActor!=null;},()=>
        {
            ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Checking Transaction "+orderID + " | " + transactionID  + " | " + authorized , Colors.AsColor(Colors.COLOR_GOOD), 3f));

            OnCompletePendingPurchase=onCompletePendingPurchase;

            JSONNode node = new JSONClass();
            node["orderId"] = orderID;
            node["transactionId"] = transactionID;
            node["authorized"].AsBool = authorized;
            SocketHandler.Instance.SendEvent("buy_eq_steam_answer", node);
        });
    }
    
    public void ShowPromotionalCodePrompt()
    {
        CORE.Instance.CloseCurrentWindow();
        InputLabelWindow.Instance.Show("Promo Code:","Enter your PROMO CODE",(string givenCode)=>
        {
            JSONClass node = new JSONClass();
            node["code"] = givenCode;
            SocketHandler.Instance.SendEvent("use_promo",node);
        });
    }

#endregion
}
