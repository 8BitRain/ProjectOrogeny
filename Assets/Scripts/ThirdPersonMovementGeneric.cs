 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovementGeneric : MonoBehaviour
{
    public float speed = 6.0f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public CharacterController controller;
    public Transform cam;

    private bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //print("Sess Moving?: " + canMove);
        if(canMove){
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
                
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
                //animator.Play("run");
                
                
            }

            if(horizontal != 0 || veritical != 0){
                /*if(playingAnim != true){
                    animator.Play("run");
                    playingAnim = true;
                    animCounter = animCounterInitialValue;
                }*/
                //animator.SetBool("run")
            }

            
            /*if(playingAnim && animCounter > 0){
                animCounter -= Time.deltaTime;
            }

            if(animCounter < 0){
                playingAnim = false;
            }

            print(animCounter);*/
            }
    }

    void toggleMovement(bool toggle)
    {
        canMove = toggle;
    }
}
