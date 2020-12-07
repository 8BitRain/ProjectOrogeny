using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -9.81f;

    //Jump Related Code
    private Vector3 _velocity;
    private bool _isGrounded = true;
    public Transform _groundChecker;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public float JumpHeight = 2f;

    //Movement Related
    private bool moveCharacter;

    //Wall Running
    private bool _isWallRunning = false;
    public Transform _wallRunChecker;
    public float wallDistance = 0.2f;
    public LayerMask Wall;
    public float WallRunMaxHeight = 1f;
    bool isWallRight;
    bool isWallLeft;
    private Vector3 wallVector;
    private Vector3 wallJumpDirection;
    
    //Camera Management
    public GameObject mainCamera;
    public GameObject aimCamera;
    public GameObject aimReticle;

    //Time Management
    //TODO: Create a time manager
     private bool slowTime;


    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public CharacterController _controller;
    public Transform cam;
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

    public AnimationClip[] jumpingAnimationClip;

    public Animator animator;
    //https://docs.unity3d.com/ScriptReference/AnimatorOverrideController.html
    protected AnimatorOverrideController animatorOverrideController;

    // Start is called before the first frame update
    void Start()
    {
        animator = Orogene.GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;


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

            _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = 0f;
                animator.SetBool("Jumping", false);
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
                    _controller.Move(moveDir.normalized * speed * Time.deltaTime); 
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
                    _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
                    //Jump off wall at 45 degree angle
                    Vector3 jumpDirection;
                    //jump direction uses transform.TransformDirection to move player in a vector 45degrees away from wall
                    jumpDirection = transform.TransformDirection(wallJumpDirection);
                    _controller.Move(jumpDirection * speed * Time.deltaTime); 
                }
                //TODO: Better Jumping Arc //Getting a better jumping arc will probably be factored here
                //_velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
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
            _velocity.y += gravity * Time.deltaTime;
            //Getting a better jumping arc will probably be factored here
            _controller.Move(_velocity * Time.deltaTime);
            }
    }

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
       
        print(wallVector);
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
            animatorOverrideController["rig|wallRunLeft"] = jumpingAnimationClip[0];
            print("override right");
        }
        if(direction == "left")
        {
            animatorOverrideController["rig|wallRunLeft"] = jumpingAnimationClip[1];
            print("override left");
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
