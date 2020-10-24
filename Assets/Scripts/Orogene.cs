using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class Orogene : MonoBehaviour
{
    public float thrust = 1.0f;
    public float torusDescentSpd = 20.0f;
    public float torusExpanseSpd = 20.0f;
    public float torusSpinSpd = 20.0f;

    public Transform camReference;
    public Transform movementReference;

    //public Transform rock;
    //private Rigidbody rockRB;
    public Transform torus;

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

        if(Input.GetKey(KeyCode.LeftShift)){
            //castTorus();
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
        if(collisions.Count != 0){
            foreach(Collision collision in collisions){
                Rigidbody earthernElement = collision.collisionHit.GetComponent<Rigidbody>();
                direction = (collision.collisionHit.transform.position - transform.position).normalized;
                earthernElement.AddForce(-direction * thrust); 
            }
        } 
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

    /*Cast Torus*/
    void castTorus(){
        //SpawnTorus at Torus Position
        //Move Torus into earth
        torus.transform.position += new Vector3(0, -1 * torusDescentSpd * Time.deltaTime,0);

        //Increase size of torus
        float expandFactor = 1 * torusExpanseSpd * Time.deltaTime;
        torus.transform.localScale += new Vector3(expandFactor, expandFactor, expandFactor);

        //Rotate the torus
        //torus.transform.rotation = Quaternion.Euler(0f, 1 * torusSpinSpd * Time.deltaTime, 0f);
        torus.transform.Rotate(0, 1*torusSpinSpd*Time.deltaTime, 0f, Space.World);

        //Set FreeLook Camera to Torus
        camReference.GetComponent<CinemachineFreeLook>().m_Follow = torus.GetChild(0);
        camReference.GetComponent<CinemachineFreeLook>().m_LookAt = torus.GetChild(0);

        //Set Movable Player
        movementReference.gameObject.SendMessage("toggleMovement", false);
        torus.GetChild(0).gameObject.SendMessage("toggleMovement", true);

    }

    /* Target Cone Theory*/
    // Start by casting a physics.OverlapSphere call
    // You then need to take a dot product to specify what angle really matters
    // How do you then check if an object is in front or behind? 
    // I want a front radial cone to act as the selection area for a push
    // I want to throw a radial circle gizmo (Debug Draw Line) that dictates 
    // You could literally raycast out in an arc



}
