using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using UnityEngine;

public class SpecialAttackInputWindowController : MonoBehaviour
{
    public GameObject specialAttackInputWindow;
    public GameObject selector;
    public enum SelectedButton {North, South, East, West};
    public SelectedButton selectedButton;

    private bool southButtonInput = false;
    private bool northButtonInput = false;
    private bool eastButtonInput = false;
    private bool westButtonInput = false;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(northButtonInput)
        {
            selectedButton = SelectedButton.North;
        }
        
        if(southButtonInput)
        {
            selectedButton = SelectedButton.South;
        }

        if(eastButtonInput)
        {
            selectedButton = SelectedButton.East;
        }

        if(westButtonInput)
        {
            selectedButton = SelectedButton.West;
        }
    }

    public void OnSouthButtonPressed(InputAction.CallbackContext ctx) => southButtonInput = ctx.ReadValueAsButton();
    public void OnNorthButtonPressed(InputAction.CallbackContext ctx) => northButtonInput = ctx.ReadValueAsButton();
    public void OnWestButtonPressed(InputAction.CallbackContext ctx) => westButtonInput = ctx.ReadValueAsButton();
    public void OnEastButtonPressed(InputAction.CallbackContext ctx) => eastButtonInput = ctx.ReadValueAsButton();

    public void SelectSpecialAttack()
    {
        switch (selectedButton)
        {
            case SelectedButton.North:
                Debug.Log("UI: North Button pressed");
                return;
            case SelectedButton.South:
                Debug.Log("UI: South Button pressed");
                return;
            case SelectedButton.East:
                Debug.Log("UI: East Button pressed");
                return;
            case SelectedButton.West:
                Debug.Log("UI: West Button pressed");
                return;
            default:
                return;
        }
    }
}
