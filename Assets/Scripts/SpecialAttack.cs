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

    [Header("Special Attack Mobility Settings")]
    public MobilityType mobilityType;
    public enum MobilityType {Mobile, Immobile, Mixed};


    [HideInInspector]public Transform spawnLocation;
    [HideInInspector]public Transform skillUser;
    [HideInInspector]public Animator skillUserAnimator;


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
    private Transform maxDistanceTransform;
    private bool thrown = false;

    //Recoil Thrown Object
    [Header("Recoil Thrown Object Settings")]
    public GameObject kickbackSpecialHintVFX;
    public GameObject kickbackSpecialVFX;
    private bool startRecoil = false;
    private bool _isRecoiling = false;
    private float recoilLerp = 0;
    private ParticleSystem ps;
    private bool canKnockBackThrownObject = false;

    [Header("Debug Info Toggle")]
    public bool toggleDebug = false;



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
            this.GetComponent<Rigidbody>().AddForce(skillUser.transform.forward * thrust);
        }

        if(skillType == SkillType.Throw)
        {
            UpdateThrow();
        }
    }

    public MobilityType GetMobilityType()
    {
        return this.mobilityType;
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
    //TODO: Create an override for Initialize for special ability. This way Initialize can be reused for Beam and Throw
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

    //TODO: This can be made overridable so a child class can implement its own update function. A good word for this interaction is that the attack is channeled. 
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

    //TODO: Write an override for this class in a child class. Throw Ability
    public void InitializeThrow()
    {
        this.transform.rotation = Quaternion.Euler(270,0,0);
        ps = kickbackSpecialHintVFX.GetComponent<ParticleSystem>();
    }

    //TODO: Write an override for this class in a child class. Throw Ability
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
            maxDistanceTransform = this.transform;
        }

        //Recoil from maxDistance & return thrown object to player
        if(startRecoil)
        {
            //Look at the player and return to the player
            //this.transform.rotation = Quaternion.Euler(0,0,0);
            //this.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.LookAt(skillUser.transform.position);
            //this.GetComponent<Rigidbody>().AddForce(this.transform.forward * -thrust*2);
            //this.GetComponent<Rigidbody>().AddForce(transform.forward * thrust*2, ForceMode.Impulse);

            //ps.Play();
            //var em = ps.emission;
            //em.enabled = true;

            //print("Particle System, " + ps);
            print("Recoiling Thrown Object");
            startRecoil = false;
            _isRecoiling = true;
        }

        if(_isRecoiling)
        {
            BezierCurve bezierCurve = new BezierCurve();
            bezierCurve.startPoint = maxDistanceTransform;
            bezierCurve.endPoint = skillUser.GetComponent<ThirdPersonMovement>()._shieldReturnRecoilEnd.transform;
            bezierCurve.controlPointStart = skillUser.GetComponent<ThirdPersonMovement>()._shieldReturnControlPointStart.transform;
            bezierCurve.controlPointEnd = skillUser.GetComponent<ThirdPersonMovement>()._shieldReturnControlPointEnd.transform;

            //Move the controlPointEnd point to have the thrown object follow in front of player to kick.
            if(skillUserAnimator.GetCurrentAnimatorStateInfo(0).IsName("kickThrownSpecialAttack"))
            {
                bezierCurve.controlPointEnd = skillUser.GetComponent<ThirdPersonMovement>()._shieldReturnKickControlPointEnd.transform;
            }

            //print(recoilLerp);
            if(this.recoilLerp < 1)
            {
                recoilLerp += Time.deltaTime / specialAttackSpeed;
                //transform.position = Vector3.Lerp(transform.position, skillUser.transform.position, recoilLerp);
                transform.position = bezierCurve.DeCasteljausAlgorithm(bezierCurve.startPoint.position, bezierCurve.controlPointStart.position, bezierCurve.controlPointEnd.position, bezierCurve.endPoint.position, recoilLerp);
                //print(transform.position);
            }
            else
            {
                recoilLerp = 0;
                //this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //transform.LookAt(bezierCurve.endPoint);
                _isRecoiling = false;
            }

            //Turn on particle effect, this throwable object can be kicked again.
            if(currThrowDistance <= 50 && currThrowDistance > 5)
            {
                //print("Emit particles");
                canKnockBackThrownObject = true;
                ToggleHintVFX(true);

                //Debug.DrawRay(transform.position, transform.forward * (this.GetComponent<BoxCollider>().center.z + 2f), Color.red);
            }
        }


        //Destroy Game Object once it has returned to player
        if(currThrowDistance <= 5 && _isRecoiling)
        {
            canKnockBackThrownObject = false;
            //Turn off particles
            ToggleHintVFX(false);

            if(skillUserAnimator.GetCurrentAnimatorStateInfo(0).IsName("kickThrownSpecialAttack"))
            {
                print("DONT PLAY SHIELD GRAB");
            }
            else
            {
                skillUserAnimator.Play("Grounded.pull");
            }
            _isRecoiling = false;
            thrown = false;
            DisableThrow();
        }


    }

    public void DisableThrow()
    {
        Destroy(this.gameObject);
    }

    //TODO: This could be broken out into its own class. TO assist with interactable elements in game
    public void ToggleHintVFX(bool vfx)
    {
        if(vfx)
        {
            ps.Play();
            var em = ps.emission;
            em.enabled = true;
        }

        if(!vfx)
        {
            ps.Stop();
            var em = ps.emission;
            em.enabled = false;
            Destroy(ps.gameObject);
        }
    }

    public bool GetKnockbackThrownObjectStatus()
    {
        return this.canKnockBackThrownObject;
    }

    //Collisions
    void OnCollisionEnter(UnityEngine.Collision col)
    {
        //print("HIT: " + collision.collisionHit.name);
        if(col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            print("Collided with Wall");
            ContactPoint contact = col.contacts[0];
            maxDistanceTransform = this.transform;
            startRecoil = true;
        }
        //this.GetComponent<Rigidbody>().isKinematic = true;
    }




}