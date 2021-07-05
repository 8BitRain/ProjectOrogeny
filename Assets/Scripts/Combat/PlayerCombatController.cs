using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{

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
            UpdateCombatState();

            //Turn InputWindow Off (This is to prevent (square,square,square spam))
            InputWindowOpen = false;

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
            UpdateCombatState();

            //Turn InputWindow Off (This is to prevent (square,square,square spam))
            InputWindowOpen = false;

            //Start timer to open input window back up
            StartCoroutine(InputWindowCoroutine(.5f));
        }
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

    public void UpdateCombatState()
    {
        if(combatType == CombatType.Light)
        {
            switch (this.combatState)
            {
                case 0:
                    this.combatState = 1;
                    break;
                case 1:
                    this.combatState = 2;
                    break;
                case 2: 
                    this.combatState = 0;
                    break;
                default:
                    break;
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
    
}
