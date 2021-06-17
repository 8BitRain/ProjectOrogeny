using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerAbility : SpecialAbility
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
    }

    public override void Channeling()
    {
    }

    public override void Action()
    {
        if(!triggerActionStateOnce)
        {
            Debug.Log("SpecialAbility: " + this.GetAnimationStateName() + " is in action state");
            //Play the animation

            //Generate the Grappler entity
            AbilityEntity entity =  Instantiate(this.abilityEntitites[0], agent.transform.position, agent.transform.rotation);

            //Attach the entity to the player so that it moves with the player
            entity.transform.parent = agent;

            entity.SetAgent(agent);

            //Ensure the grapple point looks at the center of the screen.
            Camera cam = GetAgent().GetComponent<ThirdPersonMovement>().cam.GetComponent<Camera>();
            //entity.GetComponent<GrapplerAbilityEntity>().GrappleTip.LookAt(GetAgent().GetComponent<ThirdPersonMovement>().cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(cam.pixelWidth/2, cam.pixelHeight/2, 0)));
            
            //Ensure the Grapple tip looks forward at the center of the screen
            entity.GetComponent<GrapplerAbilityEntity>().GrappleTip.LookAt(cam.transform.forward);

            //Ensure the ability entity is connected to the object the player's reticle is currently aiming at
            try
            {
                entity.GetComponent<GrapplerAbilityEntity>().ConnectedObject = agent.GetComponent<ThirdPersonMovement>().aimReticle.GetComponent<Reticle>().Target.transform;
            }
            catch (System.Exception)
            {
                Debug.Log("Reticle is not looking at a target: destroy this ability");
                //Ensure the entity resets player controls
                entity.GetComponent<GrapplerAbilityEntity>().GetGrapple().ResetPlayer();
                Destroy(entity.gameObject);
                Destroy(this.gameObject);
            }
            
            //Grab the Target that is connected to the grappler. This target is originally defined by the Reticle
            /*if(entity.GetComponent<GrapplerAbilityEntity>().ConnectedObject != null)
            {
                agent.GetComponent<ThirdPersonMovement>().EngageDynamicTargetLock(entity.GetComponent<GrapplerAbilityEntity>().ConnectedObject);
            }*/

            //Turn off character controller & Turn the rigid body on
            agent.GetComponent<ThirdPersonMovement>()._controller.enabled = false;
            agent.GetComponent<ThirdPersonMovement>().moveCharacter = false;
            agent.GetComponent<ThirdPersonMovement>().togglePlayerMovementControl(false);
            agent.GetComponent<ThirdPersonMovement>().applyGravity = false;
            agent.GetComponent<Rigidbody>().isKinematic = false;
            agent.GetComponent<RigidbodyCharacter>().enabled = true;

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

