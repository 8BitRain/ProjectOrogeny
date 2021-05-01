using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Transform Agent;
    public LayerMask[] layers;
    public Transform spawnPoint;
    public GameObject combatAction;
    
    public CombatType combatType;
    public enum CombatType {Light, Medium, Heavy, Misc};


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
                Debug.Log("Combat: Eston's fists Collided with Enemy: " + other.gameObject.name);
                if(this.combatType == CombatType.Light)
                {
                    Debug.Log("Combat: Light Attack triggered");
                    //CombatAction combatActionSpecifics = new CombatAction(10, 10, Agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0), other.gameObject, null, 1);
                    GameObject combatActionInstance = Instantiate(combatAction);
                    combatActionInstance.GetComponent<CombatAction>().Initialize(5f, 1000, Agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0), other.gameObject, null, 1);
                }
            }
        }
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
}
