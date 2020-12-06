using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public GameObject followTransform;
    public GameObject mainCamera;
    public GameObject aimCamera;
    public GameObject aimReticle;
    public Transform cam;

    private float aim = 0f;
    private bool slowTime;
    public float speed = 6.0f;
    public float rotationPower = 5.0f;

    public CharacterController _controller;
    private Vector3 _look;
    // Start is called before the first frame update
    void Start()
    {
        aimReticle.SetActive(false);
        slowTime = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Vector3 direction = new Vector3(horizontal, 0, veritical).normalized;
        //Vector3 direction = new Vector3(horizontal, 0, veritical).normalized;
        _look.x = Input.GetAxis("Thumbstick X");
        _look.y = Input.GetAxis("Thumbstick Y");

        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.y * rotationPower, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        //Clamp Up  & Down Rotation
        if(angle > 180 && angle < 340){
            angles.x = 340;
        }
        else if(angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTransform.transform.localEulerAngles = angles;

        //Set players rotation based ont transform. 
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        //Controller Input
        /*if(Input.GetAxis("Aim") == 1 && !aimCamera.activeInHierarchy)
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
        }*/


    }

    IEnumerator ShowReticle()
    {
        yield return new WaitForSeconds(1f);
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
