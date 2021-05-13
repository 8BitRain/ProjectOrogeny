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

    /*state: the state of the ability precast,cast,channeling,recoil could be an enum */
    public enum Stage {Warmup, Cast, Channeling, Recoil}
    protected int state;

    /*castTime: The time it takes to cast the ability Useful for animation/movement lock */
    public float castTime = 0;

    /*castTime: The time it takes to cast the ability to channel Useful for animation/movement lock */
    public float channelingTime = 0;

    /*recoilTime: The time it takes to cast the ability to recoil Useful for animation/movement lock */
    public float recoilTime = 0;

    /* cooldownTime: The time it takes to cast the ability Useful for animation/movement lock */
    public float cooldownTime = 0;

    /* cooldownTimer: timer until cooldown is complete */
    protected float cooldownTimer = 0;

    /* _timer: Internal timer to use when time needs to be logged */
    private float _timer = 0;

    /* started: Has the skill been started?*/
    private bool _started = false;

    //Special Attack Spawn Settings
    [Header("Special Ability Type Settings")]
    public AbilityType abilityType;
    public enum AbilityType {Beam, Throw, Physical, Emission};

    //Information about the skillUser
    [HideInInspector]protected Transform spawn;
    [HideInInspector]protected Transform agent;
    [HideInInspector]protected Animator agentAnimator;

    [Header("Debug Info Toggle")]
    public bool toggleDebug = false;

    void Start()
    {
        CreateAbility();
    }

    void Update()
    {
        UpdateAbility();
    }

    //necessary for physics calls.
    void FixedUpdate()
    {

    }

    //TODO: Double check that for overriding methods, this method must be declared virtual void
    public virtual void CreateAbility()
    {

    }

    public virtual void ReadAbilityInfo()
    {

    }

    public virtual void UpdateAbility()
    {

    }

    public virtual void DestroyAbility()
    {

    }

}