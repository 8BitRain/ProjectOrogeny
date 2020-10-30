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

    //Wall Running
    private bool _isWallRunning = false;
    public Transform _wallRunChecker;
    public float wallDistance = 0.2f;
    public LayerMask Wall;
    public float WallRunMaxHeight = 1f;
    
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
    public float animCounterInitialValue;
    private float animCounter;
    private bool m_running;

    public Animation animation;

    public Animator animator;
    public AnimatorClipInfo an;

    // Start is called before the first frame update
    void Start()
    {
        animator = Orogene.GetComponent<Animator>();
        playingAnim = false;
        animCounter = animCounterInitialValue;
        //animator
        running = false;
        m_running = false;

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
            }

            _isWallRunning = Physics.CheckSphere(_wallRunChecker.position, wallDistance, Wall, QueryTriggerInteraction.Ignore);
            if (_isWallRunning)
            {
                animator.SetBool("WallRunning", true);
                print(direction);
                print("WallRunning");
                //Rotating character while wall running. Temporary w/ no wall run animation
                if(direction.z > 0)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                } 
                else 
                {
                    transform.rotation = Quaternion.Euler(0f, 180f, 90f);
                }
                //Enable to allow controlled movement on wall
                //_isGrounded = true;

                //Modify code to use the same jumping function for wall run.
                //Mathf.Sqrt(JumpHeight * -2f * gravity);
                //Don't set velocity to let gravity effect Wall Run
                //_velocity.y = 0f; 
            }
            else
            {
                animator.SetBool("WallRunning", false);
                //print("OffWall");
            }
            
            if(direction.magnitude >= .1f)
            {
                //Atan2 returns angle between x axis and the angle between 0
                //Gives us an angle in radians
                //Add the rotation of the camera on the y axis on to the camera
                /*===== ThirdPersonCamera_GamePad Rotation*/
                
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                Vector3 moveDir;
                if(_isWallRunning == false)
                {
                    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                    //Move Forward as normal
                    moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                }
                else
                {
                    ///Move Forward and up slightly
                    //moveDir = Quaternion.Euler(0f, targetAngle, 0f) * new Vector3(0, 1, 1);

                    //Move Forward along wall
                    moveDir = Quaternion.Euler(0f, targetAngle, 0f) * new Vector3(0, 0, 1);

                    //Look up parabolic motion. There seem to be animation cuves, bezier curves, and other lines to use.
                    //I'm curious what mathmatical functions simulate parbolas. How do you achieve x^2?

                    //The square root function looks like an rotated > 
                    //_velocity.y += (Mathf.Pow(Mathf.Sqrt(WallRunMaxHeight * -2f * gravity), 3));
                    _velocity.y += Mathf.Sqrt(WallRunMaxHeight * -2f * gravity);

                    //Enable for constant y axis movement
                    //moveDir = Quaternion.Euler(0f, targetAngle, 0f) * new Vector3(0, 1, 1);
                }
                _controller.Move(moveDir.normalized * speed * Time.deltaTime);            
            }

            //Controller Input
            if(Input.GetAxis("Aim") == 1 && !aimCamera.activeInHierarchy)
            {
                print("AIM");
                mainCamera.SetActive(false);
                aimCamera.SetActive(true);
                //aimReticle.SetActive(true);
                StartCoroutine(ShowReticle());

            }
            else if(Input.GetAxis("Aim") != 1 && !mainCamera.activeInHierarchy)
            {
                mainCamera.SetActive(true);
                aimCamera.SetActive(false);
                aimReticle.SetActive(false);
                SetSlowTime(false);
            }

            if (Input.GetButton("PlayerJump") && _isGrounded && !_isWallRunning){
                print("JUMP");
                //TODO: Better Jumping Arc //Getting a better jumping arc will probably be factored here
                _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
            }

            if(horizontal != 0 || veritical != 0)
            {
                if(!_isWallRunning)
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
