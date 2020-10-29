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
    



    public GameObject followTarget;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public CharacterController _controller;
    public Transform cam;
    public Transform Orogene;

    private bool canMove = true;

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
                print("OffWall");
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
                
                

                /*===== Third Person Follow Rotation =====*/
                
                //Add the rotation of the targetFollow to the y axis
                /*float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + followTarget.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(followTarget.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                
                //Removed as it conflicts with ThirdPerson Camera Script
                //transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                followTarget.transform.rotation = Quaternion.Euler(0f, angle, 0f);
                
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _controller.Move(moveDir.normalized * speed * Time.deltaTime);*/
                //animator.Play("run");
                
                
                
            }

            if (Input.GetButton("PlayerJump") && _isGrounded && !_isWallRunning){
                print("JUMP");
                //Better Jumping Arc //Getting a better jumping arc will probably be factored here
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
}
