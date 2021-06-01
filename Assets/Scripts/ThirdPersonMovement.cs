using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ThirdPersonMovement : MonoBehaviour
{
    //Movement
    [Header("Movement Settings")]
    public float speed = 6.0f;
    private float defaultSpeed;
    public float acceleration = .1f;
    public float friction = .025f;
    public float gravity = -9.81f;
    public bool applyGravity = true;
    private bool dashInput = false;
    private Vector3 axis;
    private bool _isTilting = false;
    private Vector2 tiltRotationSpeed;


    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Dash Settings")]
    public float dashSpeed = 6.0f;
    private float defaultDashSpeed;
    public float maxSpeed = 10.0f;
    public float dashFriction = .025f;
    public Transform glideAfterImage;
    private float _afterImageTimer = 0;
    private float _dashTimer = 0;
    private bool glide = false;

    //Jump Related Code
    [Header("Jump Settings")]
    public Transform _groundChecker;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public float JumpHeight = 2f;
    private Vector3 _velocity;
    private bool _isGrounded = true;
    private bool jumpInput = false;

    public GameObject _jumpVFX;

    //Mantling 
    [Header("Ledge Grab (Mantling) Settings")]
    public LayerMask Ledge;
    public float ledgeDistance = 2f;
    private bool _isMantling = false;
    public enum MantleType {High, Mid};
    public MantleType mantleType;
    private float _mantleTimer = 0;


    //Movement Related
    private bool moveCharacter;
    private Vector2 movementInput;

    //Wall Running
    [Header("WallRunning Settings")]
    public Transform _wallRunChecker;
    public float wallDistance = 0.2f;
    public LayerMask Wall;
    public float WallRunMaxHeight = 1f;
    bool isWallRight;
    bool isWallLeft;
    bool isWallInFront;
    private Vector3 wallVector;
    private Vector3 wallJumpDirection;
    private bool _canWallRun = false;
    private bool _isWallRunning = false;

    //Enviromental EFX
    [Header("Enviromental EFX")]
    public float cosmicWindDistance = .1f;
    private bool isCosmicWindFloating = false;
    public float windRiseSpeed = 1f;
    //Dictates y velocity so player does not sink in uprising wind. 
    //0 represents no sinking, while the min value represents sinking to a degree. -10 would represent bobbing.
    [Range(0.0f, -10.0f)] public float windResetVelocityY = 0;

    //Impact
    [Header("Enviromental EFX")]
    public float mass = 3.0f;
    Vector3 impact = Vector3.zero;

    //Button to trigger enviromental action
    [Header("Enviromental Action")]
    private bool _attachedToPlatform = false;
    private bool enviromentActionInput = false;

    [Header("VFX")]
    public GameObject speedLines;
    public GameObject dustTrails;

    //Respawn
    [Header("Respawn")]
    public LayerMask Bounds;
    public Vector3 respawnPosition;
    private bool _isTouchingBounds = false;



    //Combat
    [Header("Combat Ability Management")]
    public Transform _specialAttackSpawnLocation;
    public GameObject specialAttack;
    private GameObject instancedSpecialAttack;
    private bool displaySpecialAttackWindowInput = false;
    public GameObject specialAttackWindow;

    [Header("VFX Management")]
    public GameObject combatVFXManager;
    public Transform _vfxPositionCombo1;

    
    //Camera Management
    [Header("Camera Management")]
    public GameObject mainCamera;
    public GameObject aimCamera;
    public GameObject aimReticle;
    public GameObject freeLookCamera;
    public GameObject lockOnCamera;
    public GameObject dynamicCameraFloatingTarget;
    private bool lockOnInput = false;
    private bool targetSwitchRightInput = false;
    private bool targetSwitchLeftInput = false;
    private bool lockedOn = false;
    private int currentTarget = 0;
    private float _swapTimer = 0;
    public Transform targetToLock;
    public LayerMask Foe; 
    private Transform combatant;

    public Transform[] foes;


    //Time Management
    //TODO: Create a time manager
    [Header("Time Settings")]
     private bool slowTime;

    //Combat Management
    [Header("Combat Timer Management")]
    private float _actionTimer = 0;
    private bool startTimer = false;
    private bool firePalmIntoDistance = false;
    private bool specialAttackInput = false;
    private bool kickThrownSpecialAttackInput = false;

    [Header("Combat Settings")]
    public float maxCombatSlideSpeed = 10.0f;
    public float currentCombatSlideSpeed = 0;
    public float combatSlideFriction = 20;
    public bool combatForwardMomentum = false;
    public bool combatRecoilMomentum = false;

    public HitBox[] hitBoxes;
    private bool combatSlide = false;

    private float combatStateIndex = 0;
    
    private bool groundMeleeAttack1Input = false;
    private bool iframe = false;

    private float animationWindow = 0;
    private bool startAnimationWindow = false;

    [Header("Special Attack Settings")]
    public Transform _shieldReturnControlPointStart;
    public Transform _shieldReturnControlPointEnd;
    public Transform _shieldReturnKickControlPointEnd;
    public Transform _shieldReturnRecoilEnd;
    public Transform _shieldReturnRecoilKickSpawn;

    [Header("Character Settings")]
    public CharacterController _controller;
    public System.String characterName;
    public Transform cam;
    //This is the reference to the player so we can pull components that are children
    public Transform Orogene;
    public Transform ParentObj;

    private bool canMove = true;
    private bool canPlayerInputMove = true;
    
    /* MESSAGE: CanRotate: This variable is to help define when the player should rotate. Use this tor situations where you want the player
    *  to continue moving, but lock this scripts control of the rotation. I'm doing this so I can let the ThirdPersonCamera Script Drive Camera interaction
    *  There's probably a more effecient way to do this, but this is an experiment. 
    *
    */
    private bool canRotate = true;

    private bool playingAnim;
    private bool running;
    private float animCounter;

    [Header("Rigging Settings")]
    public Transform Body;

    [Header("Animation Settings")]
    public AnimationClip[] wallRunningAnimationClip;
    public AnimationClip[] jumpingAnimationClip;
    public AnimationClip[] combatAnimationClip;
    public Animator animator;

    [Header("Health Bar Settings")]
    public HealthBar healthBar; 
    //https://docs.unity3d.com/ScriptReference/AnimatorOverrideController.html
    protected AnimatorOverrideController animatorOverrideController;



    // Start is called before the first frame update
    void Start()
    {
        animator = Orogene.GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        //I have to generate a new Avatar at runtime because I'm overriding the runtime animator.
        Avatar avatar = AvatarBuilder.BuildGenericAvatar(this.gameObject, "");
        animator.avatar = avatar;

        defaultSpeed = speed;
        defaultDashSpeed = dashSpeed;

        tiltRotationSpeed = new Vector2(0,0);
        playingAnim = false;
        moveCharacter = true;
        //animator
        running = false;

        speedLines.SetActive(false);

        //Used to set up aiming. Reenable
        //aimReticle.SetActive(false);
        slowTime = false;

        healthBar.SetMaxHealth(100);

        //If the cam is null, instantiate a new cam. 
        /*if(this.cam == null)
        {
            this.cam =  GameObject.FindGameObjectWithTag("MainCamera").transform;
        }*/

        if(characterName.Equals(""))
        {
            characterName = "Generic";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Grounded status
        //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        RaycastHit groundedRaycast;
        _isGrounded = Physics.Raycast(_groundChecker.position, Vector3.down, out groundedRaycast, .2f, Ground);
        if(_isGrounded)
        {
            print("grounded");
            //canMove = true;
            AttachToMovingPlatform(groundedRaycast);

        }

        RaycastHit ledgeSeekerRefernce = new RaycastHit();
        //We are in the air!
        if(!_isGrounded)
        {
            if(!_isWallRunning)
            {
                if(dustTrails != null) dustTrails.SetActive(false);
            }
            //Raycast and see if we are facing a ledge
            //print("Airborne");
            RaycastHit ledgeSeekerHit;
            Vector3 _ledgeSeekerPositionHigh = new Vector3(_groundChecker.position.x, _groundChecker.position.y + this._controller.height, _groundChecker.position.z);
            Vector3 _ledgeSeekerPositionMid = new Vector3(_groundChecker.position.x, _groundChecker.position.y + this._controller.height/2.0f, _groundChecker.position.z);
            if(Physics.Raycast(_ledgeSeekerPositionHigh, _groundChecker.forward, out ledgeSeekerHit, 2f, Ledge))
            {
                print("Mantling: Grabbing Ledge");
                _isMantling = true;
                ledgeSeekerRefernce = ledgeSeekerHit;

                if(ledgeSeekerHit.transform.tag == "Asteroid")
                {
                    //ledgeReference.transform.GetComponentInParent<>
                    //Set the parent rigidbody isKinematic to true
                    print(ledgeSeekerHit.transform.name);

                    //Accessibility feature, when a player lands on a moving platform, have the platform stop moving so the player can keep their footing.
                    ledgeSeekerHit.transform.GetComponent<MeshCollider>().enabled = false;
                    ledgeSeekerHit.transform.GetComponent<Rigidbody>().isKinematic = true;
                    ledgeSeekerHit.transform.GetComponent<MeshCollider>().enabled = true;

                    //MantleType = MantleType.High;
                    mantleType = MantleType.High;
                }
            } 
            else if(Physics.Raycast(_ledgeSeekerPositionMid, _groundChecker.forward, out ledgeSeekerHit, 2f, Ledge))
            {
                print("Mantling: vaulting");
                _isMantling = true;
                ledgeSeekerRefernce = ledgeSeekerHit;

                if(ledgeSeekerHit.transform.tag == "Asteroid")
                {
                    //ledgeReference.transform.GetComponentInParent<>
                    //Set the parent rigidbody isKinematic to true
                    print(ledgeSeekerHit.transform.name);

                    //Accessibility feature, when a player lands on a moving platform, have the platform stop moving so the player can keep their footing.
                    ledgeSeekerHit.transform.GetComponent<MeshCollider>().enabled = false;
                    ledgeSeekerHit.transform.GetComponent<Rigidbody>().isKinematic = true;
                    ledgeSeekerHit.transform.GetComponent<MeshCollider>().enabled = true;
                    mantleType = MantleType.Mid;
                }
            }
            Debug.DrawRay(_ledgeSeekerPositionHigh, _groundChecker.forward, Color.green);
            Debug.DrawRay(_ledgeSeekerPositionMid, _groundChecker.forward, Color.green);
            
        }

        if(_isMantling)
        {
            if(_mantleTimer < .75f)
            {
                Mantle(ledgeSeekerRefernce);
            }

            if(_mantleTimer >= .75f)
            {
                _isMantling = false;
                canMove = true;
                print("Mantling Ended");
                _mantleTimer = 0;
            }
        }

        
        if(canMove)
        {
            //Player x and y movement OLD Unity Input Manager
            /*float horizontal = Input.GetAxisRaw("Horizontal");
            float veritical = Input.GetAxisRaw("Vertical");*/
            Vector3 direction = new Vector3(0,0,0);
            if(canPlayerInputMove)
            {
                direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            }
            

            //Grounded status
            //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

            if (_isGrounded && _velocity.y < 0)
            {
                //print("grounded");
                _velocity.y = 0f;
                animator.SetBool("Jumping", false);

                _canWallRun = false;

                //No longer falling
                if(animator.GetBool("Falling")){
                    print("Not falling");
                    animator.SetBool("Landing", true);
                    animator.SetBool("Falling", false);
                }

                if(!_isWallRunning)
                {
                    if(enviromentActionInput)
                    {
                        //speedLines.GetComponent<ParticleSys
                        speed = 20;
                        freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 80;
                        speedLines.SetActive(true);
                    }
                    else
                    {
                        speed = defaultSpeed;
                        freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 40;
                        speedLines.SetActive(false);
                    }
                }

                //Dust Trails VFX
                TriggerDustTrails(direction);

            }

            if(!_isGrounded)
            {
                _canWallRun = true;
            }

            //WALLRUNNING 2.0 using Raycasts
            if(_canWallRun){
                CheckForWall();
            }
            if(isWallRight && enviromentActionInput)
            {
                StartWallRun("right", direction);
            }
            if(isWallLeft && enviromentActionInput)
            {
                StartWallRun("left", direction);
            }
            if(isWallInFront && enviromentActionInput)
            {
                StartWallRun("front", direction);
            }
            if(!isWallLeft && !isWallRight && !isWallInFront || !enviromentActionInput)
            {
                ExitWallRun();
            }

            //MOVEMENT
            /* For acceleration and deceleration moveDir is defined by default as the local transform forward vector.*/
            /* For tighter movement, move moveDir inside the scope of the moveCharacter and direction.magnitude check*/
            //This introduces a bug similar to the super bounce! moveDir being set to transform.forward affects wall jump movement.
            //Vector3 moveDir = transform.forward;
            //Not Moving
            if(direction.magnitude == 0)
            {
                if(tiltRotationSpeed.x > 0)
                {
                    tiltRotationSpeed.x -= .2f;
                }
                if(tiltRotationSpeed.y > 0)
                {
                    tiltRotationSpeed.y -= .2f;
                }

            }
            if(direction.magnitude >= .1f && moveCharacter)
            {

                //Atan2 returns angle between x axis and the angle between 0
                //Gives us an angle in radians
                //Add the rotation of the camera on the y axis on to the camera
                /*===== ThirdPersonCamera_GamePad Rotation*/
                
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                Vector3 moveDir;
                if(!_isWallRunning)
                {

                    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                 

                    //Move Forward as normal
                    moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                    //Tilt Player
                    //Tilting Right
                    //print("MovementInput.x: " + movementInput.x);
                    //print("MovementInput.y: " + movementInput.y);
                    if(movementInput.x > 0.4 && movementInput.y > 0)
                    {
                        float tiltAngleZ = -15f;
                        float tiltAngleX = 5;

                        //Tilt Angle X 
                        if(tiltRotationSpeed.x * Time.deltaTime < 5)
                        {
                            tiltRotationSpeed.x += .5f;
                            //print("tiltRotationSpeed.x: " + tiltRotationSpeed.x);
                        }

                        //Tilt Angle Z
                        if(tiltRotationSpeed.y * Time.deltaTime < 15)
                        {
                            tiltRotationSpeed.y += .05f;;
                            //print("tiltRotationSpeed.y: " + tiltRotationSpeed.y);
                        }

                        //axis = Vector3.Cross(moveDir.normalized, Vector3.up);

                        //Tilts character over time
                        /*transform.Rotate(new Vector3(0,0,1), tiltRotationSpeed.y * Time.deltaTime * -1);
                        transform.Rotate(new Vector3(1,0,0), tiltRotationSpeed.x * Time.deltaTime);*/
                        
                        //Tilts character instantly
                        transform.Rotate(new Vector3(0,0,1), tiltAngleZ);
                        transform.Rotate(new Vector3(1,0,0), tiltAngleX);
                        //_isTilting = true;    
                    }

                    //Moving forward no tilt
                    if(movementInput.x == 0 && movementInput.y > 0)
                    {
                        transform.Rotate(new Vector3(0,0,1), 0f);
                        transform.Rotate(new Vector3(1,0,0), 0f);
                    }
                    //Tilting Left
                    if(movementInput.x < -0.4 && movementInput.y > 0)
                    {
                        float tiltAngleZ = 15f;
                        float tiltAngleX = 5;
                        axis = Vector3.Cross(moveDir.normalized, Vector3.up);
                        transform.Rotate(new Vector3(0,0,1), tiltAngleZ);
                        transform.Rotate(new Vector3(1,0,0), tiltAngleX);
                        //_isTilting = true;    
                    }


                    RaycastHit slopeHit;
                    Vector3 slopeNormal;
                    if(Physics.Raycast(_groundChecker.position, Vector3.down, out slopeHit, GroundDistance, Ground))
                    {
                        Debug.Log("Player touched the ground");
                        AttachToMovingPlatform(slopeHit);
                        slopeNormal = slopeHit.normal;
                        Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.up, slopeNormal);
                        //Multiply slope offset by move Direction. You can multiply a quaternion x a vector. Not a vector x a quaternion
                        _controller.Move(slopeOffset * moveDir.normalized * speed * Time.deltaTime);
                        Debug.DrawRay(transform.position, Vector3.down, Color.red);
                    } 
                    
                    if(!glide && !_isGrounded)
                    {
                        _controller.Move(moveDir.normalized * speed * Time.deltaTime);
                    } 
                }
                else
                {
                    // The step size is equal to speed times frame time.
                    float singleStep = speed * Time.deltaTime;

                    // Rotate the forward vector towards the target direction by one step
                    Vector3 turnDirection = Vector3.RotateTowards(transform.forward, wallVector, singleStep, 0.0f);
                    // Draw a ray pointing at our target in
                    Debug.DrawRay(transform.position, turnDirection, Color.red);

                    // Calculate a rotation a step closer to the target and applies rotation to this object
                    transform.rotation = Quaternion.LookRotation(turnDirection);

                    
                    moveDir = wallVector;

                    //Look up parabolic motion. There seem to be animation cuves, bezier curves, and other lines to use.
                    //I'm curious what mathmatical functions simulate parbolas. How do you achieve x^2?

                    _velocity.y = 0;

                    _controller.Move(wallVector * speed * Time.deltaTime); 
                }          
            }

            /*if(_isTilting)
            {
                transform.Rotate(axis, -30);
                _isTilting = false;
            }*/

            if(dashInput || glide && !GetCombatState())
            {
                Dash();
            }

            if(dashInput || glide && GetCombatState())
            {
                DodgeBulletTime();
            }

            //Constant subtration of friction for glide/decceleration
            //https://www.gamasutra.com/blogs/MarkVenturelli/20140821/223866/Game_Feel_Tips_II_Speed_Gravity_Friction.php
            /*if(glide)
            {
                Vector3 moveDir = transform.forward;
                _controller.Move(moveDir.normalized * speed * Time.deltaTime); 
                if(speed > 0)
                {
                    speed = speed - (friction * Time.deltaTime);
                }
                if(speed < 0)
                {
                    //resetting speed to default. If you want a more natural acceleration, allow speed to = 0. 
                    this.glide = false;
                    speed = defaultSpeed;
                }   
            }*/
         

            

            //Controller Input
            /*
            if(Input.GetAxis("Aim") == 1 && !aimCamera.activeInHierarchy)
            {
                moveCharacter = false;
                print("AIM");
                mainCamera.SetActive(false);
                aimCamera.SetActive(true);
                this.GetComponent<ThirdPersonCamera>().enabled = true;
                StartCoroutine(ShowReticle());

            }
            else if(Input.GetAxis("Aim") != 1 && !mainCamera.activeInHierarchy)
            {
                moveCharacter = true;
                mainCamera.SetActive(true);
                aimCamera.SetActive(false);
                aimReticle.SetActive(false);
                this.GetComponent<ThirdPersonCamera>().enabled = false;
                SetSlowTime(false);
            }
            */

            //if (Input.GetButton("PlayerJump")){
            if (jumpInput && !displaySpecialAttackWindowInput){
                if(_isGrounded && !_isWallRunning){
                    print("JUMP");
                    //Deatach from platform
                    if(_attachedToPlatform)
                    {
                        DetachFromMovingPlatform();
                    }

                    //reset y_velocity to prevent super bouncing
                    _velocity.y = 0; 
                    _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
                    animator.SetBool("Running", false);
                    animator.SetBool("Jumping", true);

                    //play jump vfx
                    GameObject jumpVFXClone;
                    jumpVFXClone = Instantiate(_jumpVFX, transform.position, transform.rotation);
                    //jumpVFXClone.GetChild(0).GetComponent<ParticleSystem>().Play();
                    //float duration = jumpVFXClone.GetChild(0).GetComponent<ParticleSystem>().main.duration;
                    //ParticleSystem particleSystem = jumpVFXClone.GetChild(0).GetComponent<ParticleSystem>();
                    //var main = particleSystem.main;

                    Destroy(jumpVFXClone, 2);
                }
                if(_isWallRunning && !_isGrounded){
                    //DetachFromWall

                    //Deatach from platform
                    if(_attachedToPlatform)
                    {
                        DetachFromMovingPlatform();
                    }

                    print("WALLJUMP");
                    animator.SetBool("Running", false);
                    animator.SetBool("WallRunning", false);
                    animator.SetBool("Jumping", true);
                    isWallLeft = false;
                    isWallRight = false;
                    isWallInFront = false;
                    ExitWallRun();
                    //reset y_velocity on wall
                    _velocity.y = 0;
                    print("yVelocity before jump: " + _velocity.y);
                    _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
                    print("yVelocity after jump: " + _velocity.y);
                    //Jump off wall at 45 degree angle
                    Vector3 jumpDirection;
                    //jump direction uses transform.TransformDirection to move player in a vector 45degrees away from wall
                    jumpDirection = transform.TransformDirection(wallJumpDirection);
                    _controller.Move(jumpDirection * speed * Time.deltaTime); 
                }
                //TODO: Better Jumping Arc //Getting a better jumping arc will probably be factored here
                //_velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
            }

            //Display Special Attack Input Window
            if(displaySpecialAttackWindowInput)
            {
                ToggleSpecialAttackWindow(true);
            } 
            else
            {
                ToggleSpecialAttackWindow(false);
            }

            //Action Timer
            if(startTimer)
            {
                _actionTimer += Time.deltaTime;
            }

            //Special Attack
            if(specialAttackInput && !displaySpecialAttackWindowInput){
                //If 10 seconds have passed we can do this action
                print("Skill Duration: " + specialAttack.GetComponent<SpecialAttack>().GetSkillDuration());
                if(_actionTimer >= specialAttack.GetComponent<SpecialAttack>().GetSkillDuration())
                {
                    //10 is an arbitrary value for now. Replace with a quickfire animation time
                    //Debug.Log("Reset action timer after hold for 10 seconds");
                    animator.SetBool("CosmicPalmAttack", false);

                    startTimer = false;
                    specialAttack.GetComponent<SpecialAttack>().DisableSpecialAttack();
                    //The ability is finished, we should be able to move the character manually again
                    moveCharacter = true;
                    
                }

                if(_actionTimer == 0){
                    animator.SetBool("CosmicPalmAttack", true);
                    //Special Attack is started by an event set on CosmicPalmAttack  (Special Attack)
                    Debug.Log("Starting Cosmic Palm Attack");

                    //Stop moving the character, the ability has started
                    if(specialAttack.GetComponent<SpecialAttack>().GetMobilityType() == SpecialAttack.MobilityType.Immobile)
                    {
                        moveCharacter = false;
                    }
                    //startSpecialAttack();
                    startTimer = true;
                }
            }
            
            //Trigger released
            if(!specialAttackInput)
            {
                //Debug.Log("Trigger released");
                //This was initially created so the player would stay posed up while the cosmic beam was firing
                if(_actionTimer >= specialAttack.GetComponent<SpecialAttack>().GetSkillDuration())
                {
                    startTimer = false;
                    specialAttack.GetComponent<SpecialAttack>().DisableSpecialAttack();
                    _actionTimer = 0;
                    animator.SetBool("CosmicPalmAttack", false);
                    moveCharacter = true;
                }
            }

            //GroundMeleeAttack1
            if(groundMeleeAttack1Input && !displaySpecialAttackWindowInput)
            {
                
                if(combatStateIndex == 0)
                {
                    animator.SetTrigger("combo1");
                    groundMeleeAttack1Input = false;
                    MeleeCombo1();
                    combatStateIndex = 1;
                    return;
                }

                //TODO: Turn this into a method for combo input detection
                //poll for combo
                if(combatStateIndex == 1 && animationWindow != 0)
                {
                        print("Hook Combo Played");
                        AnimationClip animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                        //if(animatorClipInfo.length > .56 && animatorClipInfo.length <= 1)
                        //{
                        //animator.SetTrigger("combo1_link2");
                        //animator.SetBool("isComboing", true);
                        animator.Play("Grounded.combo1_1_leftHook");
                        groundMeleeAttack1Input = false;
                        MeleeCombo1();
                        //end of combo so let's set the combatStateIndex back to 0.
                        combatStateIndex = 2;
                        return;
                        //}   
                }

                if(combatStateIndex == 2 && animationWindow != 0)
                {
                    print("High Kick Combo played");
                    animator.Play("Grounded.combo1_2_meialouaKick");
                    groundMeleeAttack1Input = false;
                    MeleeCombo1();
                    combatStateIndex = 3;
                    return;
                }
            }





            //This could be a general melee skill that transforms!
            if(kickThrownSpecialAttackInput && !displaySpecialAttackWindowInput)
            {
                
                if(instancedSpecialAttack != null)
                {
                    print("KICK");
                    //Check if we can kick the special thrown object
                    SpecialAttack script = instancedSpecialAttack.GetComponent<SpecialAttack>();
                    if(script.GetKnockbackThrownObjectStatus())
                    {
                        animator.Play("Grounded.kickThrownSpecialAttack");
                        //Special Attack Throw is started from this animation event
                    }
                }
                kickThrownSpecialAttackInput = false;
            }

            if(movementInput.x != 0 || movementInput.y != 0)
            {
                if(!_isWallRunning && !animator.GetBool("Jumping"))
                {
                    animator.SetBool("Running", true);
                }
            } 
            else 
            {
                if(!_isWallRunning)
                {
                    animator.SetBool("Running", false);
                }
            }
            
            //Gravity
            /*DEBUG print(_velocity.y);*/
            //Falling Animation Control
            if(_velocity.y < 0)
            {
                animator.SetBool("Falling", true);
            }
            if(applyGravity)
            {
                _velocity.y += gravity * Time.deltaTime;
                //Getting a better jumping arc will probably be factored here
                _controller.Move(_velocity * Time.deltaTime);
            }

            //Special Attack Aiming Logic (Aims to Center of Screen)
            Ray ray = cam.gameObject.GetComponent<Camera>().ScreenPointToRay(new Vector3(cam.gameObject.GetComponent<Camera>().pixelWidth/2, cam.gameObject.GetComponent<Camera>().pixelHeight/2, 0));
            _specialAttackSpawnLocation.transform.LookAt(_specialAttackSpawnLocation.transform.position + ray.direction);

            //Turn player towards special attack as it is happening. 
            if(startTimer)
            {
                transform.rotation = Quaternion.Euler(_specialAttackSpawnLocation.transform.rotation.eulerAngles.x, _specialAttackSpawnLocation.transform.rotation.eulerAngles.y, 0);
            }
        }

        //Knockback logic (Impact)
        if(impact.magnitude > .2)
        {
            _controller.Move(impact * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 1f * Time.deltaTime);
            //canMove = false;
        }

        //If touching bounds, lower health and reset player
        RaycastHit touchingBoundsRaycast;
        _isTouchingBounds = Physics.Raycast(_groundChecker.position, Vector3.down, out touchingBoundsRaycast, .2f, Bounds);
        if(_isTouchingBounds)
        {
            Respawn();
        }
        /* LOCK ON LOGIC */
        //Turn LockOn Off
        if(lockOnInput && lockedOn)
        {
            //Experimental Add widescreen bars
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            if(gameManager.canDisplayWidescreenUI)
            {
                gameManager.disableWidescreenBars(this.GetPlayerID());
            }

            //Disable Target Arrow
            gameManager.disableTargetArrow(this.GetPlayerID());

            lockedOn = false;
            DisengageDynamicTargetLock();
            lockOnInput = false;

        }

        //LockOn to Target
        if(lockOnInput && !lockedOn)
        {
            //Look for surrounding enemies
            bool targetsFound = FindNearbyTargets();
            if(targetsFound)
            {
                //Experimental Add widescreen bars
                GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                /*if(gameManager.canDisplayWidescreenUI)
                {
                    gameManager.enableWidescreenBars(this.GetPlayerID());
                }*/

                print("found targets, locking on");
                

                //targetToLock = GameObject.FindGameObjectWithTag("TargetLock").transform;
                //reset currentTarget 
                currentTarget = 0;
                targetToLock = foes[currentTarget];

                //Need a better way to determine enemy
                Transform targetToLockHead = targetToLock.GetComponent<Bladeclubber>().Head;
                if(targetToLockHead == null)
                {
                    print("Reassinging bladeclubber head");
                    targetToLockHead = targetToLock.GetComponent<Bladeclubber>().Head;
                }

                //Third object should be the "Head Game Object"
                //lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = targetToLockHead;
                EngageDynamicTargetLock(targetToLockHead);

                //Adjust camera offset to look up at monster
                //lockOnCamera.GetComponent<CinemachineCameraOffset>().m_Offset.x

                //Activate & Adjust position of targeter
                gameManager.enableTargetArrow(this.GetPlayerID());
                gameManager.updateTargetPosition(this.GetPlayerID(), targetToLockHead.gameObject);

                lockedOn = true;
                lockOnInput = false;
            }
            

        }

        //Swap to Right/Left Target
        if(lockedOn)
        {
            if(targetSwitchRightInput && _swapTimer >= .2f)
            {
                if(currentTarget >= foes.Length - 1)
                {
                    currentTarget = 0;
                }
                else if(currentTarget < foes.Length - 1)
                {
                    currentTarget += 1;
                }
                //currentTarget += 1;
                print("locked on right");
                targetSwitchRightInput = false;
                targetSwitchLeftInput = false;
                _swapTimer = 0;
            }

            if(targetSwitchLeftInput && _swapTimer >= .2f)
            {
                //An else statement here might be more beneficial since placing the >= after the < would cause an error in which if currentTarget.length = 3 and currentTarget = 2, currentTarget++ would increment currentTarget to 3, then trigger setting currentTarget to 0, creating an off by 1 error.
                //currentTarget -=1;
                if(currentTarget > 0)
                {
                    currentTarget -= 1;
                }
                else if(currentTarget == 0)
                {
                    currentTarget = foes.Length - 1;
                }
                targetSwitchLeftInput = false;
                targetSwitchRightInput = false;
                print("locked on left");
                _swapTimer = 0;
            }

            _swapTimer += Time.deltaTime;

            //print(currentTarget);

            targetToLock = foes[currentTarget];

            Transform targetToLockHead;
            if(targetToLock.GetComponent<Foe>() == null)
            {
                //Anatomy of a Bladeclubber. parent game object contains script. Box collider contained on body of bladeclubber
                targetToLockHead = targetToLock.GetComponentInParent<Bladeclubber>().Head;
            }
            else
            {
                targetToLockHead = targetToLock.GetComponent<Foe>().Head;
            }

            //Update target to lock
            //lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = targetToLockHead;
            if(targetToLockHead != null)
            {
                UpdateDynamicTargetLock(targetToLockHead);
                GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                gameManager.updateTargetPosition(this.GetPlayerID(), targetToLockHead.gameObject);
            }
        }

        //Movement that should be applied due to other techniques.
        MeleeCombo1Movement();

        //Animation Window Timing
        UpdateAnimationWindow();

        
    }

    //Combat function 
    void startSpecialAttack()
    {
        //Set Special Attack Position & Rotation
        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("kickThrownSpecialAttack"))
        {
            specialAttack.GetComponent<SpecialAttack>().SetSpawnLocation(_shieldReturnRecoilKickSpawn);
            //freeLookCamera.fo
        }
        else{
            specialAttack.GetComponent<SpecialAttack>().SetSpawnLocation(_specialAttackSpawnLocation);
        }

        //Set Skill User
        specialAttack.GetComponent<SpecialAttack>().SetSkillUser(this.transform);

        //Set Skill User Animation
        specialAttack.GetComponent<SpecialAttack>().SetSkillUserAnimator(this.animator);

        //Set Special Attack Animation
        specialAttack.GetComponent<SpecialAttack>().SetLinkedAnimationClip(combatAnimationClip[0]);

        //Instantiate special Attack
        instancedSpecialAttack = Instantiate(specialAttack, _specialAttackSpawnLocation.position, _specialAttackSpawnLocation.rotation) as GameObject;

        //Look in direction of special attack
        transform.rotation = Quaternion.Euler(_specialAttackSpawnLocation.transform.rotation.eulerAngles.x, _specialAttackSpawnLocation.transform.rotation.eulerAngles.y, 0);


        specialAttack.GetComponent<SpecialAttack>().EnableSpecialAttack();
    }

    void AttachToMovingPlatform(RaycastHit platform)
    {
        //Asteroid Condition
        if(platform.transform.tag == "Asteroid")
        {
            //_controller.detectCollisions = false;
            Debug.Log("Attaching to Asteroid");
            //_isGrounded = true;
            //animator.SetBool("Jumping", false);
            //_velocity.y = 0;
            _attachedToPlatform = true;
            //platform.transform.GetComponent<MeshRenderer>().enabled = false;
            platform.rigidbody.isKinematic = true;
            //platform.transform.GetComponent<MeshRenderer>().enabled = true;
            //this._controller.enabled = false;
            //this.transform.position = platform.point - Vector3.up;
            //this._controller.enabled = true;

            //_controller.detectCollisions = true;
        }
    }

    void DetachFromMovingPlatform()
    {
        Debug.Log("Detaching to Asteroid");
        this.ParentObj.SetParent(null);
        _attachedToPlatform = false;
    }

    void SpawnCombatVFX(string vfxName)
    {
        switch (vfxName)
        {
            case "vfx_EarthernRise":
                GameObject combatVFXManagerInstance = Instantiate(combatVFXManager, this.transform.position, this.transform.rotation) as GameObject;
                combatVFXManagerInstance.GetComponent<CombatVFXManager>().SetCombatVFXSpawn(this._shieldReturnRecoilKickSpawn);
                combatVFXManagerInstance.GetComponent<CombatVFXManager>().triggerSpecialVFX("vfx_EarthernRise");
                return;
            case "vfx_estonCombo_hit_connected":
                GameObject combatVFXManagerInstance2 = Instantiate(combatVFXManager, this.transform.position, this.transform.rotation) as GameObject;
                combatVFXManagerInstance2.GetComponent<CombatVFXManager>().SetCombatVFXSpawn(this._vfxPositionCombo1);
                combatVFXManagerInstance2.GetComponent<CombatVFXManager>().triggerSpecialVFX("vfx_estonCombo_hit_connected");
                break;
            default:
                break;
        }
        
    }

    public void SetPlayerGravity(bool toggle)
    {
        this.applyGravity = toggle;
    }

    void Dash()
    {
        //We aren't engaged in combat
        if(!GetCombatState())
        {
            moveCharacter = false;
            if(lockedOn)
            {
                transform.LookAt(GetCurrentTarget().transform);
            }
            if(_dashTimer == 0)
            {
                print("_dashTimer is 0 teleport");

                //Game Thiccness - adding field of view adjustment to dash
                freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 80;

                //https://answers.unity.com/questions/1614287/teleporting-character-issue-with-transformposition.html
                _controller.enabled = false;
                //If > 0 we want to teleport forward
                //If < 0 we want to teleport backward
                print(movementInput.y);
                if(movementInput.y > 0 || movementInput.y == 0)
                {
                    transform.position = transform.position + transform.forward * 2;
                } else 
                {
                    transform.position = transform.position + transform.forward * -2;
                }
                _controller.enabled = true;

            }
            _controller.Move(transform.forward * dashSpeed * Time.deltaTime);

            //Increase dashTimer.
            _dashTimer += Time.deltaTime;

            if(dashSpeed > 0)
            {
                if(!_isGrounded)
                {
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Falling", true);
                }
                animator.Play("Grounded.freeRun", 0, 0.5f);
                animator.speed = 0;
                dashInput = false;
                dashSpeed = dashSpeed - (dashFriction * Time.deltaTime);
                this.glide = true;
            }

            if(dashSpeed < 0) 
            {
                //resetting speed to default. If you want a more natural acceleration, allow speed to = 0. 
                this.glide = false;
                dashSpeed = defaultDashSpeed;
                animator.speed = 1;
                _dashTimer = 0;
                moveCharacter = true;
            }

            if(_dashTimer >= .5)
            {
                this.glide = false;
                dashSpeed = defaultDashSpeed;
                animator.speed = 1;
                _dashTimer = 0;
                moveCharacter = true;
                freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 40;
            }

            if(_afterImageTimer == 0)
            {
                Instantiate(glideAfterImage, this.transform.position, this.transform.rotation);
            }

            if(_afterImageTimer >= .2)
            {
                _afterImageTimer = 0;
            } 
            else 
            {
                _afterImageTimer += Time.deltaTime;
            }
        }

        /*
         //Increase speed when we want to accelerate or glide. For now we call this gliding due to the flight like effect. Think Dissidia.
                if(glide)
                {
                    if(speed < maxSpeed){
                        speed = speed + acceleration * Time.deltaTime;
                    }
                }
        */

        //Constant subtration of friction for glide/decceleration
        //https://www.gamasutra.com/blogs/MarkVenturelli/20140821/223866/Game_Feel_Tips_II_Speed_Gravity_Friction.php
        //Update dash here like glide
        /*if(glide)
        {
            Vector3 moveDir = transform.forward;
            _controller.Move(moveDir.normalized * speed * Time.deltaTime); 
            if(speed > 0)
            {
                speed = speed - (friction * Time.deltaTime);
            }
            if(speed < 0)
            {
                //resetting speed to default. If you want a more natural acceleration, allow speed to = 0. 
                this.glide = false;
                speed = defaultSpeed;
            }   
        }*/
    }

    void DodgeBulletTime()
    {
        //Player is engaged in combat, dash can optionally move character behind opponent.
        if(GetCombatState())
        {
            moveCharacter = false;
            if(lockedOn)
            {
                transform.LookAt(GetCurrentTarget().transform);
            }
            
            if(_dashTimer == 0)
            {
                print("_dashTimer is 0 teleport");

                //Game Thiccness - adding field of view adjustment to dash
                //freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 25;

                //https://answers.unity.com/questions/1614287/teleporting-character-issue-with-transformposition.html
            }
            //_controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            //Rotate player around combatant
            if(combatant != null)
            {
                _controller.enabled = false;
                transform.RotateAround(combatant.transform.position, Vector3.up, 180 * Time.deltaTime);
            }

            //Increase dashTimer.
            _dashTimer += Time.deltaTime;

            if(dashSpeed > 0)
            {
                if(!_isGrounded)
                {
                    animator.SetBool("Jumping", false);
                    animator.SetBool("Falling", true);
                }
                animator.Play("Grounded.sideDodgeLeft", 0, 0.5f);
                animator.speed = 0;
                dashInput = false;
                dashSpeed = dashSpeed - (dashFriction * Time.deltaTime);
                this.glide = true;

                
            }

            if(dashSpeed < 0) 
            {
                //resetting speed to default. If you want a more natural acceleration, allow speed to = 0. 
                this.glide = false;
                dashSpeed = defaultDashSpeed;
                animator.speed = 1;
                _dashTimer = 0;
                moveCharacter = true;
            }

            //Combatant current animation frame
            AnimationClip combatAnimationClip = combatant.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip;
            float combatantAnimationTime = combatAnimationClip.length;
            Debug.Log("Combatant Animation Length: " + combatantAnimationTime);
            Debug.Log("Player is dodging " + combatant.name + "'s " + combatAnimationClip.name);

            if(_dashTimer < combatantAnimationTime)
            {
                //Player is dodging so let's zoom the camera out.
                //freeLookCamera.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius = 40;
                freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 50;

                SetIframe(true);
                
                //Slow down time
                //SetSlowTime(true);

                //Slow down animation time for the combatant
                combatant.GetComponent<Animator>().speed = .3f;
                
            }
            if(_dashTimer >= combatantAnimationTime)
            {
                this.glide = false;
                dashSpeed = defaultDashSpeed;
                animator.speed = 1;
                _dashTimer = 0;
                _controller.enabled = true;
                moveCharacter = true;

                //SetSlowTime(false);
                //Slow down animation time for the combatant
                combatant.GetComponent<Animator>().speed = 1f;

                SetIframe(false);
                freeLookCamera.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView = 40;
                freeLookCamera.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius = 9;
            }

            //TODO turn into a function
            if(_afterImageTimer == 0)
            {
                Instantiate(glideAfterImage, this.transform.position, this.transform.rotation);
            }

            if(_afterImageTimer >= .2)
            {
                _afterImageTimer = 0;
            } 
            else 
            {
                _afterImageTimer += Time.deltaTime;
            }
        }
    }
    /*Inspired by the algorithm provided here http://www.footnotesforthefuture.com/words/wall-running-1/*/
    void CheckForWall()
    {
        RaycastHit hit;
        isWallRight = Physics.Raycast(_wallRunChecker.transform.position, _wallRunChecker.right, out hit, 1.0f, Wall | Ground);
        if(isWallRight){
            Debug.DrawRay(_wallRunChecker.transform.position, _wallRunChecker.right.normalized * hit.distance, Color.magenta );
            wallVector = -Vector3.Cross(hit.normal, Vector3.up).normalized;
            wallJumpDirection = new Vector3(-1,0,1).normalized;
            //wallJumpDirection = -wallJumpDirection;
            //print("Wall is to the Right");
        }

       
        isWallLeft = Physics.Raycast(_wallRunChecker.transform.position, -_wallRunChecker.right, out hit, 1.0f, Wall | Ground);
        if(isWallLeft){
            Debug.DrawRay(_wallRunChecker.transform.position, -_wallRunChecker.right.normalized * hit.distance, Color.green );
            wallVector = Vector3.Cross(hit.normal, Vector3.up).normalized;
            wallJumpDirection = new Vector3(1,0,1).normalized;
        }

        /*isWallInFront = Physics.Raycast(_wallRunChecker.transform.position, _wallRunChecker.forward, out hit, wallDistance, Wall);
        if(isWallInFront){
            Debug.DrawRay(_wallRunChecker.transform.position, _wallRunChecker.forward.normalized * hit.distance, Color.red);
            wallVector = Vector3.up;    
            //we are facing forward so let's jump backwards, the opposite of forward
            wallJumpDirection = -Vector3.forward.normalized;
            //transform.rotation = Quaternion.Euler(-90,0,0);
        }*/
       
        Debug.DrawRay(_wallRunChecker.transform.position, wallVector, Color.yellow);

        //Debug ray describing Vector at 45 degree from wall
        Debug.DrawRay(_wallRunChecker.transform.position, transform.TransformDirection(wallJumpDirection), Color.cyan);
    }

    public float GetActionTimer()
    {
        return _actionTimer;
    }

    void StartWallRun(string direction, Vector3 directionVector)
    {
        animator.SetBool("WallRunning", true);
        animator.SetBool("Jumping", false);

        TriggerDustTrails(directionVector);

        if(direction == "right")
        {
            //TODO: Implement 
            Debug.Log("StartWallRun: " + "Wall is to the Right.");
            animatorOverrideController["eston_rig|WallRunLeft"] = wallRunningAnimationClip[0];
        }

        if(direction == "left")
        {
            Debug.Log("StartWallRun: " + "Wall is to the Left.");
            animatorOverrideController["eston_rig|WallRunLeft"] = wallRunningAnimationClip[1];
        }

        if(direction == "front")
        {
            Debug.Log("StartWallRun: " + "Wall is in Front.");
            animatorOverrideController["eston_rig|WallRunLeft"] = wallRunningAnimationClip[2];
        }

        print("wallrunning");
        _isWallRunning = true;
    }

    void TriggerDustTrails(Vector3 directionVector)
    {
        if(dustTrails != null && directionVector.magnitude != 0) 
        {
            dustTrails.SetActive(true);
        } else if(directionVector.magnitude == 0)
        {
             dustTrails.SetActive(false);
        }
    }

    void ExitWallRun()
    {
         animator.SetBool("WallRunning", false);
        _isWallRunning = false;
    }
    void togglePlayerMovementControl(bool toggle)
    {
        canPlayerInputMove = toggle;
    }

    //New Unity Input Management.
    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext ctx) => jumpInput = ctx.ReadValueAsButton();
    public void OnSpecialAttack(InputAction.CallbackContext ctx) => specialAttackInput = ctx.ReadValueAsButton();
    public void OnDash(InputAction.CallbackContext ctx) => dashInput = ctx.ReadValueAsButton();
    public void OnLockOn(InputAction.CallbackContext ctx) => lockOnInput = ctx.ReadValueAsButton();
    public void OnLockOnSwitchTargetRight(InputAction.CallbackContext ctx) => targetSwitchRightInput = ctx.ReadValueAsButton();
    public void OnLockOnSwitchTargetLeft(InputAction.CallbackContext ctx) => targetSwitchLeftInput = ctx.ReadValueAsButton();
    public void OnEnviromentInteraction(InputAction.CallbackContext ctx) => enviromentActionInput = ctx.ReadValueAsButton();
    public void OnKickThrownSpecialAttack(InputAction.CallbackContext ctx) => kickThrownSpecialAttackInput = ctx.ReadValueAsButton();
    public void OnGroundMeleeAttack1(InputAction.CallbackContext ctx) => groundMeleeAttack1Input = ctx.ReadValueAsButton();
    public void OnDisplaySpecialAttackInputWindow(InputAction.CallbackContext ctx) => displaySpecialAttackWindowInput = ctx.ReadValueAsButton();
    
    /*public void OnSouthButtonPressed(InputAction.CallbackContext ctx) => southButtonInput = ctx.ReadValueAsButton();
    public void OnNorthButtonPressed(InputAction.CallbackContext ctx) => northButtonInput = ctx.ReadValueAsButton();
    public void OnWestButtonPressed(InputAction.CallbackContext ctx) => westButtonInput = ctx.ReadValueAsButton();
    public void OnEastButtonPressed(InputAction.CallbackContext ctx) => eastButtonInput = ctx.ReadValueAsButton();*/

    //https://answers.unity.com/questions/242648/force-on-character-controller-knockback.html?_ga=2.213933971.521934289.1611610771-608714207.1587856867
    public void AddImpact(Vector3 direction, float force)
    {
       direction.Normalize();
       //direction = direction.normalized;

       if(direction.y < 0)
       {
           //reflects downard force
           direction.y = -direction.y;
       }
       impact += direction.normalized * force / mass;
       //impact += direction * force / mass;
       print("Adding impact: " + impact);

       //Apply screenshake TODO: Supply a variable to this function that holds the intensity and time values, that way each effect that defines an impact can define a screenshake!
       CinemachineScreenShake.Instance.screenShake(10.0f, .5f);
    }


    public bool FindNearbyTargets()
    {
        
        Vector3 position = transform.position + _controller.center;
        RaycastHit[] foeHit = Physics.SphereCastAll(position, 30f, transform.forward, 30f, Foe);

        //This sort method compares the magnitude (distance) of target to the player. The closer the enemy, the sooner we want to target
        System.Array.Sort(foeHit, (x,y) => ((int)(x.transform.position - transform.position).magnitude).CompareTo((int)(y.transform.position - transform.position).magnitude));

        bool foundTargets = false;

        //Create an array based on the number of targets around us;
        foes = new Transform[foeHit.Length];
        if(foeHit.Length > 0)
        {
            int iter = 0;
            foundTargets = true;

            foreach(RaycastHit foe in foeHit)
            {
                print(foe.transform.name + " : " + foe.distance);
                //Problem with rigidbodies read here https://forum.unity.com/threads/raycast-hit-rigidbody-object-instead-of-collider.544297/
                /*if(foe.rigidbody != null)
                {
                    print(foe.rigidbody.transform);
                    foes[iter] = foe.rigidbody.transform;
                } 
                else
                {
                    foes[iter] = foe.transform;
                }*/
                //print(foe.GetType().ToString());
                foes[iter] = foe.transform;
                iter++;
            }
        } 

        return foundTargets;
    }

    public GameObject GetCurrentTarget()
    {
        print("Current Target" + foes[currentTarget].name);
        //return targetToLock.GetComponent<Foe>().Head.gameObject;
        return targetToLock.GetComponent<Bladeclubber>().Head.gameObject;
    }

    public int GetPlayerID()
    {
        if(gameObject.tag == "P1")
        {
            return 1;
        }
        if(gameObject.tag == "P2")
        {
            return 2;
        }
        return -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Enviromental EFX Cosmic Wind Riding/Rising
        if(other.tag == "CosmicWind Enter Function")
        { 
            print("CosmicWind Rise");
            if(_velocity.y < windResetVelocityY)
            {
                _velocity.y = windResetVelocityY;
            }
            _velocity.y += windRiseSpeed * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        /*if(other.tag == "Mantle")
        {
            if(!_isGrounded)
            {
                print("Mantling");
                _isMantling = true;
            }
        }*/
    }

    private void OnTriggerStay(Collider other)
    {
        //Enviromental EFX Cosmic Wind Riding/Rising
        if(other.tag == "CosmicWind")
        { 
            if(_velocity.y < windResetVelocityY)
            {
                _velocity.y = windResetVelocityY;
            }
            animator.SetBool("Running", false);
            animator.SetBool("Jumping", true);
            print("CosmicWind Rise Stay function");
            _velocity.y += windRiseSpeed * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + _controller.center;
        Gizmos.DrawWireSphere(position, 30f);

    }


    void Mantle(RaycastHit ledgeReference)
    {
        //Adjust y velocity so player does not fall
        _velocity.y = 0;

        animator.SetBool("Falling", false);
        if(mantleType == MantleType.High)
        {
            //Conditional that prevents animation from overwriting another animation
            if(!this.animator.GetCurrentAnimatorStateInfo(0).IsName("mantle") && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("ledgeHop"))
            {
                animator.Play("Grounded.mantle");
            }    
        }

        if(mantleType == MantleType.Mid)
        {
            if(!this.animator.GetCurrentAnimatorStateInfo(0).IsName("ledgeHop") && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("mantle"))
            {
                animator.Play("Grounded.ledgeHop");
            }    
        }
    
        canMove = false;
        if(_mantleTimer <= .5f)
        {
            // mantleUpVector = new Vector3(0,1,0);
            _controller.Move(Vector3.up * 7.5f * Time.deltaTime);
        } else
        {
            //Vector3 mantleDiagnolVector = new Vector3(0,0,1);
            _controller.Move(transform.forward * 5.5f * Time.deltaTime);  
        }
        _mantleTimer += Time.deltaTime;

        //animator.SetBool("Landing", true);
        //animator.SetBool("Falling", false);
        print("Mantling");
    }

    void MeleeCombo1()
    {
        //Melee in direction player is aiming.
        //transform.LookAt(transform.position + cam.transform.forward);

        //Melee in direction player is facing.
        

        EngageCombatSlide();
        InitiateForwardMomentum();
        currentCombatSlideSpeed = maxCombatSlideSpeed;

        //Reset animation window
        startAnimationWindow = false;
        animationWindow = 0;
    }

    void UpdateAnimationWindow()
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //TODO for readability, we could change combatStateIndex to rather correspond with an arrayList of "Strings" with the animation name
        if(this.combatStateIndex == 1)
        {
            if(animatorStateInfo.normalizedTime >= .56 && animationWindow == 0)
            {
                //the player's movement is locked for the duration of this animation window set
                animationWindow = 1 - animatorStateInfo.normalizedTime;
                //animationWindow = 1f;
                startAnimationWindow = true;
            }
            
            if(animationWindow > 0 && startAnimationWindow)
            {
                animationWindow -= Time.deltaTime;
            }

            if(animationWindow <= 0 && startAnimationWindow)
            {
                animationWindow = 0;

                //reset comboState to 0
                combatStateIndex = 0;
                startAnimationWindow = false;
                DisengageCombatSlide();
            }
        }

        if(this.combatStateIndex == 2)
        {
            if(animatorStateInfo.normalizedTime >= .85 && animationWindow == 0)
            {
                //the player's movement is locked for the duration of this animation window set
                animationWindow = 1 - animatorStateInfo.normalizedTime;
                startAnimationWindow = true;
            }
            
            if(animationWindow > 0 && startAnimationWindow)
            {
                animationWindow -= Time.deltaTime;
            }

            if(animationWindow <= 0 && startAnimationWindow)
            {
                animationWindow = 0;

                //reset comboState to 0
                combatStateIndex = 0;
                startAnimationWindow = false;
                DisengageCombatSlide();
            }
        }

        if(this.combatStateIndex == 3)
        {
            if(animatorStateInfo.normalizedTime >= .56 && animationWindow == 0)
            {
                //the player's movement is locked for the duration of this animation window set
                animationWindow = 1 - animatorStateInfo.normalizedTime;
                startAnimationWindow = true;
            }
            
            if(animationWindow > 0 && startAnimationWindow)
            {
                animationWindow -= Time.deltaTime;
            }

            if(animationWindow <= 0 && startAnimationWindow)
            {
                animationWindow = 0;

                //reset comboState to 0
                combatStateIndex = 0;
                startAnimationWindow = false;
                DisengageCombatSlide();
            }
        }

        //print("Current animation window time: " + animationWindow);
    }

    //TODO: Refactor this name. This function is quickly becoming an initializer for combat
    void EngageCombatSlide()
    {
        combatSlide = true;

        //TODO: Experiment with toggling player control of movement off. 
        togglePlayerMovementControl(false);

        //turn hit boxes on.
        foreach (var hitBox in hitBoxes)
        {
            hitBox.ActivateHitBox();
        }

        
    }

    //TODO: Refactor this name. This function is quickly becoming a terminator for combat
    void DisengageCombatSlide()
    {
        print("Combat Slide Disengaged");
        combatSlide = false;

        //TODO: Experiment with toggling player control of movement off. 
        togglePlayerMovementControl(true);

        //turn hit boxes off.
        foreach (var hitBox in hitBoxes)
        {
            hitBox.DeactivateHitBox();
        }
        
    }

    void InitiateForwardMomentum()
    {
        currentCombatSlideSpeed = maxCombatSlideSpeed;
        this.combatRecoilMomentum = false;
        this.combatForwardMomentum = true;
    }

    void InitiateRecoilMomentum()
    {
        currentCombatSlideSpeed = maxCombatSlideSpeed;
        this.combatForwardMomentum = false;
        this.combatRecoilMomentum = true;
    }

    void MeleeCombo1Movement()
    {
        if(combatSlide)
        {
            Vector3 moveDir = transform.forward;
            if(this.combatForwardMomentum)
            {
                moveDir = transform.forward;
                _controller.Move(moveDir.normalized * currentCombatSlideSpeed * Time.deltaTime); 
            }
            if(this.combatRecoilMomentum)
            {
                moveDir = -transform.forward;
                _controller.Move(moveDir.normalized * currentCombatSlideSpeed * Time.deltaTime); 
                print("Moving backwards");
                print("Current currentCombatSlideSpeed: " + currentCombatSlideSpeed);
            }
            
            //Animation specific logic to control when combatRecoilForwardMomentum and backwardMomentum end
            
            //Controls the glide acceleration/decelleration
            if(currentCombatSlideSpeed > 0)
            {
                currentCombatSlideSpeed = currentCombatSlideSpeed - (combatSlideFriction * Time.deltaTime);
            }
            /*if(currentCombatSlideSpeed < 0)
            {
                //resetting speed to default. If you want a more natural acceleration, allow speed to = 0.
                if(combatForwardMomentum)
                {
                    this.currentCombatSlideSpeed = maxCombatSlideSpeed;
                    this.combatForwardMomentum = false;
                }

                if(combatRecoilMomentum)
                {
                    print("combat recoil activated");
                    this.combatSlide = false;
                    this.combatRecoilMomentum = false;
                    this.currentCombatSlideSpeed = maxCombatSlideSpeed;
                }
                

            }   */
        }
    }


    void Respawn()
    {
        print("Resetting player position");
        transform.position = respawnPosition;
        TakeDamage(100);
    }

    IEnumerator ShowReticle()
    {
        yield return new WaitForSeconds(0.25f);
        aimReticle.SetActive(enabled);
        SetSlowTime(true);
    }

    public void TakeDamage(float damage)
    {
        healthBar.SetHealth(healthBar.GetHealth() - damage);
    }

    public void SetSlowTime(bool on)
    {
        float time = on ? .1f : 1;
        Time.timeScale = time;
        Time.fixedDeltaTime = time * .02f;
    }

    //TODO: Consider a 1 v 1 or 1 vs many scenario. Does it make sense to set just one combatant, multiple combatants, or only focus the target?
    public void SetCombatState(bool combatState, Transform combatant)
    {
        animator.SetBool("EngagedInCombat", combatState);
        this.combatant = combatant;
    }

    //public void SetCombatTimer

    public void UpdateCombatState(float index)
    {
        //How should we link a dodge?

        //Handling Combat 
        switch (index)
        {
            case 0: 
                MeleeCombo1Movement();
                break;
            default:
                break;
        }

    }

    public bool GetCombatState()
    {
        if(animator == null)
        {
            return false;
        }
        else
        {
            return animator.GetBool("EngagedInCombat");
        }
    }

    public void SetIframe(bool iframeValue)
    {
        iframe = iframeValue;
    }

    public bool GetIframe()
    {
        return iframe;
    }

    public void EngageDynamicCameraTargetFloating(GameObject target)
    {
        freeLookCamera.SetActive(false);
        lockOnCamera.SetActive(false);
        //DisengageDynamicTargetLock();

        CinemachineTargetGroup targetGroup = dynamicCameraFloatingTarget.GetComponentInChildren<CinemachineTargetGroup>();
        targetGroup.AddMember(this.transform, 1, 2);
        targetGroup.AddMember(target.transform, 1, 6);

        dynamicCameraFloatingTarget.SetActive(true);

    }

    public void DisengageDynamicCameraTargetFloating()
    {
        dynamicCameraFloatingTarget.SetActive(false);
        freeLookCamera.SetActive(true);
        lockOnCamera.SetActive(false);
    }

    public void EngageDynamicTargetLock(Transform target)
    {
        freeLookCamera.SetActive(false);
        dynamicCameraFloatingTarget.SetActive(false);

        CinemachineTargetGroup targetGroup = lockOnCamera.GetComponentInChildren<CinemachineTargetGroup>();
        targetGroup.AddMember(this.transform, 1, 2);
        targetGroup.AddMember(target.transform, 1.5f, 4);

        lockOnCamera.SetActive(true);
    }

    public void DisengageDynamicTargetLock()
    {
        dynamicCameraFloatingTarget.SetActive(false);
        lockOnCamera.SetActive(false);
        freeLookCamera.SetActive(true);
    }

    public void UpdateDynamicTargetLock(Transform target)
    {
        CinemachineTargetGroup targetGroup = lockOnCamera.GetComponentInChildren<CinemachineTargetGroup>();
        
        //Update target at index 1 to the new target
        targetGroup.m_Targets[1].target = target.transform;

    }

    public void ToggleSpecialAttackWindow(bool value)
    {
        this.specialAttackWindow.SetActive(value);
        this.specialAttackWindow.GetComponent<SpecialAttackInputWindowController>().SetAgentProperties(this.transform);
    }
    
}
