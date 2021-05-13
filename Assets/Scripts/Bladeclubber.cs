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

    protected NavMeshAgent navMeshAgent;
    protected Transform target;

    public float attackCombo = 1;


    public Transform[] weaponsL;
    public Transform[] weaponsR;
    public bool hitBoxEnabled = false;

    public Animator animator;
    public float combatTimer = 0;

    public HealthBar healthBar;
    public PoiseMeter poiseMeter;

    protected bool _floatState = false;
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
        if(!_floatState)
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
    }

    public void SeekPlayer()
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

    public void AssignTarget()
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

    public void OrbitTarget()
    {
        transform.RotateAround(target.transform.position, Vector3.up, 60 * Time.deltaTime);
        //print("Orbiting Target: " + target.transform.position);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + this.hitBox.center;
        Gizmos.DrawWireSphere(position, 15f);

    }

    public void EngageCombat()
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
            //TODO: Enable this navmesh to have the agent reset to the track
            //navMeshAgent.enabled = true;
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

    public void DisengageCombat()
    {
        animator.SetBool("Attacking", false);
        //weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
        //weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
    }

    public void EnableHitBoxes()
    {
        Debug.Log("HitBoxEnabled");
        hitBoxEnabled = true;
    }

    public void DisableHitBoxes()
    {
        Debug.Log("HitBoxDisabled");
        hitBoxEnabled = false;
    }

    public void HandleHitStun()
    {
        //Shake the target
        Debug.Log("Combat: Poise UP add some hitstun shake");
        Debug.Log("Combat: NavMesh Value: " + transform.GetComponent<NavMeshAgent>().enabled);
        transform.GetComponent<NavMeshAgent>().enabled = false;
        Debug.Log("Combat: NavMesh Value: " + transform.GetComponent<NavMeshAgent>().enabled);

        StartCoroutine(HitStunEnumerator(10));


    }

    IEnumerator HitStunEnumerator (int frames) 
    {
        int count = frames;
        while (count > 0) {
            float shakeFactor = Mathf.Sin(Time.time * 2f) * .1f;
            Debug.Log("HitStun: " + transform.name + " Frame: " + count + " Timestamp: " + Time.time);
            Vector3 objectShakeVector = new Vector3(transform.transform.position.x + shakeFactor , transform.transform.position.y + shakeFactor, transform.transform.position.z);
            transform.transform.position = objectShakeVector;
            Debug.Log("HitStun: Shake vector: " + objectShakeVector);
            count--;
        }
        transform.GetComponent<NavMeshAgent>().enabled = true;

        yield return null;


    }

    public void HandleFloatState(GameObject aggresor)
    {
        Debug.Log("Combat: Floating Target");
 
        transform.GetComponent<NavMeshAgent>().enabled = false;
        //throwing in if block to limit accidental triggers
        if(!_floatState)
        {
            StartCoroutine(FloatStateEnumerator(7, aggresor.GetComponent<ThirdPersonMovement>()));
        }
    }

    IEnumerator FloatStateEnumerator (float time, ThirdPersonMovement aggresor) 
    {
        _floatState = true;

        Vector3 startingPos  = transform.position + (Vector3.up * 5);
        Vector3 finalPos = transform.position + (Vector3.up * 15);
        float elapsedTime = 0;
        
        Debug.Log("Combat: Floating Target");

        //knock the target up.
        Debug.Log("Combat: Float Force " + 300);
        transform.GetComponent<Rigidbody>().useGravity = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        //transform.GetComponentInParent<Rigidbody>().AddForce((Vector3.up) * 200);
        transform.rotation = Quaternion.Euler(0,0,90);


        Vector3 startingRotation = new Vector3(0,0,0);
        Vector3 endingRotation = new Vector3(0,30,0);
        
        //Rise
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));

            //Sping the target as its being knocked up
            transform.Rotate(Vector3.Lerp(endingRotation, startingRotation, (elapsedTime / time))); 
            //transform.Rotate(endingRotation); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Fall
        elapsedTime = 0;
        time = 2;
        finalPos = transform.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(finalPos, startingPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //tilt the target so it looks like its spinning.
        transform.GetComponentInParent<Rigidbody>().useGravity = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        
        transform.GetComponent<NavMeshAgent>().enabled = true;
        _floatState = false;

        aggresor.DisengageDynamicCameraTargetFloating();


    }
    
}
