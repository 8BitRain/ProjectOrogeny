using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorAbilityEntity : AbilityEntity
{
    public override void InitializeAbilityEntity()
    {
        this.transform.localScale = new Vector3(0,0,0);
    }

    public override void UpdateAbilityEntity()
    {
        //Increase Dome Size
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, new Vector3(1000,1000,1000), 1 * Time.deltaTime);
    }
}
