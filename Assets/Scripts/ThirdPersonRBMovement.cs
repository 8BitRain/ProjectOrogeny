 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonRBMovement : MonoBehaviour
{
    /*==== RigidBody Person Movement Vars ====*/
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;

    public Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    //An object with a sphere collider that determines how a player can jump
    public Transform _groundChecker;

    /*===== New Third Person Movement Vars ======*/
    private Vector3 moveDir;

    /*==== Third Person Movement Vars ====*/
    //public float speed = 6.0f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public CharacterController controller;
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

        //_body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove){
            /*Rigidbody Movement*/
            _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

            _inputs = Vector3.zero;
            _inputs.x = Input.GetAxis("Horizontal");
            _inputs.z = Input.GetAxis("Vertical");
            if (_inputs != Vector3.zero)
                transform.forward = _inputs;

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            }

            /*Character Controller defined values*/
            float horizontal = Input.GetAxisRaw("Horizontal");
            float veritical = Input.GetAxisRaw("Vertical");
            
            Vector3 direction = new Vector3(horizontal, 0, veritical).normalized;


            if(direction.magnitude >= .1f){
                //Atan2 returns angle between x axis and the angle between 0
                //Gives us an angle in radians
                //Add the rotation of the camera on the y axis on to the camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                
                //Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                /*Character Controller movement*/
                //controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

            if(horizontal != 0 || veritical != 0){
                animator.SetBool("Running", true);
            } else {
                animator.SetBool("Running", false);
            }
        }
    }
     
    void FixedUpdate()
    {
        _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
    }


     void toggleMovement(bool toggle)
    {
        canMove = toggle;
    }
}
