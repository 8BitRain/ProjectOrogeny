using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public Camera SceneCamera;
    public Canvas canvas;

    public Image startGameModal;


    /// <summary>Vector2 action for pressing south face button to start game </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference StartGamePressed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(StartGamePressed.action.triggered)
        {
            Debug.Log("Started");
            StartGame();
        }
        
    }

    public void StartGame()
    {
        SceneCamera.gameObject.SetActive(false);
        StartTutorial();
    }

    public void StartTutorial()
    {
        Debug.Log("Tutorial Started");
        Destroy(startGameModal.gameObject);
    }

}
