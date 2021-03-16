using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntity : MonoBehaviour
{
    public Portal PortalReference;

    private void Reset()
    {
        PortalReference.portalPositionX = transform.position.x;
        PortalReference.portalPositionY = transform.position.y;
    }

    public void Teleport()
    {
        SocketHandler.Instance.SendEnterPortal(PortalReference);
    }
}
