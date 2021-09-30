
using System.Collections.Generic;
using EdgeworldBase;
using SimpleJSON;
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
    Animator Animer;



    [SerializeField]
    TextMeshProUGUI EQPLabel;

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
    
    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void OnDisable()
    {
        OnHide?.Invoke();
    }

    void Start()
    {
        if(Instance == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("CashShopUpdated", RefreshUI);
        CORE.Instance.SubscribeToEvent("InventoryUpdated",RefreshEQPState);

        RefreshUI();
    }


    public void Show(ActorData actorData, object data = null)
    {
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
        DisplayActor.AttachedCharacter.SetActorInfo(CORE.PlayerActor.Clone());
        
        RefreshEQPState();

        CORE.Instance.DelayedInvokation(0.1f,()=>{CashItemsInventoryScroll.verticalNormalizedPosition = 1f;});
    }

    public void HideDisplayActor()
    {
        DisplayActorPanel.SetActive(false);
    }

    public void RefreshEQPState()
    {

        EQPLabel.text = System.String.Format("{0:n0}", SocketHandler.Instance.CurrentUser.cashPoints);

        CORE.ClearContainer(CashItemsInventoryContainer);

        for (int i = 0; i < SocketHandler.Instance.CurrentUser.cashItems.Count; i++)
        {
            InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUI").GetComponent<InventorySlotUI>();
            slot.SetItem(SocketHandler.Instance.CurrentUser.cashItems[i], null);
            slot.transform.SetParent(CashItemsInventoryContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
        }

        RefreshSelectionGroup();
    }

    public void SelectProduct(CashShopProductUI product)
    {
        DeselectProduct();

        product.SetSelected();
        SelectedProduct = product;

        Item itemInstance = new Item();
        itemInstance.Data = product.CurrentItem;

        if(itemInstance.Data.Type.name == "Chat Bubble")
        {
            DisplayTextBubble.gameObject.SetActive(true);
            DisplayTextBubble.Show(DisplayTextBubble.transform,"Hello",null,false,itemInstance.Data.Icon);
        }
        else
        {
            DisplayTextBubble.gameObject.SetActive(false);
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

        BuyButton.gameObject.SetActive(true);

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
    }

    public void DeselectProduct()
    {
        if(SelectedProduct != null)
        {
            DisplayActor.AttachedCharacter.State.Data.equips.Remove(SelectedProduct.CurrentItem.Type.name);
            DisplayActor.AttachedCharacter.RefreshLooks();
            
            SelectedProduct.SetDeselected();
            SelectedProduct = null;
        }

        BuyButton.gameObject.SetActive(false);
    }

    public void BuyProduct()
    {
        if(SelectedProduct == null)
        {
            return;
        }

        if(SocketHandler.Instance.CurrentUser.cashPoints < SelectedProduct.CurrentItem.CashItemPrice)
        {
            TopNotificationUI.Instance.Show(
                new TopNotificationUI.TopNotificationInstance("You don't have enough EQP! ("+SocketHandler.Instance.CurrentUser.cashPoints+"/"+SelectedProduct.CurrentItem.CashItemPrice+")", Color.red,3f,true));
            return;
        }

        CashShopWarningWindowUI.Instance.Show("Buy "+SelectedProduct.CurrentItem.DisplayName+" for "+SelectedProduct.CurrentItem.CashItemPrice+" EQP? ",()=>
        {
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
        CurrentFocusedStoreKey = storeKey;

        if(CurrentFocusedStoreContainer == null)
        {
            CORE.Instance.LogMessageError("NO STORE CONTAINER SET!");
            return;
        }

        CORE.ClearContainer(CurrentFocusedStoreContainer);

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
    }   

    public void RefreshSelectionGroup()
    {
        CORE.Instance.DelayedInvokation(0.3f,()=>SelectionGroup.RefreshGroup());
    }

    public void Hide()
    {
        CORE.Instance.UnsubscribeFromEvent("CashShopUpdated", RefreshUI);
        CORE.Instance.UnsubscribeFromEvent("InventoryUpdated",RefreshEQPState);

        if(CORE.Instance != null && CORE.PlayerActor.ActorEntity != null)
        {
            CORE.IsMachinemaMode = false;
            CORE.Instance.InvokeEvent("MachinemaModeRefresh");
            CORE.Instance.RefreshSceneInfo();
            
            AudioControl.Instance.Play(HideSound);
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

    }


}
