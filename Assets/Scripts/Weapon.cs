using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform Wielder;
    public Transform weaponTrail;
    public LayerMask Player;
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
        //https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
        if(Player == (Player | (1 << other.gameObject.layer)))
        {
            print("Collided with " + other.gameObject.name + "Layer: " + other.gameObject.layer);
            print(Player);

            ThirdPersonMovement playerCharacter = other.transform.GetComponent<ThirdPersonMovement>();
            //TODO: Make this generic. Currently this "Weapon" is setup for a bladeclubber's animations
            if(Wielder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("AttackStringI"))
            {
                //playerCharacter.AddImpact(transform.forward, 1.0f);
                print("Knockback 1");
                hitCounter1++;
                print("Hit Counter Attack String I: " + hitCounter1);
            }
            else if(Wielder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("AttackStringII"))
            {
                playerCharacter.AddImpact(Wielder.transform.forward, force);    
                print("Knockback 10");
                hitCounter2++;
                print("Hit Counter Attack String II: " + hitCounter2);

            }
            /*else
            {
                playerCharacter.AddImpact(transform.forward, force);
            }*/
            
            playerCharacter.healthBar.SetHealth(playerCharacter.healthBar.GetHealth() - 2.5f);


            SpawnCombatVFX(this.transform);

        }
    }

    void SpawnCombatVFX(Transform weaponTransform)
    {
        GameObject combatVFXManagerInstance = Instantiate(combatVFXManager, weaponTransform.position, weaponTransform.rotation) as GameObject;
        combatVFXManagerInstance.GetComponent<CombatVFXManager>().SetCombatVFXSpawn(weaponTransform);
        combatVFXManagerInstance.GetComponent<CombatVFXManager>().triggerSpecialVFX();
    }
}
