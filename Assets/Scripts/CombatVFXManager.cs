using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;


public class CombatVFXManager : MonoBehaviour
{
    //public GameObject specialAttackVFX;
    public GameObject[] specialAttackVFX;
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
    public void triggerSpecialVFX(string vfxName)
    {
        print("VFXName: " + vfxName);
        if(specialAttackVFXSpawn != null)
        {
            GameObject specialAttackVFXInstance = null;
            for(int i = 0; i < specialAttackVFX.Length; i++)
            {
                print("Special Attack Name: " + specialAttackVFX[i].name);
                if(specialAttackVFX[i].name == vfxName)
                {
                    specialAttackVFXInstance = specialAttackVFX[i].gameObject;
                    print("Special Attack Assigned");
                }    
            }
            if(specialAttackVFXInstance != null)
            {
                instancedSpecialVFX = Instantiate(specialAttackVFXInstance, specialAttackVFXSpawn.position, specialAttackVFXSpawn.rotation) as GameObject;
                print("instancedSpecialVFX Name: " + instancedSpecialVFX.name);
                //This rotation is made for game objects that aren't already facing the correct direction. Should probably check for correct rotation here.
                if(specialAttackVFXInstance.name == "vfx_EarthernRise")
                {
                    instancedSpecialVFX.transform.rotation = Quaternion.Euler(90,0,0);
                }

                if(specialAttackVFXInstance.name == "vfx_estonCombo_hit_connected")
                {
                    
                    //Spawning at the player's hand plus 2 units in the z direction
                    instancedSpecialVFX.transform.position += transform.forward;
                    //instancedSpecialVFX.transform.rotation = Quaternion.Euler(0,180,0);
                    instancedSpecialVFX.transform.LookAt(transform.position + Camera.main.transform.forward);
                    instancedSpecialVFX.transform.rotation = Quaternion.Euler(0,instancedSpecialVFX.transform.eulerAngles.y,0);
                }

                

                if(enablePostProcessingEFX)
                {
                    instancedPostProcessingEFX = Instantiate(postProcessingEFX, specialAttackVFXSpawn.position, specialAttackVFXSpawn.rotation) as GameObject;
                    
                }    
            }
        }
    }

    public void SetCombatVFXSpawn(Transform vfxSpawn)
    {
        this.specialAttackVFXSpawn = vfxSpawn;
    }
}