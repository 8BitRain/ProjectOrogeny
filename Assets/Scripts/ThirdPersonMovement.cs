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
    private bool jumpPressed = false;

    public GameObject _jumpVFX;

    //Mantling 
    [Header("Ledge Grab (Mantling) Settings")]
    public LayerMask Ledge;
    public float ledgeDistance = 2f;
    private bool _isMantling = false;
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
    private bool enviromentActionInput = false;

    [Header("VFX")]
    public GameObject speedLines;

    //Respawn
    [Header("Respawn")]
    public LayerMask Bounds;
    public Vector3 respawnPosition;
    private bool _isTouchingBounds = false;



    //Combat
    [Header("Combat Ability Management")]
    public GameObject cosmicPalmBeam;
    public Transform _cosmicPalmBeamSpawnLocation;
    private GameObject spawnedCosmicPalmBeam;
    private Transform _laser;
    public float cosmicPalmBeamDuration = 5;
    public float cosmicPalmBeamSpeed = 5;
    public float cosmicPalmBeamThrust = 10000;
    
    //Camera Management
    [Header("Camera Management")]
    public GameObject mainCamera;
    public GameObject aimCamera;
    public GameObject aimReticle;
    public GameObject freeLookCamera;
    public GameObject lockOnCamera;
    private bool lockOnInput = false;
    private bool targetSwitchRightInput = false;
    private bool targetSwitchLeftInput = false;
    private bool lockedOn = false;
    private int currentTarget = 0;
    private float _swapTimer = 0;
    public Transform targetToLock;
    public LayerMask Foe; 

    public Transform[] foes;


    //Time Management
    //TODO: Create a time manager
    [Header("Time Settings")]
     private bool slowTime;

    //Combat Management
    [Header("Combat Timer Management")]
    private float cosmicPalmTimer = 0;
    private float _actionTimer = 0;
    private bool startTimer = false;
    private bool firePalmIntoDistance = false;
    private bool cosmicPalmInput = false;

    [Header("Character Settings")]
    public CharacterController _controller;
    public Transform cam;
    //This is the reference to the player so we can pull components that are children
    public Transform Orogene;

    private bool canMove = true;
    
    /* MESSAGE: CanRotate: This variable is to help define when the player should rotate. Use this tor situations where you want the player
    *  to continue moving, but lock this scripts control of the rotation. I'm doing this so I can let the ThirdPersonCamera Script Drive Camera interaction
    *  There's probably a more effecient way to do this, but this is an experiment. 
    *
    */
    private bool canRotate = true;

    private bool playingAnim;
    private bool running;
    private float animCounter;

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
            //print("grounded");
            //canMove = true;
        }

        //We are in the air!
        if(!_isGrounded)
        {
            //Raycast and see if we are facing a ledge
            //print("Airborne");
            RaycastHit ledgeSeekerHit;
            Vector3 _ledgeSeekerPosition = new Vector3(_groundChecker.position.x, _groundChecker.position.y + this._controller.height, _groundChecker.position.z);
            if(Physics.Raycast(_ledgeSeekerPosition, _groundChecker.forward, out ledgeSeekerHit, 2f, Ledge))
            {
                print("Mantling");
                _isMantling = true;
            }
            Debug.DrawRay(_ledgeSeekerPosition, _groundChecker.forward, Color.green);
            
        }

        if(_isMantling)
        {
            if(_mantleTimer < .75f)
            {
                Mantle();
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
            Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;

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
                StartWallRun("right");
            }
            if(isWallLeft && enviromentActionInput)
            {
                StartWallRun("left");
            }
            if(isWallInFront && enviromentActionInput)
            {
                StartWallRun("front");
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
                        slopeNormal = slopeHit.normal;
                        Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.up, slopeNormal);
                        //Multiply slope offset by move Direction. You can multiply a quaternion x a vector. Not a vector x a quaternion
                        _controller.Move(slopeOffset * moveDir.normalized * speed * Time.deltaTime);
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

            if(dashInput || glide)
            {
                Dash();
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
            if (jumpPressed){
                if(_isGrounded && !_isWallRunning){
                    print("JUMP");
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
            

            //Combat Player Input
            /*2 considerations. 1 QuickFire 2. Holding Palm Attack*/
            /*if(cosmicPalmTimer <= 0 && startTimer)
            {
                animator.SetBool("CosmicPalmAttack", false);
                startTimer = false;
                cosmicPalmTimer = 0;

                //Send out cosmicPalm, animation is finished
                startCosmicPalmBeam();
            }*/

            //Action Timer
            if(startTimer)
            {
                _actionTimer += Time.deltaTime;
            }

            //Trigger pressed or held
            //Old Trigger
            //if(Input.GetAxis("Right Trigger") == 1.0f){
            if(cosmicPalmInput){
                //Gravity,Sensitivty, and deadzone controller values were pulled from https://wiki.unity3d.com/index.php/Xbox360Controller
                //print("COSMIC PAWLLLMMM action timer started");

                //If 10 seconds have passed we can do this action
                if(_actionTimer >= 10)
                {
                    //10 is an arbitrary value for now. Replace with a quickfire animation time
                    //Debug.Log("Reset action timer after hold for 10 seconds");
                    animator.SetBool("CosmicPalmAttack", false);
                    startTimer = false;

                    //The ability is finished, we should be able to move the character manually again
                    moveCharacter = true;
                    
                }

                if(_actionTimer == 0){
                    if(spawnedCosmicPalmBeam == null)
                    {
                        startCosmicPalmBeam();
                    }
                    animator.SetBool("CosmicPalmAttack", true);
                    Debug.Log("Starting Cosmic Palm Attack");
                    //Stop moving the character, the ability has started
                    moveCharacter = false;
                    startTimer = true;
                }

                if(_actionTimer > .5){
                    //Debug.Log("Holding Trigger for longer than .5 seconds");
                    //print("CosmicPalm");
                }

                cosmicPalmTimer = combatAnimationClip[0].length;
            }
            
            //Trigger released
            //OLD TRIGGER
            //if(Input.GetAxis("Right Trigger") <= 0)
            if(!cosmicPalmInput)
            {
                //Debug.Log("Trigger released");
                if(_actionTimer >= 4)
                {
                    //Debug.Log("action time reset after trigger released");
                    startTimer = false;
                    _actionTimer = 0;
                    animator.SetBool("CosmicPalmAttack", false);
                    moveCharacter = true;
                }
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
            _velocity.y += gravity * Time.deltaTime;
            //Getting a better jumping arc will probably be factored here
            _controller.Move(_velocity * Time.deltaTime);

            //Attack Timers
            if(cosmicPalmTimer > 0 && startTimer)
            {
                cosmicPalmTimer -= Time.deltaTime;
            }

            //Update cosmic palm Spawn to look at center of screen
            //_cosmicPalmBeamSpawnLocation.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(.5f,.5f,0)));
            //Debug.DrawRay(_cosmicPalmBeamSpawnLocation.transform.position, Camera.main.ViewportToWorldPoint(new Vector3(.5f,.5f,0)), Color.red);

            Ray ray = cam.gameObject.GetComponent<Camera>().ScreenPointToRay(new Vector3(cam.gameObject.GetComponent<Camera>().pixelWidth/2, cam.gameObject.GetComponent<Camera>().pixelHeight/2, 0));
            //information for drawing gizmo ray that indicates the direction of the cosmicPalmBeam 
            //Debug.DrawRay(_cosmicPalmBeamSpawnLocation.transform.position, ray.direction * 10, Color.yellow);
            _cosmicPalmBeamSpawnLocation.transform.LookAt(_cosmicPalmBeamSpawnLocation.transform.position + ray.direction);
            //Update combat abilities
            updateCosmicPalmBeam();

            //print("Action Timer Value: " + _actionTimer);
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
            lockOnCamera.SetActive(false);
            freeLookCamera.SetActive(true);
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
                if(gameManager.canDisplayWidescreenUI)
                {
                    gameManager.enableWidescreenBars(this.GetPlayerID());
                }                
                print("found targets, locking on");
                freeLookCamera.SetActive(false);
                lockOnCamera.SetActive(true);
                //targetToLock = GameObject.FindGameObjectWithTag("TargetLock").transform;
                //reset currentTarget 
                currentTarget = 0;
                targetToLock = foes[currentTarget];
                Transform targetToLockHead = targetToLock.GetComponent<Foe>().Head;

                //Third object should be the "Head Game Object"
                lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = targetToLockHead;

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
            Transform targetToLockHead = targetToLock.GetComponent<Foe>().Head;
            lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = targetToLockHead;
            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.updateTargetPosition(this.GetPlayerID(), targetToLockHead.gameObject);
        }

        
    }

    void FixedUpdate()
    {
        if(spawnedCosmicPalmBeam != null)
        {
            if(firePalmIntoDistance)
            {
                Debug.Log("Fire Palm blast into distance");
                spawnedCosmicPalmBeam.GetComponent<Rigidbody>().AddForce(spawnedCosmicPalmBeam.transform.forward * cosmicPalmBeamThrust);
                firePalmIntoDistance = false;
                spawnedCosmicPalmBeam = null;
            }
        }
    }

    //Combat function 
    void startCosmicPalmBeam()
    {
        //Spawn Cosmic Palm Beam
        spawnedCosmicPalmBeam = Instantiate(cosmicPalmBeam, _cosmicPalmBeamSpawnLocation.position, _cosmicPalmBeamSpawnLocation.rotation) as GameObject;

        //The laser is the first object
        //TODO: Add error handling here. _laser has the possibility of not being the first child if the game object is not setup that way. 
        _laser = spawnedCosmicPalmBeam.transform.GetChild(0);
        if(_laser.gameObject.activeSelf)
        {
            //Set the laser beam to inactive. It should only form once the palm animation has finished. That way we get the anticipation effect of ki building in the palm.
            _laser.gameObject.SetActive(false);
        }

        //let's have the character look at where the beam is firing
        //transform.rotation = Quaternion.Euler(0, _cosmicPalmBeamSpawnLocation.transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Euler(_cosmicPalmBeamSpawnLocation.transform.rotation.eulerAngles.x, _cosmicPalmBeamSpawnLocation.transform.rotation.eulerAngles.y, 0);
    }

    void updateCosmicPalmBeam()
    {
        if(spawnedCosmicPalmBeam != null)
        {
            //The beam updating depends upon the animation state CosmicPalmAttack. For now this move is animation locked.
            if(animator.GetBool("CosmicPalmAttack"))
            {
                //Check timer to make sure we can summon the beam. We want to summon the beam once the cosmic palm strike animation completes
                if(_actionTimer >= combatAnimationClip[0].length)
                {
                    if(!_laser.gameObject.activeSelf && spawnedCosmicPalmBeam != null)
                    {
                        _laser.gameObject.SetActive(true);
                    }
                }
                spawnedCosmicPalmBeam.transform.position = _cosmicPalmBeamSpawnLocation.position;
                spawnedCosmicPalmBeam.transform.rotation = _cosmicPalmBeamSpawnLocation.rotation;

                //If the beam is active, have it grow larger and shoot further into the distance
                if(_laser.gameObject.activeSelf)
                {
                    LineRenderer lineRenderer = _laser.GetComponent<LineRenderer>();
                    //https://docs.unity3d.com/ScriptReference/LineRenderer.SetPosition.html
                    //initialize length of beam to 0
                    float previousBeamLength = lineRenderer.GetPosition(1).z;
                    float updatedBeamLength = previousBeamLength + (cosmicPalmBeamSpeed * Time.deltaTime);
    
                    //Ensure hitbox of spawned beam moves
                    spawnedCosmicPalmBeam.GetComponent<BoxCollider>().center = new Vector3(0,0,updatedBeamLength);
                    Vector3 beamLength = new Vector3(0,0,updatedBeamLength);
                    lineRenderer.SetPosition(1,beamLength);
                }

    
                //let's have the character look at where the beam is firing
                //transform.rotation = Quaternion.Euler(0, _cosmicPalmBeamSpawnLocation.transform.rotation.eulerAngles.y, 0);
                transform.rotation = Quaternion.Euler(_cosmicPalmBeamSpawnLocation.transform.rotation.eulerAngles.x, _cosmicPalmBeamSpawnLocation.transform.rotation.eulerAngles.y, 0);
            }
            if(!animator.GetBool("CosmicPalmAttack"))
            {
                //reset player's rotation to an upright neutral position. Experiment :D You can even create a flip animation to flip back into a neutral rotation for flair.
                transform.rotation = Quaternion.Euler(0,0,0);
                stopCosmicPalmBeam();
            }
        }
    }

    void stopCosmicPalmBeam()
    {
        if(spawnedCosmicPalmBeam != null)
        {
            /*spawnedCosmicPalmBeam.gameObject.transform*/
            //Destroy the beam after 3 seconds

            Destroy(spawnedCosmicPalmBeam, 3);
            firePalmIntoDistance = true;
        }
    }

    void Dash()
    {
        //print("Dashing");
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
    /*Inspired by the algorithm provided here http://www.footnotesforthefuture.com/words/wall-running-1/*/
    void CheckForWall()
    {
        RaycastHit hit;
        isWallRight = Physics.Raycast(_wallRunChecker.transform.position, _wallRunChecker.right, out hit, 1.0f, Wall);
        if(isWallRight){
            Debug.DrawRay(_wallRunChecker.transform.position, _wallRunChecker.right.normalized * hit.distance, Color.magenta );
            wallVector = -Vector3.Cross(hit.normal, Vector3.up).normalized;
            wallJumpDirection = new Vector3(-1,0,1).normalized;
            //wallJumpDirection = -wallJumpDirection;
            //print("Wall is to the Right");
        }

       
        isWallLeft = Physics.Raycast(_wallRunChecker.transform.position, -_wallRunChecker.right, out hit, 1.0f, Wall);
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

    void StartWallRun(string direction)
    {
        animator.SetBool("WallRunning", true);
        animator.SetBool("Jumping", false);
        if(direction == "right")
        {
            //TODO: Implement 
            animatorOverrideController["rig|wallRunLeft"] = wallRunningAnimationClip[0];
        }

        if(direction == "left")
        {
            animatorOverrideController["rig|wallRunLeft"] = wallRunningAnimationClip[1];
        }

        if(direction == "front")
        {
            animatorOverrideController["rig|wallRunLeft"] = wallRunningAnimationClip[2];
        }

        print("wallrunning");
        _isWallRunning = true;
    }

    void ExitWallRun()
    {
         animator.SetBool("WallRunning", false);
        _isWallRunning = false;
    }
    void toggleMovement(bool toggle)
    {
        canMove = toggle;
    }

    //New Unity Input Management.
    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext ctx) => jumpPressed = ctx.ReadValueAsButton();
    public void OnCosmicPalm(InputAction.CallbackContext ctx) => cosmicPalmInput = ctx.ReadValueAsButton();
    public void OnDash(InputAction.CallbackContext ctx) => dashInput = ctx.ReadValueAsButton();
    public void OnLockOn(InputAction.CallbackContext ctx) => lockOnInput = ctx.ReadValueAsButton();
    public void OnLockOnSwitchTargetRight(InputAction.CallbackContext ctx) => targetSwitchRightInput = ctx.ReadValueAsButton();
    public void OnLockOnSwitchTargetLeft(InputAction.CallbackContext ctx) => targetSwitchLeftInput = ctx.ReadValueAsButton();
    public void OnEnviromentInteraction(InputAction.CallbackContext ctx) => enviromentActionInput = ctx.ReadValueAsButton();

    //https://answers.unity.com/questions/242648/force-on-character-controller-knockback.html?_ga=2.213933971.521934289.1611610771-608714207.1587856867
    public void AddImpact(Vector3 direction, float force)
    {
       direction.Normalize();
       if(direction.y < 0)
       {
           //reflects downard force
           direction.y = -direction.y;
       }
       impact += direction.normalized * force / mass;
       print("Adding impact: " + impact);
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
                foes[iter] = foe.transform;
                iter++;
            }
        } 

        return foundTargets;
    }

    public GameObject GetCurrentTarget()
    {
        print("Current Target" + foes[currentTarget].name);
        return targetToLock.GetComponent<Foe>().Head.gameObject;
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


    void Mantle()
    {
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

        animator.SetBool("Landing", true);
        animator.SetBool("Falling", false);
        print("Mantling");
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
}
