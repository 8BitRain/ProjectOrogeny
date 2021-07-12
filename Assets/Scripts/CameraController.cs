using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    private ThirdPersonMovement _playerScriptRef;

    [Header("Target Lock Camera Settings")]
    public CinemachineVirtualCamera dynamicTargetLockCam;
    public bool adjustCameraDistance; 
    public float maxCamDistance = 10;
    public float minCamDistance = 3;
    public float maxTargetDistance = 15;

    [Header("Combat Camera(s) Settings")]
    public CinemachineVirtualCamera combatCamKick;
    public CinemachineSmoothPath combatCamKickDolly;




    // Start is called before the first frame update
    void Start()
    {
        _playerScriptRef = Player.GetComponent<ThirdPersonMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDynamicTargetLockCam();
    }

    void UpdateDynamicTargetLockCam()
    {
        if(dynamicTargetLockCam.m_Priority == 12)
        { 
            if(adjustCameraDistance)
            {
                UpdateDynamicTargetLockCamDistance();
            }
       }
    
    }

    void UpdateDynamicTargetLockCamDistance()
    {
        //Equation. Player position is t. goes from 10 (maximum distance) to 3 minimum distance  
        float distance = _playerScriptRef.GetDistanceToTarget();
        print("DynamicTargetLockCam: targetDistance is: " + distance);

        if(distance > maxTargetDistance)
        {
           distance = maxTargetDistance;
        }

        /*if(distance <= 8)
        {
            distance = 0;
        }*/

        print("DynamicTargetLockCam: distance/maxTargetDistance is: " + distance/maxTargetDistance);
        float camDistance = Mathf.Lerp(minCamDistance, maxCamDistance, distance/maxTargetDistance);
        print("CAMDISTANCE_LERPVALUE: is: " + camDistance);

        //Get Player distance to target
        dynamicTargetLockCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = camDistance;
    }

    public void EnableCinematicKickCam()
    {
        _playerScriptRef.freeLookCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;
        if(_playerScriptRef.GetCurrentTarget() != null)
        {
            dynamicTargetLockCam.m_Priority = 10;
        }

        combatCamKick.m_Priority = 12;

        //Adjust Dolly. 
        //First Waypoint position should be to the right of the player
        Vector3 playerPos = Player.transform.position;
        Vector3 playerLegPos = combatCamKick.m_LookAt.position;
        combatCamKickDolly.m_Waypoints[0].position = new Vector3(playerLegPos.x + 3, playerLegPos.y, playerLegPos.z);
        combatCamKickDolly.m_Waypoints[1].position = new Vector3(playerLegPos.x + 3 + 3, playerLegPos.y, playerLegPos.z + 5);


    }

    public void resetCams()
    {
        print("Reset Cams");
        combatCamKick.m_Priority = 10;
        /*if(_playerScriptRef.GetCurrentTarget() != null)
        {
            dynamicTargetLockCam.m_Priority = 12;
            _playerScriptRef.freeLookCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;
        } 
        else*/
        //{
            dynamicTargetLockCam.m_Priority = 10;
            _playerScriptRef.freeLookCamera.GetComponent<CinemachineFreeLook>().m_Priority = 12;
        //}
    }
}
