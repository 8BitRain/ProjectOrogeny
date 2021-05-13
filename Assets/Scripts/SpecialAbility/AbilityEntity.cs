using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

/* An ability entity can be the dome summoned, the beam summoned, any additional game object generated from a special ability. Also holds damage properties?*/
public abstract class AbilityEntity : MonoBehaviour
{
    [Header("Ability Entity")]
    /* name: The name of the ability entity*/ 
    public System.String entityName;

    /* name: The hitbox attached to the entity*/
    public HitBox hitBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
