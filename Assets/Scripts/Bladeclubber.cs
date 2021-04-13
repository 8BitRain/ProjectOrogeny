using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Bladeclubber : MonoBehaviour
{
    public Transform Head;
    public Transform Body;

    public LayerMask Player;
    
    public BoxCollider hitBox;

    private NavMeshAgent navMeshAgent;
    private Transform target;

    public float attackCombo = 1;


    public Transform[] weaponsL;
    public Transform[] weaponsR;
    public bool hitBoxEnabled = false;

    public Animator animator;
    public float combatTimer = 0;

    public HealthBar healthBar;
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

        if(!this.animator.GetBool("Attacking"))
        {
            SeekPlayer();   
        }
        

        EngageCombat();


        
    }

    void SeekPlayer()
    {
        if(this.navMeshAgent != null && target != null)
        {
            transform.LookAt(target);

            Vector3 position = transform.position + hitBox.center;
            float targetDistance = (transform.position - target.position).magnitude;

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
        //sentinel value
        float targetDistance = -1;
        if(target != null)
        {
            targetDistance = (transform.position - target.position).magnitude;
        }
        
        
        //Slide forward with combo
        //navMeshAgent.SetDestination(new Vector3(0,0,3));
        if(this.animator.GetBool("Attacking"))
        {
            //Force the target into a combat state
            //TODO: Test with multiple combatants. How does the state respond?
            target.GetComponent<ThirdPersonMovement>().SetCombatState(true, this.transform);
        }
        else
        {
            //TODO: Test with multiple combatants. How does the state respond?
            //Concern:
            if(target != null)
            {
                target.GetComponent<ThirdPersonMovement>().SetCombatState(false, null);
            }
            navMeshAgent.enabled = true;
            //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        //Start Combat String 1
        //Play the animation
        if(targetDistance < 5 && targetDistance != -1)
        {
            if(!this.animator.GetBool("Attacking"))
            {
                animator.SetTrigger("Initiate-AttackString");
                animator.SetBool("Attacking", true);
                animator.SetBool("WindUp-AttackStringI", true);
                //animator.Play("Base Layer.AttackStringI");
                this.attackCombo = 0;   
            }
        }


        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringI"))
        {
            //Get the time duration of AttackStringII
            AnimatorStateInfo combatAnimatorStateInfo = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            float combatantAnimationTimeElapsed = combatAnimatorStateInfo.length;

            print("Combatant Time Elapsed: " + combatantAnimationTimeElapsed);

            print("Current Animation State is: Attack String I");
            weaponsR[0].GetComponent<Weapon>().EnableWeaponTrail();
            
            //Hitbox only activates when triggered by an event in the Bladeclubber animation. This way we have control over the timing without having to do timing calculations here.
            if(hitBoxEnabled)
            {
                weaponsR[0].GetComponent<BoxCollider>().enabled = true;
            } else 
            {
                weaponsR[0].GetComponent<BoxCollider>().enabled = false;
            }
            

            weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsL[0].GetComponent<BoxCollider>().enabled = false;

            if(animator.GetBool("WindUp-AttackStringI"))
            {
                navMeshAgent.SetDestination(target.transform.position);
                transform.LookAt(target.GetComponent<ThirdPersonMovement>().Body.transform.position);
                animator.SetBool("WindUp-AttackStringI", false);
                //animator.SetBool("WindUp-AttackStringII", true);
                //Cache wind up for second hit
            }
            //transform.LookAt(target.GetComponent<ThirdPersonMovement>().Body.transform.TransformPoint(target.GetComponent<ThirdPersonMovement>().Body.transform.position));
            print("Bladeclubbers's position: " + this.transform.position);
            print("Target's Body Position: " + target.GetComponent<ThirdPersonMovement>().Body.transform.position);
            print("Target's Box Collider Position: " + target.GetComponent<BoxCollider>().transform.position);
        }

        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringII"))
        {
            //Get the time duration of AttackStringII
            AnimatorStateInfo combatAnimatorStateInfo = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            float combatantAnimationTimeElapsed = combatAnimatorStateInfo.length;
            //if(!animator.GetBool())
            print("Current Animation State is: Attack String II");
            weaponsL[0].GetComponent<Weapon>().EnableWeaponTrail();

            //Hitbox only activates when triggered by an event in the Bladeclubber animation. This way we have control over the timing without having to do timing calculations here.
            if(hitBoxEnabled)
            {
                weaponsL[0].GetComponent<BoxCollider>().enabled = true;
            } else 
            {
                
                weaponsL[0].GetComponent<BoxCollider>().enabled = false;
            }

            weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsR[0].GetComponent<BoxCollider>().enabled = false;

            if(animator.GetBool("WindUp-AttackStringII"))
            {
                //Currently Bladeclubber looks a bit above the player character. This causes the Axe swing in AttackStringII to uppercut the player to the sky
                GameObject aboveTarget = new GameObject();
                //aboveTarget.transform.position = 
                transform.LookAt(target.transform.position + transform.up);
                animator.SetBool("WindUp-AttackStringII", false);
            }
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

    void EnableHitBoxes()
    {
        Debug.Log("HitBoxEnabled");
        hitBoxEnabled = true;
    }

    void DisableHitBoxes()
    {
        Debug.Log("HitBoxDisabled");
        hitBoxEnabled = false;
    }
}
