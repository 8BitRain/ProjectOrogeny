using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public Camera SceneCamera;
    public Canvas canvas;

    public ScreenManager ScreenManager;

    public Image[] modals;
    private Image currentModal;

    public Image startGameModal;

    private bool _tutorialLoopActive = false;
    private bool _tutorialInitiated = false;


    /// <summary>Vector2 action for pressing south face button to start game </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference StartGamePressed;

    /// <summary>Vector2 action for moving thumbstick </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference LeftThumbStick;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(StartGamePressed.action.triggered && !_tutorialInitiated)
        {
            Debug.Log("Started");
            //StartGamePressed.action.
            StartGame();
        }

        if(_tutorialLoopActive)
        {
            TutorialLoop();
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
        //Destroy(startGameModal.gameObject);

        if(ScreenManager != null)
        {
            ScreenManager.OpenPanel(modals[0].GetComponent<Animator>());
            _tutorialInitiated = true;

            StartCoroutine(InputWindowCoroutine(1));
        }
    
    }

    public void TutorialLoop()
    {
        if(this.ScreenManager.DoesCurrentModalExists())
        {
            switch (this.ScreenManager.GetCurrent())
            {
                case "Movement Modal":
                    if(LeftThumbStick.action.ReadValue<Vector2>().x > 0 || LeftThumbStick.action.ReadValue<Vector2>().y > 0)
                    {
                        this.ScreenManager.CloseCurrent();
                        Debug.Log("Close Active Modal");
                        _tutorialLoopActive = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void ActivateTutorialLoop()
    {
        _tutorialLoopActive = true;
    }

    IEnumerator InputWindowCoroutine (float time) 
    {
        float elapsedTime = 0;

        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ActivateTutorialLoop();
    }

}
