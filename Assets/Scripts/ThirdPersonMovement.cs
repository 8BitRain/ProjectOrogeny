 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -9.81f;

    private Vector3 _velocity;
    private bool _isGrounded = true;
    public Transform _groundChecker;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public float JumpHeight = 2f;

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
            
            if(direction.magnitude >= .1f)
            {
                //Atan2 returns angle between x axis and the angle between 0
                //Gives us an angle in radians
                //Add the rotation of the camera on the y axis on to the camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _controller.Move(moveDir.normalized * speed * Time.deltaTime);
                //animator.Play("run");
                
                
            }

            if (Input.GetButtonDown("Jump") && _isGrounded){
                _velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
            }
            


            if(horizontal != 0 || veritical != 0)
            {
                animator.SetBool("Running", true);
            } else 
            {
                animator.SetBool("Running", false);
            }
            
            //Gravity
            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
    
        
            }
    }
     void toggleMovement(bool toggle)
    {
        canMove = toggle;
    }
}
