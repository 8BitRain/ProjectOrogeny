using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orogene : MonoBehaviour
{
    public float thrust = 1.0f;

    public Transform rock;
    private Rigidbody rockRB;

    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        rockRB = rock.GetComponent<Rigidbody>();


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
        //Force
        //rockRB.AddForce(transform.forward * thrust); 
        //Move Position
        //rockRB.MovePosition(rockRB.transform.position + transform.forward * thrust * Time.deltaTime);
        direction = (rock.transform.position - transform.position).normalized;
        rockRB.AddForce(direction * thrust); 
    
    }

    void pull_obj()
    {
        print("Pull");
        Animator anim = this.GetComponent<Animator>();
        //anim.StopPlayback;
        anim.Play("push");
        //rockRB.MovePosition(rockRB.transform.position - transform.forward * thrust * Time.deltaTime);
        direction = (rock.transform.position - transform.position).normalized;
        rockRB.AddForce(-direction * thrust); 
    }


}
