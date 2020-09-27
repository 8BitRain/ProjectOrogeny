using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Orogene : MonoBehaviour
{
    public float thrust = 1.0f;

    //public Transform rock;
    //private Rigidbody rockRB;

    public List<Collision> collisions = new List<Collision>();


    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        //rockRB = rock.GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if(Input.GetButton("Pull")){
            pull_obj();
        }

        if(Input.GetButton("Push")){
            push_obj();
        }
        
    }

    void push_obj()
    {
       
        print("Push");
        Animator anim = this.GetComponent<Animator>();
        //anim.StopPlayback;
        anim.Play("push");
        //Loop through objects in list
        if(collisions.Count != 0){
            foreach(Collision collision in collisions){
                Rigidbody earthernElement = collision.collisionHit.GetComponent<Rigidbody>();
                direction = (collision.collisionHit.transform.position - transform.position).normalized;
                earthernElement.AddForce(direction * thrust); 
            }
        }
    }

    void pull_obj()
    {
        print("Pull");
        Animator anim = this.GetComponent<Animator>();
        //anim.StopPlayback;
        anim.Play("push");
        //rockRB.MovePosition(rockRB.transform.position - transform.forward * thrust * Time.deltaTime);
        //direction = (rock.transform.position - transform.position).normalized;
        //rockRB.AddForce(-direction * thrust); 
    }

    /* Add Earthern Element to Target Cone*/
    void addEarthernElement(Collision collision){
        this.collisions.Add(collision);
        print("Added collision");
    }

    /* Remove Earthern Element from Target Cone*/
    void removeEarthernElement(Collision collision){
        //Loop through current list of collisions and remove specified collison
        foreach(Collision earthernElement in collisions.ToList<Collision>()){
            if(collision.colliderName == earthernElement.colliderName 
            && collision.collisionHit.name == collision.collisionHit.name){
                print(this.collisions.Remove(earthernElement));
                print("Removed collision: " + collision.collisionHit.name);
            }
        }
    }

    /* Target Cone Theory*/
    // Start by casting a physics.OverlapSphere call
    // You then need to take a dot product to specify what angle really matters
    // How do you then check if an object is in front or behind? 
    // I want a front radial cone to act as the selection area for a push
    // I want to throw a radial circle gizmo (Debug Draw Line) that dictates 
    // You could literally raycast out in an arc



}
