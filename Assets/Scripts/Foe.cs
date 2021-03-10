using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Foe : MonoBehaviour
{
    public Transform Head;

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
            transform.LookAt(target);
            navMeshAgent.SetDestination(target.position);
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
}
