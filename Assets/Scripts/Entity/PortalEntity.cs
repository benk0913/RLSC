using UnityEngine;
using UnityEngine.Events;

public class PortalEntity : MonoBehaviour
{
    public Portal PortalReference;

    [SerializeField]
    SpriteRenderer GateSymbol;

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
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("The chamber is not complete!", Color.yellow, 3));
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
