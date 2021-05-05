using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform Wielder;
    public Transform weaponTrail;
    public LayerMask Target;
    public float force = 40;

    private int hitCounter = 0;
    private int hitCounter1 = 0;
    private int hitCounter2 = 0;

    public GameObject combatVFXManager;
    // Start is called before the first frame update
    void Start()
    {
        if(weaponTrail == null)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.matrix = transform.localToWorldMatrix;
        if(this.GetComponent<BoxCollider>().enabled)
        {
            Gizmos.DrawWireCube(this.GetComponent<BoxCollider>().center, this.GetComponent<BoxCollider>().size);
        }
        
        //Gizmos.DrawWireCube(this.transform.position, new Vector3(2,2,2));
    }

    public void EnableWeaponTrail()
    {
        weaponTrail.gameObject.SetActive(true);
    }

    public void DisableWeaponTrail()
    {
        weaponTrail.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (gameObject.tag)
        {
            
            default:
                break;
        }
        //https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
        if(Target == (Target | (1 << other.gameObject.layer)))
        {
            print("Collided with " + other.gameObject.name + "Layer: " + other.gameObject.layer);
            print(Target);

            ThirdPersonMovement playerCharacter = other.transform.GetComponent<ThirdPersonMovement>();
            //TODO: Make this generic. Currently this "Weapon" is setup for a bladeclubber's animations
            if(Wielder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("AttackStringI") && hitCounter1 < 1)
            {
                //playerCharacter.AddImpact(transform.forward, 1.0f);
                if(!playerCharacter.GetIframe())
                {
                    playerCharacter.AddImpact(Vector3.zero, 0);
                    print("Knockback 1");
                    hitCounter1++;
                    print("Hit Counter Attack String I: " + hitCounter1);

                    playerCharacter.healthBar.SetHealth(playerCharacter.healthBar.GetHealth() - 2.5f);
                    SpawnCombatVFX(this.transform);
                }
            }
            else if(Wielder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("AttackStringII") && hitCounter2 < 1)
            {
                if(!playerCharacter.GetIframe())
                {
                    playerCharacter.AddImpact(Vector3.up, force);    
                    print("Knockback 10");
                    hitCounter2++;
                    print("Hit Counter Attack String II: " + hitCounter2);

                    playerCharacter.healthBar.SetHealth(playerCharacter.healthBar.GetHealth() - 2.5f);
                    SpawnCombatVFX(this.transform);
                }
            }
            /*else
            {
                playerCharacter.AddImpact(transform.forward, force);
            }*/
            


        }
    }

    void SpawnCombatVFX(Transform weaponTransform)
    {
        GameObject combatVFXManagerInstance = Instantiate(combatVFXManager, weaponTransform.position, weaponTransform.rotation) as GameObject;
        combatVFXManagerInstance.GetComponent<CombatVFXManager>().SetCombatVFXSpawn(weaponTransform);
        combatVFXManagerInstance.GetComponent<CombatVFXManager>().triggerSpecialVFX("vfx_hit_connected");
    }

    public void ResetHitCounters()
    {
        hitCounter1 = 0;
        hitCounter2 = 0;
    }
}
