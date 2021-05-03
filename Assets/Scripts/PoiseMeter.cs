using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoiseMeter : MonoBehaviour
{

    public Slider slider;

    private GameManager gameManager;
    private Camera[] playerCameras;

    private float refillPoiseTimer = 3.0f;
    private float _poiseCooldownTime = 3.0f;
    private bool startRefillPoiseTimer = false;
    private bool refillGauge = false;

    void Update()
    {
        if(startRefillPoiseTimer)
        {
            UpdatePoiseTimer();
        }

        if(refillGauge)
        {
            RefillPoiseMeter();
        }
    }

    // Start is called before the first frame update
    public void SetPoise(float poise)
    {
        if(poise < GetMaxPoise())
        {
            this.startRefillPoiseTimer = true;
            refillPoiseTimer = _poiseCooldownTime;
        }
        this.refillGauge = false;
        slider.value = poise;
    }

    public void SetMaxPoise(float poise)
    {
        slider.maxValue = poise;
        slider.value = poise;
    }

    public float GetMaxPoise()
    {
        return slider.maxValue;
    }

    public float GetPoise()
    {
        return slider.value;
    }

    public void RefillPoiseMeter()
    {
        if(GetPoise() < GetMaxPoise())
        {
            //SetPoise(GetPoise() + 5/60);
            Debug.Log("UI: Refilling Poise Meter: " + GetPoise() + 5.0f/60 + "/" + GetMaxPoise());
            slider.value = GetPoise() + (5.0f/60);
        }

        if(GetPoise() >= GetMaxPoise())
        {
            SetPoise(GetMaxPoise());
        }
    }

    public void SetPoiseCooldownTime(float time)
    {
        this._poiseCooldownTime = time;
    }

    void UpdatePoiseTimer()
    {
        print("UI: Updating Pose Timer: " + refillPoiseTimer);
        if(startRefillPoiseTimer)
        {
            refillPoiseTimer -= Time.deltaTime;
        }

        if(refillPoiseTimer <= 0)
        {
            refillPoiseTimer = _poiseCooldownTime;
            refillGauge = true;
            startRefillPoiseTimer = false;
        }

    }

    //Update enemy poise bar
    public void PositionEnemyPoiseMeter(Camera playerCam, Transform enemy, Transform player)
    {
        RectTransform poiseMeterRectTransform = this.GetComponent<RectTransform>();
        poiseMeterRectTransform.position = playerCam.WorldToScreenPoint(new Vector3(enemy.position.x,enemy.GetComponent<Bladeclubber>().Head.position.y,enemy.position.z) + new Vector3(0,2.9f,0));
        //poiseMeterRectTransform.position = playerCam.WorldToScreenPoint(enemy.transform.position + new Vector3(0,8,0));
        //poiseMeterRectTransform.position = playerCam.WorldToScreenPoint(enemy.transform.position);

        Renderer enemy_renderer = enemy.GetComponent<Bladeclubber>().Body.GetComponent<Renderer>();
        if(enemy_renderer.isVisible)
        {
            Canvas poiseMeterCanvas = this.GetComponentInParent<Canvas>();
            poiseMeterCanvas.enabled = true;

        } else
        {
            Canvas poiseMeterCanvas = this.GetComponentInParent<Canvas>();
            poiseMeterCanvas.enabled = false;

        }


        //transform.LookAt()
        //transform.LookAt(player.position);
        //transform.rotation = Quaternion.Euler(player.rotation.x, 90, player.rotation.z);
    }
}
