using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    private GameManager gameManager;
    private Camera[] playerCameras;

    // Start is called before the first frame update
    public void SetHealth(float health)
    {
        slider.value = health;
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public float GetHealth()
    {
        return slider.value;
    }

    //Update enemy health bar
    public void PositionEnemyHealthBar(Camera playerCam, Transform enemy, Transform player)
    {
        RectTransform healthBarRectTransform = this.GetComponent<RectTransform>();
        healthBarRectTransform.position = playerCam.WorldToScreenPoint(new Vector3(enemy.position.x,enemy.GetComponent<Bladeclubber>().Head.position.y,enemy.position.z) + new Vector3(0,3,0));
        //healthBarRectTransform.position = playerCam.WorldToScreenPoint(enemy.transform.position + new Vector3(0,8,0));
        //healthBarRectTransform.position = playerCam.WorldToScreenPoint(enemy.transform.position);

        Renderer enemy_renderer = enemy.GetComponent<Bladeclubber>().Body.GetComponent<Renderer>();
        if(enemy_renderer.isVisible)
        {
            Canvas healthBarCanvas = this.GetComponentInParent<Canvas>();
            healthBarCanvas.enabled = true;

        } else
        {
            Canvas healthBarCanvas = this.GetComponentInParent<Canvas>();
            healthBarCanvas.enabled = false;

        }


        //transform.LookAt()
        //transform.LookAt(player.position);
        //transform.rotation = Quaternion.Euler(player.rotation.x, 90, player.rotation.z);
    }
}
