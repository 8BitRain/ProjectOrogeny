using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Courtesy of https://www.youtube.com/watch?v=Xgh4v1w5DxU&t=13s*/
public class Grapple : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform grappleTip, player;
    //To Set up with Third person view camera follow script
    public Transform camera;
    public float swingSpeed = 50;
    public float originalPlayerSpeed = 5;

    public float maxDistance = 100f;

    [Range(0,10)]public float jointMaxDistance = .8f;
    [Range(0,10)]public float jointMinDistance = .25f;

    private float distanceFromGrappleTarget = -1.0f;

    //Joint Physics
    private SpringJoint joint;
    public float spring = 30;
    public float damper = 7f;
    public float massScale = 4.5f;
    

    private bool _isGrappling = false;

    void Awake(){
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = transform.GetComponentInParent<AbilityEntity>().GetAgent();
        camera = player.GetComponent<ThirdPersonMovement>().cam;
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
        if(player != null && !_isGrappling)
        {
            StartGrapple();
            print("Grappling: Grapple Started");
            //player.
        }

        if(_isGrappling)
        {
            //Reel in!
            //joint.maxDistance = Vector3.Distance(player.position, grapplePoint);
            print("Grappling: Reeling player in " + "Distance to grapplePoint: " + joint.maxDistance);

            distanceFromGrappleTarget = Vector3.Distance(player.position, grapplePoint);

            joint.spring = spring;
            joint.damper = damper;
        }

        if(distanceFromGrappleTarget != -1 && distanceFromGrappleTarget < 10)
        {
            StopGrapple();
        }
    }

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
            joint.maxDistance = .1f;
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

    void StopGrapple(){

        _isGrappling = false;
        lineRenderer.positionCount = 0;
        distanceFromGrappleTarget = -1;
        print("Grappling: Grapple Ended");
        RigidbodyCharacter.Speed = originalPlayerSpeed;
        Destroy(joint);

        //Start Resetting players information
        //Turn off character controller & Turn the rigid body on
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<RigidbodyCharacter>().enabled = false;
        player.GetComponent<ThirdPersonMovement>()._controller.enabled = true;
        player.GetComponent<ThirdPersonMovement>().moveCharacter = true;
        player.GetComponent<ThirdPersonMovement>().togglePlayerMovementControl(true);
        player.GetComponent<ThirdPersonMovement>().applyGravity = true;


        player.GetComponent<ThirdPersonMovement>().DisengageDynamicTargetLock();

        Destroy(this.transform.parent.gameObject);
    }

    void DrawGrapple(){
        //Don't draw grapple when there is no joint
        if(!joint) return;

        lineRenderer.SetPosition(0, grappleTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
