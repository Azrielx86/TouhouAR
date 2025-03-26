using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Vuforia;

public class Portal : MonoBehaviour
{
    public Material enabledMaterial;
    public Material disabledMaterial;
    [CanBeNull] public Portal targetPortal;
    public PortalType portalType;

    private void Start()
    {
        GetComponent<Renderer>().material = disabledMaterial;
    }

    public void OnConnectedPortal(Portal target)
    {
        targetPortal = target;
        GetComponent<Renderer>().material = enabledMaterial;

        if (targetPortal is null) return;
        Debug.Log($"Found portal at position {targetPortal.transform.position}");
        Debug.DrawLine(transform.position, targetPortal.transform.position, Color.yellow);
    }

    public void OnLostPortal()
    {
        targetPortal = null;
        GetComponent<Renderer>().material = disabledMaterial;
        Debug.Log("Connected portal lost.");
    }
}