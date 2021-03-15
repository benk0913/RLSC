using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntity : MonoBehaviour
{
    public Portal PortalReference;

    public void Teleport()
    {
        SocketHandler.Instance.SendEnterPortal(PortalReference);
    }
}
