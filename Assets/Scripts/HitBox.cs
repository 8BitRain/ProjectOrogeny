using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Transform Agent;
    public LayerMask[] layers;
    public Transform spawnPoint;
    public GameObject combatAction;
    
    public CombatType combatType;
    public enum CombatType {Light, Medium, Heavy, Misc};

    private bool rumble = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var layer in layers)
        {
            if(layer == (layer | (1 << other.gameObject.layer)))
            {
                Debug.Log("Combat: Eston's fists Collided with Enemy: " + other.gameObject.name + "at: " + Time.time);
                if(this.combatType == CombatType.Light)
                {
                    Debug.Log("Combat: Light Attack triggered");
                    //CombatAction combatActionSpecifics = new CombatAction(10, 10, Agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0), other.gameObject, null, 1);
                    GameObject combatActionInstance = Instantiate(combatAction);
                    combatActionInstance.GetComponent<CombatAction>().Initialize(5f, 1000, Agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0), Agent.gameObject, other.gameObject, null, 1);

                    Gamepad.current.SetMotorSpeeds(0.25f,0.55f);
                    StartCoroutine(RumbleCountdown(.2f));
                    rumble = false;
                }
            }
        }
    }

    public void ActivateHitBox()
    {
        this.GetComponent<BoxCollider>().enabled = true;
    }

    public void DeactivateHitBox()
    {
        this.GetComponent<BoxCollider>().enabled = false;
    } 

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.matrix = transform.localToWorldMatrix;
        if(this.GetComponent<BoxCollider>().enabled)
        {
            Gizmos.DrawWireCube(this.GetComponent<BoxCollider>().center, this.GetComponent<BoxCollider>().size);
        }
        
        //Gizmos.DrawWireCube(this.transform.position, new Vector3(2,2,2));
    }

    IEnumerator RumbleCountdown (float seconds) 
    {
        int counter = 1;
        while (counter > 0) {
            yield return new WaitForSeconds (seconds);
            counter--;
        }
        Gamepad.current.SetMotorSpeeds(0,0);
        rumble = true;
        //Gamepad.current.PauseHaptics();
        //Gamepad.current.ResetHaptics();
    }
}
