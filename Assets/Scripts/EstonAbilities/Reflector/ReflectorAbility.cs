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

    public override void Cast()
    {
        Debug.Log("Cast is called");
        //Set the timer
        if(GetTimer() == 0)
        {
            SetTimer(3);
        }

        //Play the animation
        //this.agentAnimator.Play(GetAnimationStateName());

        //Make the player float. 
        agent.GetComponent<ThirdPersonMovement>().SetPlayerGravity(false);
        //agent.GetComponent<ThirdPersonMovement>()._controller.Move(Vector3.up * 50 * Time.deltaTime);
        StartCoroutine(FloatPlayer(3, agent.GetComponent<ThirdPersonMovement>()._controller));


        //Wait a certain number of seconds
        //Proceed to the next state of the ability
        
        //Generate left and right orb
        AbilityEntity entityOrbL =  Instantiate(this.abilityEntitites[1], agent.transform.position, Quaternion.Euler(0,0,0));
        AbilityEntity entityOrbR =  Instantiate(this.abilityEntitites[2], agent.transform.position, Quaternion.Euler(0,0,0));

        //Attach the orbs to the player so that it moves with the player
        entityOrbL.transform.parent = agent;
        entityOrbR.transform.parent = agent;

        //LOG Cast
        Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is in casting state");
    }

    public override void Channeling()
    {
        if(!triggerChannelingStateOnce)
        {
            Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is in channeling state");
            //Play the animation
            
            //AddSpawnedAbilityEntity(entity);


            ActivateChannelingStateOnce();
        }
    }

    public override void Action()
    {
        if(!triggerActionStateOnce)
        {
            Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is in action state");
            //Play the animation

            //Generate the dome that will surround Eston is this an abilityEntity? Something that spawns and has a hitbox?
            AbilityEntity entity =  Instantiate(this.abilityEntitites[0], agent.transform.position, Quaternion.Euler(-90,0,0));

            //Attach the dome to the player so that it moves with the player
            entity.transform.parent = agent;

            //Have the player affected by gravity again.
            agent.GetComponent<ThirdPersonMovement>().SetPlayerGravity(true);

            //Proceed to the next animation
            ActivateActionStateOnce();
        }
    }

    public override void Recoil()
    {
        Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is in action recoiling");
    }

    IEnumerator FloatPlayer (float time, CharacterController controller) 
    {
        float elapsedTime = 0;

        
        //Rise
        while (elapsedTime < time)
        {
            controller.Move(Vector3.up * 2 * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
