using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenOrbisAbilityEntity : AbilityEntity
{
    public override void UpdateAbilityEntity()
    {
        try
        {
            print("AGENT: " + this.GetAgent());
            if(this.GetAgent().GetComponent<Animator>() != null)
            {
                //GetCurrentAnimationStateName();
                //this.GetComponent<Animator>().Play(GetCurrentAnimationStateName());
                PlayCloneAnimation();
            }   
        }
        catch (System.Exception)
        {
            Debug.Log("Golden Orbis Ability has no attached agent");
            throw;
        }
    }

    public override void InitializeAbilityEntity()
    {   
        
    }

    //Get's the current animation name from the animator
    public string GetCurrentAnimationStateName()
    {
        Debug.Log(this.agent.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name);
        return this.agent.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    public void PlayCloneAnimation()
    {
        Animator localAnimator = this.GetComponent<Animator>();
        
        switch (GetCurrentAnimationStateName())
        {
            case "eston_rig|ability_reflector_cast":
                localAnimator.Play("Grounded.Reflector.eston_reflector_cast");
                break;
            case "eston_rig|ability_reflector_channel":
                localAnimator.Play("Grounded.Reflector.eston_reflector_channel");
                break;
            case "eston_rig|ability_reflector_effect":
                localAnimator.Play("Grounded.Reflector.eston_reflector_effect");
                break;
            case "eston_rig|backflip":
                localAnimator.Play("Grounded.Airborne.backflip");
                break;
            case "eston_rig|falling":
                localAnimator.Play("Grounded.Airborne.falling");
                break;
            case "eston_rig|landing":
                localAnimator.Play("Grounded.Airborne.falling");
                break;      
            case "eston_rig|idle":
                localAnimator.Play("Grounded.Idle");
                break;                         
            case "eston_rig|combat_0_rightStraightPunch":
                localAnimator.Play("Grounded.combo1_0_straightPunch");
                break;
            case "eston_rig|combat_1_leftHook.002_legLiftExperiment.001":
                localAnimator.Play("Grounded.combo1_1_leftHook");
                break;
            case "eston_rig|combat_2_jumpingMeiaLua.001":
                localAnimator.Play("Grounded.combo1_2_meialouaKick");
                break;
            case "eston_rig|WallRunLeft":
                localAnimator.Play("Grounded.wallRun");
                break;
            case "eston_rig|WallRunRight":
                localAnimator.Play("Grounded.wallRun");
                break;
            case "eston_rig|sideDodgeLeft":
                localAnimator.Play("Grounded.sideDodgeLeft");
                break;
            case "eston_rig|Run":
                localAnimator.Play("Grounded.freeRun");
                break;
            case "eston_rig|combat_shieldThrow":
                localAnimator.Play("Grounded.shieldThrow");
                break;
            case "eston_rig|shieldSpinningKick":
                localAnimator.Play("Grounded.kickThrownSpecialAttack");
                break;
            case "eston_rig|combat_shieldGaurd":
                localAnimator.Play("Grounded.eston_reflector_cast");
                break;
            case "eston_rig|combat_shieldBash":
                localAnimator.Play("Grounded.eston_reflector_cast");
                break;
            case "eston_rig|combat_parry":
                localAnimator.Play("Grounded.Airborne.landing");
                break;
            default:
                break;
        }
    }
}
