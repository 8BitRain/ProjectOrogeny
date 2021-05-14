using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorAbility : SpecialAbility
{
    /*public override void CreateAbility()
    {
        
    }*/

    public override void UpdateAbility()
    {
        //Listener that polls for a switch in the current animation name.  
        switch (GetAnimationStateSuffix())
        {
            case "cast":
                this.abilityState = AbilityState.Cast;
                Debug.Log("Casting");
                break;
            case "channel":
                this.abilityState = AbilityState.Channeling;
                Debug.Log("Channeling");
                break;
            case "effect":
                this.abilityState = AbilityState.Action;
                Debug.Log("Effect");
                break;
            default:
                Debug.Log("No longer using ability destroy this object");
                Destroy(this.gameObject);
                break;
        }

        switch (this.GetAbilityState())
        {
            case AbilityState.Cast:
                Debug.Log("Timer Value: " + GetTimer());
                if(GetTimer() == 0)
                {
                    Cast();
                }
                return;
            case AbilityState.Channeling:
                Channeling();
                return;
            case AbilityState.Action:
                Action();
                return;
            case AbilityState.Recoil:
                Recoil();
                return;
            default:
                break;
        }
    }

    public override void ReadAbilityInfo()
    {
        base.ReadAbilityInfo();
    }

    public void Cast()
    {
        Debug.Log("Cast is called");
        //Set the timer
        if(GetTimer() == 0)
        {
            SetTimer(3);
        }

        //Play the animation
        //this.agentAnimator.Play(GetAnimationStateName());


        //Wait a certain number of seconds
        //Proceed to the next state of the ability 

        //LOG Cast
        Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is casting");
    }

    public void Channeling()
    {
        Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is channeling");
        //Play the animation
        //Generate the dome that will surround Eston is this an abilityEntity? Something that spawns and has a hitbox?

    }

    public void Action()
    {
        Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is in action state");
        //Play the animation
        //Deal damage to a target that is hit
        //Proceed to the next animation
    }

    public void Recoil()
    {
        Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is in action recoiling");
    }
}
