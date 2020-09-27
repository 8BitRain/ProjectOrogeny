using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeHitDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "EarthernElement"){
            Collision collision = new Collision(other.name, other.gameObject);
            Debug.Log(this.gameObject.name + "collided with: " + other.gameObject.name);
            this.SendMessageUpwards("addEarthernElement", collision);
        }
    }

    void OnTriggerExit(Collider other){
        if(other.tag == "EarthernElement"){
            Debug.Log(this.gameObject.name + "exited: " + other.gameObject.name);
            Collision collision = new Collision(other.name, other.gameObject);
            this.SendMessageUpwards("removeEarthernElement", collision);
        }
    }
}
