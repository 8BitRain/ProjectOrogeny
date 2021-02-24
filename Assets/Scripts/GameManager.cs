using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public Transform[] spawnedPlayers;

    [Header("UI Settings")]
    public GameObject canvas;
    public bool canDisplayWidescreenUI;

    [Header("Network Settings")]
    public bool enableNetworkedLoadedPlayer;
    // Start is called before the first frame update
    void Start()
    {
        spawnedPlayers = new Transform[2];
    }

    // Update is called once per frame
    void Update()
    {
        //Need Logic here that also gates the server has loaded
        if(enableNetworkedLoadedPlayer)
        {
            //Debug.Log("Player 1 Value " + InputUser.all[0].index);
            spawnedPlayers[0] = GameObject.FindGameObjectWithTag("Player").transform;
            spawnedPlayers[0].tag = "P1";
            spawnedPlayers[0].GetComponentInChildren<CinemachineInputProvider>().PlayerIndex = 0;

            GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
            virtualCamera.tag = "P1VirtualCamera";
            virtualCamera.layer = LayerMask.NameToLayer("P1Cam");



            spawnedPlayers[0].gameObject.SetActive(true);
        }

        //https://github.com/Unity-Technologies/upm-package-cinemachine/blob/d15a253e9e729594fc94ee672e7e8a5532cb9f79/Runtime/Helpers/CinemachineInputProvider.cs
        //Debug.Log("Connected Players: " + InputUser.all.Count);
        if(!enableNetworkedLoadedPlayer)
        {
            //Loading Player 1
            if(InputUser.all.Count == 1 && spawnedPlayers[0] == null)
            {
                //Debug.Log("Player 1 Value " + InputUser.all[0].index);
                spawnedPlayers[0] = GameObject.FindGameObjectWithTag("Player").transform;
                spawnedPlayers[0].tag = "P1";   
                //TODO change this function to grab the correct character name.
                //Mesko, Eston, Jaco
                spawnedPlayers[0].Find("Third Person Character").Find("Eston").tag = "P1";
                spawnedPlayers[0].GetComponentInChildren<CinemachineInputProvider>().PlayerIndex = InputUser.all[0].index;

                GameObject[] virtualCameras = GameObject.FindGameObjectsWithTag("VirtualCamera");

                foreach (GameObject virtualCamera in virtualCameras)
                {
                    virtualCamera.tag = "P1VirtualCamera";
                    virtualCamera.layer = LayerMask.NameToLayer("P1Cam");
                }

                spawnedPlayers[0].name = "P1";
                spawnedPlayers[0].gameObject.SetActive(true);

                //Setup Player 1 UI
                setupUI(1);
            }
            //Loading Player 2
            if(InputUser.all.Count == 2 && spawnedPlayers[1] == null)
            {
                //Debug.Log("Player 2 Value " + InputUser.all[1].index);
                spawnedPlayers[1] = GameObject.FindGameObjectWithTag("Player").transform;
                spawnedPlayers[1].tag = "P2";
                spawnedPlayers[1].Find("Third Person Character").Find("animation_lab_nyx").tag = "P2";
                spawnedPlayers[1].GetComponentInChildren<CinemachineInputProvider>().PlayerIndex = InputUser.all[1].index;

                GameObject[] virtualCameras = GameObject.FindGameObjectsWithTag("VirtualCamera");
                foreach (GameObject virtualCamera in virtualCameras)
                {
                    virtualCamera.tag = "P2VirtualCamera";
                    virtualCamera.layer = LayerMask.NameToLayer("P2Cam");
                }

                Camera cam = spawnedPlayers[1].GetComponentInChildren<Camera>();
                Debug.Log("Camera referenced" + cam.name);

                cam.cullingMask = -1;
                cam.cullingMask &=  ~(1 << LayerMask.NameToLayer("P1Cam"));

                spawnedPlayers[1].name = "P2";
                spawnedPlayers[1].gameObject.SetActive(true);

                //Setting Player 2 UI
                setupUI(2);

            }
            //Input System Device Logging
            /*InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        // New Device.
                        Debug.Log("New Device Added" + device);
                        break;
                    case InputDeviceChange.Disconnected:
                        // Device got unplugged.
                        Debug.Log("Device Disconnected" + device);
                        break;
                    case InputDeviceChange.Removed:
                        // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                        Debug.Log("Device Removed from Input System" + device);
                        break;
                    default:
                        // See InputDeviceChange reference for other event types.
                        break;
                }
            };*/        
        }

    }

    public void setupUI(int playerNum)
    {
        if(playerNum == 1)
        {
            print("Setting up Player 1 UI");
            HealthBar p1HealthBar = spawnedPlayers[playerNum - 1].GetComponentInChildren<HealthBar>();
            RectTransform p1HealthBarRT= p1HealthBar.gameObject.GetComponent<RectTransform>();
            //Center of screen + plus half the width of the healthbar + an offset of 10
            //https://stackoverflow.com/questions/44471568/how-to-calculate-sizedelta-in-recttransform
            p1HealthBarRT.anchoredPosition = new Vector2(-Screen.width/2 + p1HealthBarRT.sizeDelta.x/2 + 10.0f , Screen.height/2 - p1HealthBarRT.sizeDelta.y/2 - 10.0f);
            //p2HealthBarRT.position.Set(Screen.width/2 + p2HealthBarRT.width/2 + 10.0f, p2HealthBarRT.position.y); 
        }
        if(playerNum == 2)
        {
            print("Setting up Player 2 UI");
            HealthBar p2HealthBar = spawnedPlayers[playerNum - 1].GetComponentInChildren<HealthBar>();
            RectTransform p2HealthBarRT= p2HealthBar.gameObject.GetComponent<RectTransform>();
            //Center of screen + plus half the width of the healthbar + an offset of 10
            //https://stackoverflow.com/questions/44471568/how-to-calculate-sizedelta-in-recttransform
            p2HealthBarRT.anchoredPosition = new Vector2(p2HealthBarRT.sizeDelta.x/2 + 10.0f, Screen.height/2 - p2HealthBarRT.sizeDelta.y/2 - 10.0f);
            //p2HealthBarRT.position.Set(Screen.width/2 + p2HealthBarRT.width/2 + 10.0f, p2HealthBarRT.position.y);
        }
    }

    public void enableWidescreenBars(int playerNum)
    {
        if(playerNum == 1)
        {
            canvas.GetComponent<UI>().widescreenUI.gameObject.SetActive(true);
        }
    }
    public void disableWidescreenBars(int playerNum)
    {
        if(playerNum == 1)
        {
            canvas.GetComponent<UI>().widescreenUI.gameObject.SetActive(false);
        }
    }

    public void updateTargetPosition(int playerNum, GameObject playerTarget)
    {
        if(playerNum == 1)
        {
            Camera p1Cam = spawnedPlayers[0].Find("Cam").GetComponent<Camera>();
            canvas.GetComponent<UI>().target.position = p1Cam.WorldToScreenPoint(playerTarget.transform.position + new Vector3(0,2,0));
        }
    }

    public void enableTargetArrow(int playerNum)
    {
        if(playerNum == 1)
        {
            canvas.GetComponent<UI>().target.gameObject.SetActive(true);
        }
    }

    public void disableTargetArrow(int playerNum)
    {
        if(playerNum == 1)
        {
            canvas.GetComponent<UI>().target.gameObject.SetActive(false);
        }
    }
}
