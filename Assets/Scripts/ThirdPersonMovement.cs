using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    //Movement
    [Header("Movement Settings")]
    public float speed = 6.0f;
    private float defaultSpeed;
    public float acceleration = .1f;
    public float friction = .025f;
    public float maxSpeed = 10.0f;
    public float gravity = -9.81f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    
    public bool glideOnGround = false;

    //Jump Related Code
    [Header("Jump Settings")]
    public Transform _groundChecker;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public float JumpHeight = 2f;
    private Vector3 _velocity;
    private bool _isGrounded = true;


    //Movement Related
    private bool moveCharacter;

    //Wall Running
    [Header("WallRunning Settings")]
    public Transform _wallRunChecker;
    public float wallDistance = 0.2f;
    public LayerMask Wall;
    public float WallRunMaxHeight = 1f;
    bool isWallRight;
    bool isWallLeft;
    private Vector3 wallVector;
    private Vector3 wallJumpDirection;
    private bool _isWallRunning = false;

    //Enviromental EFX
    [Header("Enviromental EFX")]
    public float cosmicWindDistance = .1f;
    private bool isCosmicWindFloating = false;
    public float windRiseSpeed = 1f;
    //Dictates y velocity so player does not sink in uprising wind. 
    //0 represents no sinking, while the min value represents sinking to a degree. -10 would represent bobbing.
    [Range(0.0f, -10.0f)] public float windResetVelocityY = 0;

    

    
    //Camera Management
    [Header("Camera Management")]
    public GameObject mainCamera;
    public GameObject aimCamera;
    public GameObject aimReticle;

    //Time Management
    //TODO: Create a time manager
    [Header("Time Settings")]
     private bool slowTime;

    //Combat Management
      [Header("Combat Timer Management")]
    public float cosmicPalmTimer = 0;
    private bool startTimer = false;

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
    //https://docs.unity3d.com/ScriptReference/AnimatorOverrideController.html
    protected AnimatorOverrideController animatorOverrideController;

    // Start is called before the first frame update
    void Start()
    {
        animator = Orogene.GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        defaultSpeed = speed;
        playingAnim = false;
        moveCharacter = true;
        //animator
        running = false;

        aimReticle.SetActive(false);
        slowTime = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float veritical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0, veritical).normalized;

            //Grounded status
            _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = 0f;
                animator.SetBool("Jumping", false);

                //No longer falling
                if(animator.GetBool("Falling")){
                    print("Not falling");
                    animator.SetBool("Landing", true);
                    animator.SetBool("Falling", false);
                }
            }

            //WALLRUNNING 2.0 using Raycasts
            CheckForWall();
            if(isWallRight)
            {
                StartWallRun("right");
            }
            if(isWallLeft)
            {
                StartWallRun("left");
            }
            if(!isWallLeft && !isWallRight)
            {
                ExitWallRun();
            }

            //MOVEMENT
            /* For acceleration and deceleration moveDir is defined by default as the local transform forward vector.*/
            /* For tighter movement, move moveDir inside the scope of the moveCharacter and direction.magnitude check*/
            //This introduces a bug similar to the super bounce! moveDir being set to transform.forward affects wall jump movement.
            //Vector3 moveDir = transform.forward;
            if(direction.magnitude >= .1f && moveCharacter)
            {
                //Increase speed when we want to accelerate or glide. For now we call this gliding due to the flight like effect. Think Dissidia.
                if(glideOnGround)
                {
                    if(speed < maxSpeed){
                        speed = speed + acceleration * Time.deltaTime;
                    }
                }

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
                    if(!glideOnGround)
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

            //Constant subtration of friction for glide/decceleration
            //https://www.gamasutra.com/blogs/MarkVenturelli/20140821/223866/Game_Feel_Tips_II_Speed_Gravity_Friction.php
            if(glideOnGround)
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
                    this.glideOnGround = false;
                    speed = defaultSpeed;
                }   
            }
         

            

            //Controller Input
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

            if (Input.GetButton("PlayerJump")){
                if(_isGrounded && !_isWallRunning){
                    print("JUMP");
                    //reset y_velocity to prevent super bouncing
                    _velocity.y = 0; 
                    _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
                    animator.SetBool("Running", false);
                    animator.SetBool("Jumping", true);
                }
                if(_isWallRunning && !_isGrounded){
                    //DetachFromWall
                    print("WALLJUMP");
                    animator.SetBool("Running", false);
                    animator.SetBool("WallRunning", false);
                    animator.SetBool("Jumping", true);
                    isWallLeft = false;
                    isWallRight = false;
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
            if(cosmicPalmTimer <= 0 && startTimer)
            {
                animator.SetBool("CosmicPalmAttack", false);
                startTimer = false;
                cosmicPalmTimer = 0;
            }

            if(Input.GetAxis("Right Trigger") == 1.0f && cosmicPalmTimer == 0){
                //Gravity,Sensitivty, and deadzone controller values were pulled from https://wiki.unity3d.com/index.php/Xbox360Controller
                print("COSMIC PAWLLLMMM");
                animator.SetBool("CosmicPalmAttack", true);
                cosmicPalmTimer = combatAnimationClip[0].length;
                startTimer = true;
            }

            if(horizontal != 0 || veritical != 0)
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
            print(_velocity.y);
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



            

        }
    }

    /*Inspired by the algorithm provided here http://www.footnotesforthefuture.com/words/wall-running-1/*/
    void CheckForWall()
    {
        RaycastHit hit;
        isWallRight = Physics.Raycast(_wallRunChecker.transform.position, _wallRunChecker.right, out hit, wallDistance, Wall);
        if(isWallRight){
            Debug.DrawRay(_wallRunChecker.transform.position, _wallRunChecker.right.normalized * hit.distance, Color.magenta );
            wallVector = -Vector3.Cross(hit.normal, Vector3.up).normalized;
            wallJumpDirection = new Vector3(-1,0,1).normalized;
            //wallJumpDirection = -wallJumpDirection;
            //print("Wall is to the Right");
        }
       
        isWallLeft = Physics.Raycast(_wallRunChecker.transform.position, -_wallRunChecker.right, out hit, wallDistance, Wall);
        if(isWallLeft){
            Debug.DrawRay(_wallRunChecker.transform.position, -_wallRunChecker.right.normalized * hit.distance, Color.green );
            wallVector = Vector3.Cross(hit.normal, Vector3.up).normalized;
            wallJumpDirection = new Vector3(1,0,1).normalized;
        }
       
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

    IEnumerator ShowReticle()
    {
        yield return new WaitForSeconds(0.25f);
        aimReticle.SetActive(enabled);
        SetSlowTime(true);
    }

    public void SetSlowTime(bool on)
    {
        float time = on ? .1f : 1;
        Time.timeScale = time;
        Time.fixedDeltaTime = time * .02f;
    }
}
