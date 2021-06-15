using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine;

/*Courtesy of https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=13s*/
public class Grapple : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    [Header("Grapple Settings")]
    public LayerMask whatIsGrappleable;
    public Transform grappleTip, player;
    //To Set up with Third person view camera follow script
    public Transform camera;
    public float swingSpeed = 50;
    public float originalPlayerSpeed = 5;

    [Header("Spawned Spring Settings")]
    public float maxDistance = 100f;

    [Range(0,10)]public float jointMaxDistance = .8f;
    [Range(0,10)]public float jointMinDistance = .25f;

    private float distanceFromGrappleTarget = -1.0f;

    //Joint Physics
    private SpringJoint joint;
    public float spring = 30;
    public float damper = 7f;
    public float massScale = 4.5f;

    //Grapple Input timer
    public bool InputWindow = true;
    public float inputWindowTimer = 1.0f;

    //Grapple Jump Duration
    public float jumpDuration = 2f;
    

    [Header("Player Inputs")]
    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for Enviroment Interaction ")]
    public InputActionReference enviromentInteractionButtonPressed;
    public InputActionReference jumpButtonPressed;

    private bool _isGrappling = false;

    void Awake(){
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = transform.GetComponentInParent<AbilityEntity>().GetAgent();
        camera = player.GetComponent<ThirdPersonMovement>().cam;

        StartCoroutine(InputWindowCoroutine(inputWindowTimer));
    }

    // Update is called once per frame
    void Update()
    {
     
        /*if(Input.GetKeyDown(KeyCode.G)){
            StartGrapple();
        }
        else if(Input.GetKeyUp(KeyCode.G)){
            StopGrapple();
        }*/

        if(jumpButtonPressed.action.triggered)
        {
            StopGrapple(true);
            //print("Player Jumped out of grapple");
        }

        if(player != null && !_isGrappling)
        {
            StartGrapple();
            print("Grappling: Grapple Started");
            //player.
        }

        if(_isGrappling)
        {
            //Force the player to look at the Grapple Point
            player.LookAt(grapplePoint);

            

            //Reel in!
            //joint.maxDistance = Vector3.Distance(player.position, grapplePoint);
            print("Grappling: Reeling player in " + "Distance to grapplePoint: " + joint.maxDistance);
            distanceFromGrappleTarget = Vector3.Distance(player.position, grapplePoint);

            joint.spring = spring;
            joint.damper = damper;

            if(!InputWindow)
            {
                if(enviromentInteractionButtonPressed.action.ReadValue<float>() > 0)
                {
                    print("EnviromentInteraction Button held");
                    //StopGrapple();
                } else
                {
                    StopGrapple(false);
                }
            }
        }

        if(distanceFromGrappleTarget != -1 && distanceFromGrappleTarget < 10)
        {
            StopGrapple(false);
        }
    }

    //Physics calls go
    /*void FixedUpdate()
    {
        if(jumpButtonPressed.action.triggered)
        {
            player.GetComponent<RigidbodyCharacter>()._body.AddForce(Vector3.up * Mathf.Sqrt(player.GetComponent<RigidbodyCharacter>().JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            Debug.Log("Player Jumped from grapple, apply some force");
        }
        
    }*/

    void LateUpdate()
    {
        DrawGrapple();
    }

    void StartGrapple(){
        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, whatIsGrappleable)){
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            //joint.connectedBody = hit.transform.GetComponent<Rigidbody>();
            //print("Grappler: Rigidbody connected");

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            distanceFromGrappleTarget = distanceFromPoint;
            print("Grappler: Distance from point: " + distanceFromPoint);

            //Some rules that can help with experimentation of grapple
            //https://answers.unity.com/questions/1690048/grappling-hook-with-spring-joint.html
            //Distnace grapple will try to keep from grapple point
            joint.maxDistance = jointMaxDistance;
            joint.minDistance = 0;

            //Play around with these values
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;

            //Adjust Player Speed to Swing
            RigidbodyCharacter.Speed = swingSpeed;

            _isGrappling = true;

        }
    }

    void StopGrapple(bool fromJump){

        _isGrappling = false;
        lineRenderer.positionCount = 0;
        distanceFromGrappleTarget = -1;
        print("Grappling: Grapple Ended");
        RigidbodyCharacter.Speed = originalPlayerSpeed;
        Destroy(joint);

        // If the player isn't jumping we can sart Resetting players information
        //Turn off rigidbody & turn character controller on
        if(!fromJump)
        {
            ResetPlayer();
            Destroy(this.transform.parent.gameObject);
        }

        if(fromJump)
        {
            StartCoroutine(JumpRigidbodyControlCoroutine(2.5f));
            player.GetComponent<Animator>().Play("Grounded.Airborne.backflip");
            player.GetComponent<RigidbodyCharacter>()._body.AddForce(Vector3.up * Mathf.Sqrt(player.GetComponent<RigidbodyCharacter>().JumpHeight * -4f * Physics.gravity.y), ForceMode.VelocityChange);
        }

    }

    void ResetPlayer()
    {
        Debug.Log("Player Character Controller velocity is: " + player.GetComponent<ThirdPersonMovement>().GetPlayerVelocity());
        //player.GetComponent<ThirdPersonMovement>().SetPlayerVelocity(player.GetComponent<RigidbodyCharacter>()._body.velocity);
        Vector3 playerMomentum = new Vector3(player.GetComponent<RigidbodyCharacter>()._body.velocity.x/10, player.GetComponent<RigidbodyCharacter>()._body.velocity.y, player.GetComponent<RigidbodyCharacter>()._body.velocity.z/10);
        player.GetComponent<ThirdPersonMovement>().SetPlayerVelocity(playerMomentum);
        Debug.Log("Setting velocity of player to:  " + playerMomentum);
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<RigidbodyCharacter>().enabled = false;
        player.GetComponent<ThirdPersonMovement>()._controller.enabled = true;
        player.GetComponent<ThirdPersonMovement>().moveCharacter = true;
        player.GetComponent<ThirdPersonMovement>().togglePlayerMovementControl(true);
        player.GetComponent<ThirdPersonMovement>().applyGravity = true;
        player.GetComponent<ThirdPersonMovement>().DisengageDynamicTargetLock();
    }



    void DrawGrapple(){
        //Don't draw grapple when there is no joint
        if(!joint) return;

        lineRenderer.SetPosition(0, grappleTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    IEnumerator InputWindowCoroutine (float time) 
    {
        float elapsedTime = 0;

        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        InputWindow = false;
    }

    IEnumerator JumpRigidbodyControlCoroutine(float time)
    {
        float elapsedTime = 0;
        //Destroy the grapple line
        lineRenderer.positionCount = 0;
        distanceFromGrappleTarget = -1;
        

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ResetPlayer();
        Destroy(this.transform.parent.gameObject);
    }
}
