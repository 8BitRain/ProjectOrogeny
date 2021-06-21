using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

/* An ability entity can be the dome summoned, the beam summoned, any additional game object generated from a special ability. Also holds damage properties?*/
public abstract class AbilityEntity : MonoBehaviour
{
    [Header("Ability Entity")]
    /* entityName: The name of the ability entity*/ 
    public System.String entityName;

    /* entityName: The duration of the ability entity*/ 
    public float duration;

    /* hitBox: The hitbox attached to the entity*/
    public HitBox hitBox;

    protected float _timer;

    protected bool _initialized;

    //Information about the skillUser
    /* Agent: The skill userr*/ 
    [HideInInspector]protected Transform agent;
    /* Animator: The animator to be used for this ability*/ 

    // Start is called before the first frame update
    void Start()
    {
        SetTimer(duration);
        InitializeAbilityEntity();
    }

    // Update is called once per frame
    void Update()
    {
        if(_timer > 0)
        {
            _timer -= Time.deltaTime;
            //Debug.Log("Timer running");
        }

        //Ability Entity is completed. Delete it
        if(_timer <= 0)
        {
            Destroy(this.gameObject);
        }

        UpdateAbilityEntity();
    }

    public virtual void UpdateAbilityEntity(){}

    public virtual void InitializeAbilityEntity(){}

    public void SetDuration(float time)
    {
        this.duration = time;
    }

    public void SetTimer(float time)
    {
        this._timer = time;
    }

    public void SetAgent(Transform agent)
    {
        print("Agent to pass: " + agent.name);
        this.agent = agent; 
    }

    public Transform GetAgent()
    {
        return this.agent;
    }
}
