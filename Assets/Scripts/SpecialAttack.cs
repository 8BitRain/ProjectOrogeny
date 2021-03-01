using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SpecialAttack : MonoBehaviour
{
    //Special Attack Settings
    [Header("Special Attack Settings")]
    public System.String specialAttackName;
    private float specialAttackTimer = 0;
    public float specialAttackSpeed = 5;
    private bool specialAttackStarted = false;
    public float specialAttackDuration = 15;

    //Special Attack Spawn Settings
    [Header("Special Attack Type Settings")]
    public SkillType skillType;
    public enum SkillType {Beam, Throw, Physical, Emission};


    public Transform spawnLocation;

    //References
    public Transform skillUser;
    public Animator skillUserAnimator;


    public AnimationClip specialAttackAnimation;

    private bool firePalmIntoDistance = false;
    private bool cosmicPalmInput = false;

    //Beam Configuration TODO add conditional
    private Transform _beam;

    //Emmision Configuration
    [Header("Emission Settings")]
    public float thrust = 10000;

    [Header("Throw Settings")]
    public float maxThrowDistance = 100;
    private bool thrown = false;
    private bool startRecoil = false;
    private bool _isRecoiling = false;


    void Start()
    {

        if(skillType == SkillType.Beam)
        {
            InitializeBeam();
        }

        if(skillType == SkillType.Throw)
        {
            InitializeThrow();
        }

    }

    void Update()
    {
        skillUserAnimator = skillUser.gameObject.GetComponent<Animator>();
        if(skillType == SkillType.Beam)
        {
            UpdateBeam();
        }

        //Special Attack Timer
        if(specialAttackTimer > 0 && specialAttackStarted)
        {
            specialAttackTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if(skillType == SkillType.Emission)
        {
            print("Emitting");
            this.GetComponent<Rigidbody>().AddForce(skillUser.transform.forward * thrust);
        }

        if(skillType == SkillType.Throw)
        {
            //print("throwing");
            UpdateThrow();
        }
    }

    public float GetSkillDuration()
    {
        return this.specialAttackDuration;
    }

    public bool GetSkillUserAnimationState()
    {
        //return value of animation state here


        return false;
    }

    public void SetSkillUserAnimator(Animator animator)
    {
        this.skillUserAnimator = animator;
    }

    public void SetSkillUser(Transform skillUser)
    {
        print("Skill User: " + skillUser.gameObject.GetComponent<Animator>());
        this.skillUser = skillUser;
    }

    public float GetSpecialAttackTimer()
    {
        return specialAttackTimer;
    }

    public void SetSpecialAttackTimer(float timer)
    {
        this.specialAttackTimer = timer;
    }

    public void SetLinkedAnimationClip(AnimationClip ac)
    {
        this.specialAttackAnimation = ac;
    }

    public void EnableSpecialAttack()
    {
        this.specialAttackStarted = true;
    }

    public void DisableSpecialAttack()
    {
        this.specialAttackStarted = false;
    }

    public void SetSpawnLocation(Transform spawnLocation)
    {
        this.spawnLocation = spawnLocation;
    }

    /*BEAM SKILL DEFINITIONS*/
    public void InitializeBeam()
    {
        //Start special attack timer
        specialAttackTimer = specialAttackAnimation.length;

        _beam = this.transform.GetChild(0);
        if(_beam.gameObject.activeSelf)
        {   
            //Set the laser beam to inactive. It should only form once the palm animation has finished. That way we get the anticipation effect of ki building in the palm.
            _beam.gameObject.SetActive(false);
        }
    }

    public void UpdateBeam()
    {
        //The beam updating depends upon the animation state CosmicPalmAttack. For now this move is animation locked.
        if(skillUserAnimator.GetBool("CosmicPalmAttack"))
        {
            //Check timer to make sure we can summon the beam. We want to summon the beam once the cosmic palm strike animation completes
            if(skillUser.GetComponent<ThirdPersonMovement>().GetActionTimer() >= specialAttackAnimation.length)
            {
                if(!_beam.gameObject.activeSelf)
                {
                    _beam.gameObject.SetActive(true);
                }
            }
            this.transform.position = spawnLocation.position;
            this.transform.rotation = spawnLocation.rotation;

            //If the beam is active, have it grow larger and shoot further into the distance
            if(_beam.gameObject.activeSelf)
            {
                LineRenderer lineRenderer = _beam.GetComponent<LineRenderer>();
                //https://docs.unity3d.com/ScriptReference/LineRenderer.SetPosition.html
                //initialize length of beam to 0
                float previousBeamLength = lineRenderer.GetPosition(1).z;
                float updatedBeamLength = previousBeamLength + (specialAttackSpeed * Time.deltaTime);

                //Ensure hitbox of spawned beam moves
                this.GetComponent<BoxCollider>().center = new Vector3(0,0,updatedBeamLength);
                Vector3 beamLength = new Vector3(0,0,updatedBeamLength);
                lineRenderer.SetPosition(1,beamLength);
            }

            //let's have the character look at where the beam is firing
            //transform.rotation = Quaternion.Euler(0, _cosmicPalmBeamSpawnLocation.transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(spawnLocation.transform.rotation.eulerAngles.x, spawnLocation.transform.rotation.eulerAngles.y, 0);
        }
        if(!skillUserAnimator.GetBool("CosmicPalmAttack"))
        {
            //reset player's rotation to an upright neutral position. Experiment :D You can even create a flip animation to flip back into a neutral rotation for flair.
            transform.rotation = Quaternion.Euler(0,0,0);
            DischargeBeam();
        }
    }

    public void DischargeBeam()
    {
        
        //Destroy the beam after 3 seconds
        Destroy(this.gameObject, 3);
        skillType = SkillType.Emission;
        print("Beam skill type has become: " + SkillType.Emission);
        
    }

    public void InitializeThrow()
    {
        this.transform.rotation = Quaternion.Euler(270,0,0);
    }

    public void UpdateThrow()
    {
        float currThrowDistance = (this.transform.position - skillUser.transform.position).magnitude;
        //maxDistance not reached Throw away from skillUser 
        if(currThrowDistance < maxThrowDistance && !thrown)
        {
            this.GetComponent<Rigidbody>().AddForce(skillUser.transform.forward * thrust, ForceMode.Impulse);
            this.GetComponent<Rigidbody>().AddTorque(this.transform.TransformDirection(Vector3.forward) * 50, ForceMode.Impulse);
            thrown = true;
        }
        
        //maxDistance Reached Return to skillUser
        if(currThrowDistance >= maxThrowDistance)
        {
            startRecoil = true;
        }

        //Recoil from maxDistance & return thrown object to player
        if(startRecoil)
        {
            //Look at the player and return to the player
            //this.transform.rotation = Quaternion.Euler(0,0,0);
            //this.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.LookAt(skillUser.transform.position);
            //this.GetComponent<Rigidbody>().AddForce(this.transform.forward * -thrust*2);
            this.GetComponent<Rigidbody>().AddForce(transform.forward * thrust*2, ForceMode.Impulse);
            print("Recoiling Thrown Object");
            startRecoil = false;
            _isRecoiling = true;
        }


        //Destroy Game Object once it has returned to player
        if(currThrowDistance <= 10 && _isRecoiling)
        {
            skillUserAnimator.Play("Grounded.pull");
            _isRecoiling = false;
            thrown = false;
            DisableThrow();
        }


    }

    public void DisableThrow()
    {
        Destroy(this.gameObject);
    }




}