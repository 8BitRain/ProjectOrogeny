using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Player Combat Configuration")]
    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for Enviroment Interaction ")]
    public InputActionReference lightMeleeInput;
    public GameObject Player;

    //Player References
    private ThirdPersonMovement playerScriptReference;
    private Animator playerAnimator;

    //Combat State References
    private int combatState = 0;
    private enum CombatType{Light, Heavy};
    private CombatType combatType;

    private bool InputWindowOpen = true;
    [Header("Light Attack Melee Combo Animation Strings")]
    public string[] lightMeleeComboStrings;

    //[Header("Light Attack Melee Combo Animation Strings")]
    //public string[] lightMeleeComboStrings;

    [Header("Camera Controller Reference")]
    public CameraController cameraController;
    // Start is called before the first frame update
    void Start()
    {
        if(Player != null)
        {
            playerScriptReference = Player.GetComponent<ThirdPersonMovement>();
            playerAnimator = Player.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lightMeleeInput.action.triggered && !playerScriptReference.GetSpecialAttackInputWindowActive() && InputWindowOpen && !playerAnimator.GetBool("Sprinting"))
        {
            Debug.Log("LightMeleeCombo");
            playerAnimator.SetTrigger("lightMeleeCombo");

            //Update Combat State
            combatType = CombatType.Light;
            //UpdateCombatState();

            //Turn InputWindow Off (This is to prevent (square,square,square spam))
            InputWindowOpen = false;

            //Turn the player toward the target
            if(playerScriptReference.GetCurrentTarget() != null)
            {
                playerScriptReference.FaceTarget();
            }

            //Start timer to open input window back up
            StartCoroutine(InputWindowCoroutine(.5f));

        }

        if(lightMeleeInput.action.triggered && !playerScriptReference.GetSpecialAttackInputWindowActive() && InputWindowOpen && playerAnimator.GetBool("Sprinting"))
        {
            Debug.Log("Sprinting Light Attack");
            playerAnimator.SetTrigger("sprintingLightAttack");
            //playerAnimator.SetBool("Sprinting", false);
            playerAnimator.SetBool("PerformingSprintingLightAttack", true);

            //Update Combat State
            combatType = CombatType.Light;
            //UpdateCombatState();

            //Turn InputWindow Off (This is to prevent (square,square,square spam))
            InputWindowOpen = false;


            //Start timer to open input window back up
            StartCoroutine(InputWindowCoroutine(.5f));
        }

        UpdateCombatState();
    }

    IEnumerator InputWindowCoroutine (float time) 
    {
        float elapsedTime = 0;

        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        InputWindowOpen = true;
    }

    //Updates Automatically. For now this should update based on events from the animation
    public void UpdateCombatState()
    {
        if(combatType == CombatType.Light)
        {
            /*switch (this.combatState)
            {
                case 0:
                    this.combatState = 1;
                    break;
                case 1:
                    this.combatState = 2;
                    break;
                case 2: 
                    cameraController.EnableCinematicKickCam();
                    this.combatState = 0;
                    break;
                default:
                    break;
            }*/
            if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("combo1"))
            {
                GetCurrentAnimationStateName();
            }
            else if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("combo2"))
            {
                GetCurrentAnimationStateName();
            }
            else if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("combo3"))
            {
                GetCurrentAnimationStateName();
                cameraController.EnableCinematicKickCam();
            }
            else
            {
                //This script will be handled by an animation event
                //cameraController.ResetCinematicCombatCams();
            }
        }

        UpdatePlayerPosition();
    }

    public void UpdatePlayerPosition()
    {

    }

    public void PerformSprintingAttack()
    {
        playerAnimator.SetBool("PerformingSprintingLightAttack", true);
    }

    //This animation event is triggered by eston_rig|spearLunge2
    public void AnimationEventEndSprintingAttack()
    {
        playerAnimator.SetBool("PerformingSprintingLightAttack", false);
    }

    public string[] GetLightMeleeComboStrings()
    {
        return this.lightMeleeComboStrings;
    }
    //Get's the current animation name from the animator
    public string GetCurrentAnimationStateName()
    {
        Debug.Log("Current Animation State Name: " + this.playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        /*for(int i = 0; i < GetLightMeleeComboStrings().Length; i++)
        {
            if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("combo1"))
            {
                combatState = i;
            }
        }*/
        return this.playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }
    
}
