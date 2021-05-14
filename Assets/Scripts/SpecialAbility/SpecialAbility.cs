using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public abstract class SpecialAbility : MonoBehaviour
{
    [Header("Special Ability Settings")]
    /* name: The name of the ability*/ 
    public System.String abilityName;

    /* abilitySpeed: if the ability has a movementComponent. How fast?*/ 
    public float abilitySpeed = 5;

    /* duration: The duration of the ability. 0 denotes the ability is instant. Should each stage of an ability have a duration?*/
    public float duration = 15;

    /*AbilityState: the state of the ability precast,cast,channeling,recoil could be an enum */
    public enum AbilityState {Cast, Channeling, Action, Recoil}
    public AbilityState abilityState;

    /*castTime: The time it takes to cast the ability Useful for animation/movement lock */
    public float castTime = 0;

    /*castTime: The time it takes to cast the ability to channel Useful for animation/movement lock */
    public float channelingTime = 0;

    /*actionTime: The time it takes to cast the ability to channel Useful for animation/movement lock */
    public float actionTime = 0;

    /*recoilTime: The time it takes to cast the ability to recoil Useful for animation/movement lock */
    public float recoilTime = 0;

    /* cooldownTime: The time it takes to cast the ability Useful for animation/movement lock */
    public float cooldownTime = 0;

    /* cooldownTimer: timer until cooldown is complete */
    protected float cooldownTimer = 0;

    /* _timer: Internal timer to use when time needs to be logged */
    protected float _timer = 0;

    /* started: Has the skill been started?*/
    protected bool _started = false;

    /* started: Has the skill been initialized with player relevant information?*/
    protected bool _initialized = false;


    //Special Attack Spawn Settings
    [Header("Special Ability Type Settings")]
    public AbilityType abilityType;
    public enum AbilityType {Beam, Throw, Physical, Emission};

    //Attached AbilityEntitity(s)
    public AbilityEntity[] abilityEntities;
    
    //Information about the skillUser
    /* Animator: The spawn Location of this ability*/
    [HideInInspector]protected Transform spawn;
    /* Agent: The skill userr*/ 
    [HideInInspector]protected Transform agent;
    /* Animator: The animator to be used for this ability*/ 
    [HideInInspector]protected Animator agentAnimator;

    [Header("Debug Info Toggle")]
    public bool toggleDebug = false;

    void Start()
    {
        if(castTime > 0)
        {
            //could create a method here that plays the cast animation first
        }
    }

    void Update()
    {
        if(_initialized)
        {
            UpdateAbility();
        }
    }

    //necessary for physics calls.
    void FixedUpdate()
    {
        if(_initialized)
        {

        }
    }

    //TODO: Double check that for overriding methods, this method must be declared virtual void
    public void CreateAbility(Transform spawn, Transform agent, Animator agentAnimator)
    {
        this.spawn = spawn;
        this.agent = agent;
        this.agentAnimator = agentAnimator;

        if(spawn == null || agent == null || agentAnimator == null)
        {
            this._initialized = false;
        }
        else
        {
            this._initialized = true;
        }

        //The ability always starts off in a cast state unless the animation state is set to another value.
        this.agentAnimator.Play(GetAnimationStateName());
    }

    public virtual void ReadAbilityInfo()
    {

    }

    public virtual void UpdateAbility(){}

    public void DestroyAbility()
    {
        Destroy(this.gameObject);
    }

    public void SetAbilityState(AbilityState abilityState)
    {
        this.abilityState = abilityState;
    }

    public AbilityState GetAbilityState()
    {
        return this.abilityState;
    }

    //Play's an ability based on target name + ability name + ability state
    public string GetAnimationStateName()
    {
        //Example Grounded.Reflector.eston_reflector_cast
        return "Grounded." + char.ToUpper(this.abilityName[0]) + this.abilityName.Substring(1) + "." + this.agent.name.ToLower() + "_" + this.abilityName.ToLower() + "_" + this.abilityState.ToString().ToLower();
    }

    //Get's the current animation name from the animator
    public string GetCurrentAnimationStateName()
    {
        //Debug.Log(this.agentAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        return this.agentAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    //Example Grounded.Reflector.eston_reflector_cast should return cast TODO: Write a test for this
    //Example eston_rig|eston_reflector_cast should return cast
    public string GetAnimationStateSuffix()
    {
        //We want the last value in the animation name
        int suffixPosition = GetCurrentAnimationStateName().Split('_').Length - 1;
        return GetCurrentAnimationStateName().Split('_')[suffixPosition];
    }

    public Transform GetAgent()
    {
        return this.agent;
    }

    public Transform GetSpawn()
    {
        return this.spawn;
    }

    public Animator GetAgentAnimator()
    {
        return this.agentAnimator;
    }

    public float GetTimer()
    {
        return this._timer;
    }

    public void SetTimer(float time)
    {
        this._timer = time;
    }

    public void ResetTimer()
    {
        this._timer = 0;
    }

}