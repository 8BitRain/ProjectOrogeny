using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using UnityEngine;

public class SpecialAttackInputWindowController : MonoBehaviour
{
    public Transform Agent;
    public GameObject selector;
    public enum SelectedButton {North, South, East, West};
    public SelectedButton selectedButton;

    public SpecialAbility specialAbilitySouth;
    public SpecialAbility specialAbilityNorth;
    public SpecialAbility specialAbilityEast;
    public SpecialAbility specialAbilityWest;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference SouthButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for North Button ")]
    public InputActionReference NorthButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for West Button ")]
    public InputActionReference WestButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for East Button ")]
    public InputActionReference EastButtonPressed;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(NorthButtonPressed.action.triggered)
        {
            selectedButton = SelectedButton.North;
            Debug.Log("UI: North Button pressed");
            SelectSpecialAttack();
        }
        
        if(SouthButtonPressed.action.triggered)
        {
            selectedButton = SelectedButton.South;
            Debug.Log("UI: South Button pressed");
            SelectSpecialAttack();
        }

        if(EastButtonPressed.action.triggered)
        {
            selectedButton = SelectedButton.East;
            Debug.Log("UI: East Button pressed");
            SelectSpecialAttack();
        }

        if(WestButtonPressed.action.triggered)
        {
            selectedButton = SelectedButton.West;
            Debug.Log("UI: West Button pressed");
            SelectSpecialAttack();
        }

        
    }

    public void SelectSpecialAttack()
    {
        switch (selectedButton)
        {
            case SelectedButton.North:
                SpecialAbility abilityNorth = Instantiate(specialAbilityNorth);
                abilityNorth.CreateAbility(Agent, Agent, Agent.GetComponent<Animator>());
                return;
            case SelectedButton.South:
                SpecialAbility abilitySouth = Instantiate(specialAbilitySouth);
                abilitySouth.CreateAbility(Agent, Agent, Agent.GetComponent<Animator>());
                return;
            case SelectedButton.East:
                SpecialAbility abilityEast = Instantiate(specialAbilityEast);
                abilityEast.CreateAbility(Agent, Agent, Agent.GetComponent<Animator>());
                return;
            case SelectedButton.West:
                SpecialAbility abilityWest = Instantiate(specialAbilityWest);
                abilityWest.CreateAbility(Agent, Agent, Agent.GetComponent<Animator>());
                return;
            default:
                return;
        }
    }

    public void SetAgentProperties(Transform agent)
    {
        this.Agent = agent;
    }
}
