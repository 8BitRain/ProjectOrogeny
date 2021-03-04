using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatVFXManager : MonoBehaviour
{
    public GameObject specialAttackVFX;
    public Transform specialAttackVFXSpawn;

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
    }

    public void triggerSpecialVFX()
    {
        if(specialAttackVFXSpawn !=  null)
        {
            instancedSpecialVFX = Instantiate(specialAttackVFX, specialAttackVFXSpawn.position, specialAttackVFXSpawn.rotation) as GameObject;
            instancedSpecialVFX.transform.rotation = Quaternion.Euler(90,0,0);
        }
    }

    public void SetCombatVFXSpawn(Transform vfxSpawn)
    {
        this.specialAttackVFXSpawn = vfxSpawn;
    }
}