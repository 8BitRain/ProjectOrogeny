using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameManager : MonoBehaviour
{
    public Transform[] spawnedPlayers;
    // Start is called before the first frame update
    void Start()
    {
        spawnedPlayers = new Transform[2];
    }

    // Update is called once per frame
    void Update()
    {
        //https://github.com/Unity-Technologies/upm-package-cinemachine/blob/d15a253e9e729594fc94ee672e7e8a5532cb9f79/Runtime/Helpers/CinemachineInputProvider.cs
        Debug.Log("Connected Players: " + InputUser.all.Count);
        if(InputUser.all.Count == 1 && spawnedPlayers[0] == null)
        {
            Debug.Log("Player 1 Value " + InputUser.all[0].index);
            spawnedPlayers[0] = GameObject.FindGameObjectWithTag("Player").transform;
            spawnedPlayers[0].tag = "P1";
            spawnedPlayers[0].GetComponentInChildren<CinemachineInputProvider>().PlayerIndex = InputUser.all[0].index;

            GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
            virtualCamera.tag = "P1VirtualCamera";
            virtualCamera.layer = LayerMask.NameToLayer("P1Cam");


            //https://docs.unity3d.com/Manual/Layers.html
            /*int layerMask = 1 << 16;
            layerMask = ~layerMask;

            spawnedPlayers[0].GetComponent<ThirdPersonMovement>().cam.GetComponent<Camera>().cullingMask = layerMask;*/
        }
        if(InputUser.all.Count == 2 && spawnedPlayers[1] == null)
        {
            Debug.Log("Player 2 Value " + InputUser.all[1].index);
            spawnedPlayers[1] = GameObject.FindGameObjectWithTag("Player").transform;
            spawnedPlayers[1].tag = "P2";
            spawnedPlayers[1].GetComponentInChildren<CinemachineInputProvider>().PlayerIndex = InputUser.all[1].index;

            GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
            virtualCamera.tag = "P2VirtualCamera";
            virtualCamera.layer = LayerMask.NameToLayer("P2Cam");

            //int layerMask = 1 << 15;
            //layerMask = ~layerMask;

            Camera cam = spawnedPlayers[1].GetComponentInChildren<Camera>();
            //Show
            //cam.cullingMask |= 1 << LayerMask.NameToLayer("P2Cam");
            //Hide
            Debug.Log("Camera referenced" + cam.name);
            cam.cullingMask = -1;
            cam.cullingMask &=  ~(1 << LayerMask.NameToLayer("P1Cam"));


            
        }
        InputSystem.onDeviceChange +=
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
        };
    }
}
