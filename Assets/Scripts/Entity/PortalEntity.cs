using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PortalEntity : MonoBehaviour
{
    public Portal PortalReference;

    [SerializeField]
    SpriteRenderer GateSymbol;

    [SerializeField]
    string EnterWarning = "";

    public UnityEvent OnLock;
    public UnityEvent OnUnlock;
    public bool IsLocked;

    private void Reset()
    {
        PortalReference.portalPositionX = transform.position.x;
        PortalReference.portalPositionY = transform.position.y;
    }

    private void Start()
    {
        if(PortalReference.IsExpeditionLocked)
        {
            Lock();
        }
    }

    private void OnEnable()
    {
        CORE.Instance.SubscribeToEvent("ChamberComplete", Unlock);

    }

    private void OnDisable()
    {
        CORE.Instance.UnsubscribeFromEvent("ChamberComplete", Unlock);
    }

    public void Teleport()
    {
        if(IsLocked)
        {
            TopNotificationUI.Instance.Show(
                new TopNotificationUI.TopNotificationInstance("The chamber is not complete! "
                + CORE.Instance.Data.content.Scenes.Find(X => X.sceneName == SceneManager.GetActiveScene().name).objectiveDescription, 
                Colors.AsColor(Colors.COLOR_HIGHLIGHT),
                3));
            return;
        }

        if(!string.IsNullOrEmpty(EnterWarning))
        {
            if(!WarningWindowUI.Instance.gameObject.activeInHierarchy)
                WarningWindowUI.Instance.Show(EnterWarning,()=>{SocketHandler.Instance.SendEnterPortal(PortalReference);});

            return;
        }
        
        SocketHandler.Instance.SendEnterPortal(PortalReference);
    }

    public void Lock()
    {
        OnLock?.Invoke();
        IsLocked = true;
    }

    public void Unlock()
    {
        OnUnlock?.Invoke();
        IsLocked = false;
    }
}
