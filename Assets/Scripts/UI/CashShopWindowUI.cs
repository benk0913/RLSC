
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

    float shortClickTimer = 0;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void OnDisable()
    {
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

        CORE.Instance.SubscribeToEvent("CashShopUpdated", RefreshUI);
        CORE.Instance.SubscribeToEvent("InventoryUpdated",RefreshEQPState);

        RefreshUI();
    }

    void Update()
    {
        if(shortClickTimer > 0)
        {
            shortClickTimer -= Time.deltaTime;
        }
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

        EQPLabel.text = System.String.Format("{0:n0}", SocketHandler.Instance.CurrentUser.cashPoints);

        CORE.ClearContainer(CashItemsInventoryContainer);

        for (int i = 0; i < SocketHandler.Instance.CurrentUser.cashItems.Count; i++)
        {
            InventorySlotUI slot = ResourcesLoader.Instance.GetRecycledObject("InventorySlotUIUninteractable").GetComponent<InventorySlotUI>();
            slot.SetItem(SocketHandler.Instance.CurrentUser.cashItems[i], null);
            slot.transform.SetParent(CashItemsInventoryContainer, false);
            slot.transform.localScale = Vector3.one;
            slot.transform.position = Vector3.zero;
        }

        RefreshSelectionGroup();
    }

    
    public void SelectProduct(CashShopProductUI product)
    {
        if(product == SelectedProduct && shortClickTimer > 0f)
        {
            BuyProduct();
            return;
        }

        shortClickTimer = 1f;
        
        DeselectProduct();

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

        if(itemInstance.Data.Type.name == "Consume")
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



        CashShopWarningWindowUI.Instance.Show("Buy "+SelectedProduct.CurrentItem.DisplayName+" for "+SelectedProduct.CurrentItem.CashItemPrice+" EQP? ",()=>
        {     
            if(SocketHandler.Instance.CurrentUser.cashPoints < SelectedProduct.CurrentItem.CashItemPrice)
            {
                CashShopWarningWindowUI.Instance.Show("You don't have enough EQP! ("+SocketHandler.Instance.CurrentUser.cashPoints+"/"+SelectedProduct.CurrentItem.CashItemPrice+")",()=>
                {

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

    public void BuyEQP(int dealIndex = 0)
    {
        
    }

}
