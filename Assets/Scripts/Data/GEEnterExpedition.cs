using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEEnterExpedition", menuName = "Data/GEEnterExpedition", order = 2)]
public class GEEnterExpedition : GameEvent
{
    public string ExpeditionName = "Forest";

    [SerializeField]
    public int memberHardWarning = 3;

    [SerializeField]
    public int memberEasysWarning = 5;

    public override void Execute(System.Object obj = null)
    {
        if(CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.members != null && CORE.Instance.CurrentParty.members .Length == memberHardWarning)
        {
            WarningWindowUI.Instance.Show(memberHardWarning+" "+CORE.QuickTranslate("Member Expeditions are HARD")+"!",()=>
            {
                base.Execute(obj);
                SocketHandler.Instance.SendEnterExpedition(ExpeditionName);
            },false,null);
        }
        else if(CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.members != null && CORE.Instance.CurrentParty.members.Length == memberEasysWarning)
        {
            WarningWindowUI.Instance.Show(memberEasysWarning+" "+CORE.QuickTranslate("Member Expeditions are easy and there is less loot for everyone")+"!",()=>
            {
                base.Execute(obj);
                SocketHandler.Instance.SendEnterExpedition(ExpeditionName);
            },false,null);
        }
        else 
        {
            base.Execute(obj);
            SocketHandler.Instance.SendEnterExpedition(ExpeditionName);
        }
    }
}
