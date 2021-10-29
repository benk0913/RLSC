using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;

public class QueWindowUI : MonoBehaviour
{
    public static QueWindowUI Instance;

    public TextMeshProUGUI MessageLabel;
    
    Coroutine PollingRoutine = null;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(int position)
    {
        if(!this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(true);

        MessageLabel.text ="The server is temporarily full, Your position in queue: "+position+".";
        
        PollingRoutine = CORE.Instance.DelayedInvokation(5f, () => SocketHandler.Instance.SendLoginQueuePositionRequest(OnQueuePositionResponse));
    }

    void OnQueuePositionResponse(JSONNode data)
    {
        int playersBefore = data["playersBefore"].AsInt;
        bool canLogin = data["canLogin"].AsBool;
        if (canLogin) {
            Hide();
            SocketHandler.Instance.SendConnectSocket();
        } else {
            Show(playersBefore);
        }
    }

    public void Hide()
    {
        if (PollingRoutine != null)
        {
            StopCoroutine(PollingRoutine);
        }
        this.gameObject.SetActive(false);
    }
}
