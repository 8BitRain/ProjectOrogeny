using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorVFXEntity : AbilityEntity
{
    public Transform[] path; 
    public Vector3[] path2;
    // Start is called before the first frame update
    /*void Start()
    {
        //iTween.MoveTo(this.gameObject, iTween.Hash("path", path, "time", 3, "easetype", iTween.EaseType.easeInCubic, "isLocal", true));
        path2.Add(new Vector3)
    }

    // Update is called once per frame
    void Update()
    {

        iTween.MoveTo(this.gameObject, iTween.Hash("path", path, "time", 3, "easetype", iTween.EaseType.easeInCubic, "isLocal", true));
    }*/

    public override void UpdateAbilityEntity()
    {
        
    }

    public override void InitializeAbilityEntity()
    {
        //Tested the below moveTo value with Vector3.forward. This provides the movement interaction we want. (movement follows parent)
        //iTween.MoveTo(this.gameObject, iTween.Hash("position", Vector3.forward*3, "time", 3, "easetype", iTween.EaseType.easeInCubic, "isLocal", true));
        
        iTween.MoveTo(this.gameObject, iTween.Hash("path", path2, "time", 3, "easetype", iTween.EaseType.easeInCubic, "isLocal", true, "looptype", iTween.LoopType.none));
    }

    void OnDrawGizmos()
    {
        iTween.DrawPathGizmos(path2);
    }
}
