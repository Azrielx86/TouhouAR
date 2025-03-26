using UnityEngine;

public class Teleportable : MonoBehaviour
{
    private PortalType _lastPortalUsed = PortalType.None;

    private void OnTriggerEnter(Collider collidedObject)
    {
        Debug.Log("Collision detected");
        var portal = collidedObject.gameObject.GetComponentInChildren<Portal>();
        if (portal?.targetPortal is null) return;
        if (portal.portalType == _lastPortalUsed) return;
        _lastPortalUsed = portal.targetPortal.portalType;
        TeleportToPortal(portal.targetPortal);
    }

    private void TeleportToPortal(Portal portal)
    {
        transform.position = portal.transform.position;
        transform.parent = null;
    }
}