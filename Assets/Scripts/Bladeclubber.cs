using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Bladeclubber : MonoBehaviour
{
    public Transform Head;

    public LayerMask Player;
    
    public BoxCollider hitBox;

    private NavMeshAgent navMeshAgent;
    private Transform target;

    public float attackCombo = 1;

    public Transform[] weaponsL;
    public Transform[] weaponsR;

    public Animator animator;
    public float combatTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(this.GetComponent<NavMeshAgent>() != null)
        {
            print("Assinged nav mesh");
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        }
        if(this.GetComponent<Animator>() != null)
        {
            this.animator = this.GetComponent<Animator>();
        }
        //if(this.GetComponent<>)
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            AssignTarget();
        }

        SeekPlayer();

        EngageCombat();


        
    }

    void SeekPlayer()
    {
        if(this.navMeshAgent != null && target != null)
        {
            transform.LookAt(target);
            Vector3 position = transform.position + hitBox.center;
            //RaycastHit hit;
            float targetDistance = (transform.position - target.position).magnitude;

            /*if(Physics.SphereCast(position, 15f, transform.forward, out hit, 15f, Player))
            {
        
                targetDistance = hit.distance;
                print("PlayerHit");
                print(hit.transform);
            }*/

            //print("Sphere Cast reported target distance: " + targetDistance);
            //print("Manual calculation of distance: " + (transform.position - target.transform.position).magnitude);

            if(targetDistance >= 15)
            {
                //print("Navigate to player");
                if(navMeshAgent.enabled)
                {
                    navMeshAgent.SetDestination(target.transform.position);
                    
                }
                //navMeshAgent.stoppingDistance = 5;
            }

            if(targetDistance < 15 && targetDistance > 5 && !this.animator.GetBool("Attacking")) 
            {
                //navMeshAgent.
                //print("Avoid player");
                //OrbitTarget();
            }

            
            /*if(targetDistance < 5)
            {
                EngageCombat();                      
            } 
            else
            {
                //DisengageCombat();
            }*/

            

        }
    }

    void AssignTarget()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("P1"))
        {
            if(obj.name == "Eston")
            {
                target = obj.transform;
                //print("Found Eston, seeking her");
            }
        }
    }

    void OrbitTarget()
    {
        transform.RotateAround(target.transform.position, Vector3.up, 60 * Time.deltaTime);
        //print("Orbiting Target: " + target.transform.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + this.hitBox.center;
        Gizmos.DrawWireSphere(position, 15f);

    }

    void EngageCombat()
    {
        float targetDistance = (transform.position - target.position).magnitude;
        
        //Slide forward with combo
        //navMeshAgent.SetDestination(new Vector3(0,0,3));
        if(this.animator.GetBool("Attacking"))
        {
            navMeshAgent.SetDestination(target.transform.position);
            //navMeshAgent.stoppingDistance = 1;
            //navMeshAgent.enabled = false;
            
            //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
        else
        {
            navMeshAgent.enabled = true;
            //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        //Start Combat String 1
        //Play the animation
        if(targetDistance < 5)
        {
            if(!this.animator.GetBool("Attacking"))
            {
                animator.SetTrigger("Initiate-AttackString");
                animator.SetBool("Attacking", true);
                //animator.Play("Base Layer.AttackStringI");
                this.attackCombo = 0;   
            }
        }


        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringI"))
        {
            print("Current Animation State is: Attack String I");
            weaponsR[0].GetComponent<Weapon>().EnableWeaponTrail();
            weaponsR[0].GetComponent<BoxCollider>().enabled = true;

            weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsL[0].GetComponent<BoxCollider>().enabled = false;

            transform.LookAt(target.GetComponent<ThirdPersonMovement>().Body);
            print("Target Box Collider Center: " + target.GetComponent<ThirdPersonMovement>().Body);
            print("Target Box Collider Position: " + target.GetComponent<BoxCollider>().transform.position);
        }

        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringII"))
        {
            print("Current Animation State is: Attack String II");
            weaponsL[0].GetComponent<Weapon>().EnableWeaponTrail();
            weaponsL[0].GetComponent<BoxCollider>().enabled = true;

            weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsR[0].GetComponent<BoxCollider>().enabled = false;

            //Currently Bladeclubber looks a bit above the player character. This causes the Axe swing in AttackStringII to uppercut the player to the sky
            transform.LookAt(target);


        }

        if(!this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringI") && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringII"))
        {

            animator.SetBool("Attacking", false);
            //print("Not attacking");
            weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
            //animator.ResetTrigger("Initiate-AttackString");

            weaponsL[0].GetComponent<BoxCollider>().enabled = false;
            weaponsR[0].GetComponent<BoxCollider>().enabled = false;

            weaponsL[0].GetComponent<Weapon>().ResetHitCounters();
            weaponsR[0].GetComponent<Weapon>().ResetHitCounters();
        }
        
        //Create a random chance for the combo string to happen
        
        //Transition to combat string 2
        //Play the animation
    }

    void DisengageCombat()
    {
        animator.SetBool("Attacking", false);
        //weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
        //weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
    }
}
