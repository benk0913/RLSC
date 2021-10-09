using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleJSON;
using TMPro;
using UnityEngine;

public class TradeWindowUI : MonoBehaviour
{
    public static TradeWindowUI Instance;

    PlayerTradeState PlayerState;
    PlayerTradeState OtherPlayerState;

    public GameObject PlayerAcceptObject;
    public GameObject OtherPlayerAcceptObject;

    public List<InventorySlotUI> PlayerItemSlots = new List<InventorySlotUI>();

    public TextMeshProUGUI PlayerMoneyLabel; 


    public List<InventorySlotUI> OtherPlayerItemSlots = new List<InventorySlotUI>();
    public TextMeshProUGUI OtherPlayerMoneyLabel; 



    [System.Serializable]
    public class PlayerTradeState
    {
        public PlayerTradeState(string pid = "", int mny = 0, Item[] itms = null)
        {
            this.playerId = pid;
            this.money = mny;
            this.items = itms;
        }

        public string playerId;
        public Item[] items;
        public int money;
    }

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

#region  OnEvents
    public void TradeStateUpdated(PlayerTradeState player = null, PlayerTradeState otherPlayer = null)
    {
        this.gameObject.SetActive(true);

        if(player != null)
        {
            PlayerState = player;
        }

        if(otherPlayer != null)
        {
            OtherPlayerState = otherPlayer;
        }

        if(PlayerState != null)
        {
            for(int i=0;i<PlayerItemSlots.Count;i++)
            {
                if(i > PlayerState.items.Length)
                {
                    PlayerItemSlots[i].SetItem(null);
                    continue;
                }

                PlayerItemSlots[i].SetItem(PlayerState.items[i]);
            }

            PlayerMoneyLabel.text = PlayerState.money.ToString();
        }

        if(OtherPlayerState != null)
        {
            for(int i=0;i<OtherPlayerItemSlots.Count;i++)
            {
                if(i > OtherPlayerState.items.Length)
                {
                    OtherPlayerItemSlots[i].SetItem(null);
                    continue;
                }

                OtherPlayerItemSlots[i].SetItem(OtherPlayerState.items[i]);
            }

            OtherPlayerMoneyLabel.text = OtherPlayerState.money.ToString();
        }
    }

    internal void StopTrade(string byWho)
    {
        this.gameObject.SetActive(false);
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Trade Cancelled",Colors.AsColor(Colors.COLOR_BAD)));
    }

    internal void AcceptTrade(string byWho)
    {
        if(PlayerState.playerId == byWho)
        {
            PlayerAcceptObject.SetActive(true);
        }
        else if(OtherPlayerState.playerId == byWho)
        {
            OtherPlayerAcceptObject.SetActive(true);
        }
    }

    internal void DontAcceptTrade(string byWho)
    {
        if(PlayerState.playerId == byWho)
        {
            PlayerAcceptObject.SetActive(false);
        }
        else if(OtherPlayerState.playerId == byWho)
        {
            OtherPlayerAcceptObject.SetActive(false);
        }
    }

    internal void TradeComplete(PlayerTradeState player, PlayerTradeState otherPlayer)
    {
        this.gameObject.SetActive(false);
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Trade Complete!",Colors.AsColor(Colors.COLOR_GOOD)));
    }

#endregion

#region  PlayerInput
    
    public void SetMoney()
    {
        InputLabelWindow.Instance.Show("Money","Set Amount",(string money)=>
        {
            int moneyValue = 0;
            if(int.TryParse(money, out moneyValue))
            {   
                if(moneyValue > CORE.PlayerActor.money)
                {
                    WarningWindowUI.Instance.Show("You don't have enough money!",null);    
                    return;
                }

                SendUpdateState(new PlayerTradeState(PlayerState.playerId,moneyValue, PlayerState.items));
            }
            else
            {
                WarningWindowUI.Instance.Show("Wrong amount of money!",null);
            }
        });
    }

    public void SetItem(Item item, InventorySlotUI slot)
    {
        int slotIndex = PlayerItemSlots.IndexOf(slot);
        List<Item> currentItems = new List<Item>();
        currentItems.AddRange(PlayerState.items);
        currentItems[slotIndex] = item;

        SendUpdateState( new PlayerTradeState(
            PlayerState.playerId,
            PlayerState.money,
            currentItems.ToArray()));
    }

    public void AddItem(Item item)
    {
        for(int i=0;i<PlayerItemSlots.Count;i++)
        {
            if(PlayerItemSlots[i].CurrentItem  == null)
            {
                SetItem(item,PlayerItemSlots[i]);
                return;
            }
        }

        WarningWindowUI.Instance.Show("Trade is full!",null);
    }


    public void SendUpdateState(PlayerTradeState state)
    {
        JSONNode data = new JSONClass();
        data[0] = JsonConvert.SerializeObject(state);

        SocketHandler.Instance.SendEvent("update_state",data);
    }

    public void SendStopCurrentTrade()
    {
        SocketHandler.Instance.SendEvent("stop_current_trade");
    }
    
    public void SendAccept()
    {
        WarningWindowUI.Instance.Show("Accept Trade?",()=>{SocketHandler.Instance.SendEvent("accept_trade");});
    }

    public void SendDontAccept()
    {
        SocketHandler.Instance.SendEvent("dont_accept_trade");
    }


#endregion
}

