using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public RectTransform reticle;
    public LayerMask Bounds;
    public GameObject Target;
    public Camera camera;

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
        }
        Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.red);


    }
}
