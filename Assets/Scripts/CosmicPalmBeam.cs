using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmicPalmBeam : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask Player;
    public float force;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Define position based on a constant x, y , then base final 
        float value = this.GetComponent<BoxCollider>().center.z;
        Debug.DrawRay(transform.position, transform.forward * (this.GetComponent<BoxCollider>().center.z + 2f), Color.red);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, this.GetComponent<BoxCollider>().center.z + 2f, Player))
        {
            ThirdPersonMovement playerCharacter = hit.transform.GetComponent<ThirdPersonMovement>();
            playerCharacter.AddImpact(transform.forward, force);
            playerCharacter.healthBar.SetHealth(playerCharacter.healthBar.GetHealth() - 10.0f);
        }
    }
}
