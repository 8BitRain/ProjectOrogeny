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

    private Image currentModal;

    [Header("Modals")]
    public Image startGameModal;
    public Image[] tutorialModals;

    [Header("Waypoints")]
    public GameObject[] waypoints;

    [Header("Quests")]
    public GameObject[] quests;




    [Header("Enviroment Objects")]
    public GameObject deadPlanet;
    
    private bool _tutorialLoopActive = false;
    private bool _tutorialInitiated = false;

    [Header("Controller Inputs")]
    /// <summary>Vector2 action for pressing south face button to start game </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference StartGamePressed;

    /// <summary>Vector2 action for moving left thumbstick </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference LeftThumbStick;

    /// <summary>Vector2 action for moving right thumbstick </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference RightThumbStick;


    
    // Start is called before the first frame update
    void Start()
    {
        //Assign all tutorial waypoints this tutorial Controller reference
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].GetComponent<Waypoint>().SetTutorialController(this.GetComponent<TutorialController>());
        }
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
        deadPlanet.gameObject.transform.localScale = new Vector3(100000,100000,100000);
        SceneCamera.gameObject.SetActive(false);
        StartTutorial();
    }

    public void StartTutorial()
    {
        Debug.Log("Tutorial Started");
        //Destroy(startGameModal.gameObject);

        if(ScreenManager != null)
        {
            ScreenManager.OpenPanel(tutorialModals[0].GetComponent<Animator>());
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

                        //Start Quest 0
                        //quests[0].GetComponent<Quest>().SetQuestStarted();
                        //_tutorialLoopActive = false;
                    }
                    
                    break;
                case "Look Around Modal":
                    if(RightThumbStick.action.ReadValue<Vector2>().x > 0 || RightThumbStick.action.ReadValue<Vector2>().y > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 0
                        quests[0].GetComponent<Quest>().SetQuestStarted();
                        //_tutorialLoopActive = false;
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

    public void DeactivateTutorialLoop()
    {
        _tutorialLoopActive = false;
    }

    public void PlayerReachedWaypoint(int id)
    {
        switch (id)
        {
            case 0:
                waypoints[0].SetActive(false);
                quests[0].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                ScreenManager.OpenPanel(tutorialModals[1].GetComponent<Animator>());
                break;
            default:
                break;
        }
    }

    //This delay prevents input from persisting multiple frames
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
