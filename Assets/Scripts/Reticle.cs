using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public RectTransform reticle;
    public LayerMask Bounds;
    public GameObject Target;
    public GameObject prevTarget;
    public Camera camera;

    public Material highlightMaterial;
    public Material previousMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit touchingBoundsRaycast;
        bool _isTouchingBounds = Physics.Raycast(camera.transform.position, camera.transform.forward, out touchingBoundsRaycast, 200f, Bounds);
        if(_isTouchingBounds)
        {
            print("Reticle colliding with: " + touchingBoundsRaycast.transform.name);
            Target = touchingBoundsRaycast.transform.gameObject;

            if(prevTarget == null)
            {
                Debug.Log("Previous Target is null");
                prevTarget = Target;
                previousMaterial = Target.GetComponent<Renderer>().material;
            } 
            if(Target != prevTarget)
            {
                Debug.Log("Previous Target is " + prevTarget.name);
                prevTarget.GetComponent<Renderer>().material = previousMaterial;
                prevTarget = null;
                prevTarget = Target;
                Debug.Log("Previous Target has been reset to " + prevTarget.name);
            }

            if(Target == prevTarget)
            {
            }

            Target.GetComponent<Renderer>().material = highlightMaterial;
            

        } else
        {
            //Adding this line really changed everything. I need to investigate how accurate this reticle is
            Target = null;
            if(prevTarget != null)
            {
                prevTarget.GetComponent<Renderer>().material = previousMaterial;
            }

        }
        Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.red);


    }
}
