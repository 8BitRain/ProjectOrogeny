using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineScreenShake : MonoBehaviour
{
    public static CinemachineScreenShake Instance {get; private set;}
    public CinemachineFreeLook cinemachineCam;
    private float shakeTimer;

    private void Awake()
    {
        Instance = this;
        //cinemachineCam = this.gameObject.GetComponent<CinemachineFreeLook>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<CinemachineFreeLook>().GetRig(1).GetC
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                cinemachineCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }

    public void screenShake(float intensity, float timer)
    {
        Debug.Log("ScreenShake: Shake the screeeen");
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    cinemachineCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        shakeTimer = timer;

    }
}
