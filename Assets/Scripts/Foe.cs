using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Foe : MonoBehaviour
{
    public Transform Head;

    public LayerMask Player;
    
    public BoxCollider hitBox;

    private NavMeshAgent navMeshAgent;
    private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        if(this.GetComponent<NavMeshAgent>() != null)
        {
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            AssignTarget();
        }

        SeekPlayer();
    }

    void SeekPlayer()
    {
        if(this.navMeshAgent != null && target != null)
        {
            //transform.LookAt(target);
            Vector3 position = transform.position + hitBox.center;
            RaycastHit hit;
            float targetDistance = (transform.position - target.position).magnitude;

            /*if(Physics.SphereCast(position, 15f, transform.forward, out hit, 15f, Player))
            {
        
                targetDistance = hit.distance;
                print("PlayerHit");
                print(hit.transform);
            }*/

            //print("Sphere Cast reported target distance: " + targetDistance);
            print("Manual calculation of distance: " + (transform.position - target.transform.position).magnitude);

            if(targetDistance >= 15)
            {
                print("Navigate to player");
                navMeshAgent.SetDestination(target.transform.position);
                navMeshAgent.stoppingDistance = 15;
            }

            if(targetDistance < 15) 
            {
                //navMeshAgent.
                print("Avoid player");
                OrbitTarget();
            }
            

        }
    }

    void AssignTarget()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("P1"))
        {
            if(obj.name == "Eston")
            {
                target = obj.transform;
                print("Found Eston, seeking her");
            }
        }
    }

    void OrbitTarget()
    {
        transform.RotateAround(target.transform.position, Vector3.up, 60 * Time.deltaTime);
        print("Orbiting Target: " + target.transform.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + this.hitBox.center;
        Gizmos.DrawWireSphere(position, 15f);

    }
}
