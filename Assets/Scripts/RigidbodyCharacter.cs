using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCharacter : MonoBehaviour
{

    public static float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;

    public Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    //An object with a sphere collider that determines how a player can jump
    private Transform _groundChecker;

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetComponent<ThirdPersonMovement>()._groundChecker;
        print(_groundChecker.name);

        //Experimental, adds a lil pop to the character.
        //_body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        _inputs = Vector3.zero;
        _inputs.x = -Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");

        Debug.Log("Rigidbody character transform forward before manipulation" + transform.forward);
        if (_inputs != Vector3.zero)
        {
            //The value below at least works
            //transform.forward = _inputs;

            //Experimental
            if(transform.forward.y > 0 && transform.forward.z > 0)
            {
                transform.forward = _inputs;
            } 
            else if(transform.forward.y < 0 && transform.forward.z > 0)
            {
                transform.forward = new Vector3(_inputs.x, _inputs.y, -_inputs.z);
            } 
            else if(transform.forward.y > 0 && transform.forward.z < 0)
            {
                transform.forward = new Vector3(-_inputs.x, _inputs.y, _inputs.z);
            }
            else if(transform.forward.y < 0 && transform.forward.z < 0)
            {
                transform.forward = new Vector3(-_inputs.x, _inputs.y, -_inputs.z);
            }
            /*if(transform.forward.z > 0 && transform.forward.y > 0)
            {
                transform.forward = _inputs;
            } else if(transform.forward.z < -0.1f)
            {
                transform.forward = new Vector3(-_inputs.x, _inputs.y, -_inputs.z);
            }*/
        }


        /*if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }*/

        /*if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
            _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        }*/

        print("Rigidbody Character _inputs: " + _inputs);
        //print("Rigidbody Character velocity: " + _body.velocity);
        //print("Rigidbody Character forward transform: " + transform.forward);
    }


    void FixedUpdate()
    {
        _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
    }

    public Vector3 GetInputVector()
    {
        return _inputs;
    }
}
