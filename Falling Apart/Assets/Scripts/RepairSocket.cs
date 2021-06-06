using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RepairSocket : XRSocketInteractor
{
    [Space(10)]
    [Header("Repair Socket Variables")]
    public SystemScript parentSystemScript;

    RepairStation parentSystem;

    public override bool CanHover(XRBaseInteractable interactable)
    {
        return base.CanHover(interactable);
    }

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        return base.CanSelect(interactable);
    }

    //Called just before the component is added
    protected override void OnSelectEntering(XRBaseInteractable interactable)
    {
        parentSystem = (RepairStation)parentSystemScript.system;
        parentSystem.componentRepairing = (interactable.GetComponent<ComponentScript>().component);
        if (interactable.gameObject.GetComponent<ComponentScript>().componentType == ComponentType.FILTER) ((Filter)interactable.gameObject.GetComponent<ComponentScript>().component).clean();
        base.OnSelectEntering(interactable);
    }

    //Called just before the component is removed
    protected override void OnSelectExiting(XRBaseInteractable interactable)
    {
        parentSystem = (RepairStation)parentSystemScript.system;
        parentSystem.componentRepairing = null;
        base.OnSelectExiting(interactable);
    }
}
