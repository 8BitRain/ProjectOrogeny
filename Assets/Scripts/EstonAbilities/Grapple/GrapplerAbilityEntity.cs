using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerAbilityEntity : AbilityEntity
{
    public Transform GrappleTip;
    public Transform ConnectedObject;

    public override void InitializeAbilityEntity()
    {
        
    }

    public override void UpdateAbilityEntity()
    {
        Camera cam = GetAgent().GetComponent<ThirdPersonMovement>().cam.GetComponent<Camera>();

        if(GrappleTip != null)
        {
            GrappleTip.LookAt(cam.transform.forward);
        }

        if(ConnectedObject != null)
        {
            Debug.Log("Grappler Connected Object: Grappler is connected to " + ConnectedObject.name);
        }
    }

    public Grapple GetGrapple()
    {
        return GrappleTip.GetComponent<Grapple>();
    }

}
