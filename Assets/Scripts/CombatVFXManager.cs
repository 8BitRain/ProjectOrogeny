using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;


public class CombatVFXManager : MonoBehaviour
{
    public GameObject specialAttackVFX;
    public Transform specialAttackVFXSpawn;
    public GameObject postProcessingEFX;
    public float postProcessingEFXDuration;

    private float postProcessingTime = 0;
    private float efxTimer = 0;
    public bool enablePostProcessingEFX = true;
    public float postProcessingSpeed = .5f;

    private GameObject instancedPostProcessingEFX;
    private GameObject instancedSpecialVFX;

    void Start()
    {

    }

    void Update()
    {
        if(instancedSpecialVFX != null)
        {
            Destroy(instancedSpecialVFX.gameObject, 3.0f);
        }
        Destroy(this.gameObject, 3.0f);

        if(enablePostProcessingEFX)
        {
            if(instancedPostProcessingEFX != null)
            {
                UpdatePostProcessingEFX();
                Destroy(instancedPostProcessingEFX, postProcessingEFXDuration);
            }   
        } 
    }

    public void UpdatePostProcessingEFX()
    {
        Volume efx = instancedPostProcessingEFX.GetComponent<Volume>();
        efxTimer += Time.deltaTime;
        /*if(efxTimer < postProcessingEFXDuration/2.0f)
        {
            postProcessingTime += postProcessingSpeed * Time.deltaTime;
            efx.weight = Mathf.Lerp(0,1, postProcessingTime);
            print("Post Processing Weight: " + efx.weight);
        }*/

        //if(efxTimer > postProcessingEFXDuration/2.0f)
        //{
            postProcessingTime += postProcessingSpeed * Time.deltaTime;
            efx.weight = Mathf.Lerp(1,0, postProcessingTime);
        //}
    }
    public void triggerSpecialVFX()
    {
        if(specialAttackVFXSpawn !=  null)
        {
            instancedSpecialVFX = Instantiate(specialAttackVFX, specialAttackVFXSpawn.position, specialAttackVFXSpawn.rotation) as GameObject;
            //This rotation is made for game objects that aren't already facing the correct direction. Should probably check for correct rotation here.
            if(specialAttackVFX.name == "vfx_EarthernRise")
            {
                instancedSpecialVFX.transform.rotation = Quaternion.Euler(90,0,0);
            }
            

            if(enablePostProcessingEFX)
            {
                instancedPostProcessingEFX = Instantiate(postProcessingEFX, specialAttackVFXSpawn.position, specialAttackVFXSpawn.rotation) as GameObject;
                
            }    
        }
    }

    public void SetCombatVFXSpawn(Transform vfxSpawn)
    {
        this.specialAttackVFXSpawn = vfxSpawn;
    }
}