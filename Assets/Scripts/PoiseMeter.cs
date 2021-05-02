using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoiseMeter : MonoBehaviour
{

    public Slider slider;

    private GameManager gameManager;
    private Camera[] playerCameras;

    // Start is called before the first frame update
    public void SetPoise(float poise)
    {
        slider.value = poise;
    }

    public void SetMaxPoise(float poise)
    {
        slider.maxValue = poise;
        slider.value = poise;
    }

    public float GetPoise()
    {
        return slider.value;
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
