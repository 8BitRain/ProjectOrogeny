﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine;

public class CombatAction : MonoBehaviour
{
    public float damage;
    public float impactForce;
    public AnimatorStateInfo animatorStateInfo;
    public GameObject target;
    public GameObject combatVFX;
    public int hitCount;

    private bool initialized = false;

    private float currentHitCount;

    /*public CombatAction(float damage, float impactForce, AnimatorStateInfo animatorStateInfo, GameObject target, GameObject combatVFX, int hitCount)
    {
        this.damage = damage;
        this.impactForce = impactForce;
        this.animatorStateInfo = animatorStateInfo;
        this.target = target;
        this.combatVFX = combatVFX;
        this.hitCount = hitCount;
        this.currentHitCount = 0;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        //currentHitCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.initialized)
        {
            if(currentHitCount < hitCount)
            {
                DealDamage(this.damage, this.target);
                DealPoiseDamage(this.damage, this.target);
                AddImpact(this.impactForce, this.target);
                currentHitCount++;
                return;
            }

            if(currentHitCount == hitCount)
            {
                Debug.Log("Destroy " + this.gameObject.name);
                Destroy(this.gameObject);
            }
        }
    }

    void DealDamage(float damage, GameObject target)
    {
        HealthBar targetHealthReference = target.GetComponentInParent<Bladeclubber>().healthBar;

        /*if(targetHealthReference == null)
        {
            //We may be a child let's look in parents
            targetHealthReference = target.GetComponentInParent<HealthBar>();
        }*/
        targetHealthReference.SetHealth(targetHealthReference.GetHealth() - damage);
        Debug.Log("Combat: " + target.name + " dealt " + damage + " damage");      
    }

    void DealPoiseDamage(float damage, GameObject target)
    {
        PoiseMeter targetPoiseReference = target.GetComponentInParent<Bladeclubber>().poiseMeter;
        targetPoiseReference.SetPoise(targetPoiseReference.GetPoise() - damage);
        Debug.Log("Combat: " + target.name + " dealt " + damage + " poise damage");
    }

    void AddImpact(float force, GameObject target)
    {
        //Experiment hit stun, turning off agent so it can fly?
        //This experiment works! When we disabel the nav mesh, we can send the target flying. However, the "Bladeclubber" automatically resets its navMeshAgent to enabled
        //after it has exited combat. In order to have the desired effect, we should gate a period of time where the navMeshAgent can not be reeanbled. 

        //Basing offNaveMesh interaction on Poisemeter being broken. 
        PoiseMeter targetPoiseReference = target.GetComponentInParent<Bladeclubber>().poiseMeter;
        if(targetPoiseReference.GetPoise() <= 0)
        {
            //This Force pattern knocks the enemy back and upwards
            //TODO: Add conditional logic to switch how the force vector is applied. For example, a floating type that has the enemy hovering in the air.
            target.GetComponentInParent<Rigidbody>().AddForce((-target.transform.forward + target.transform.up) * force);
            Debug.Log("Combat: " + target.name + " applied a force of " + force + " in direction " + (-target.transform.forward));

             //A stun timer method would work well. 
            target.GetComponentInParent<NavMeshAgent>().enabled = false;
        }
        else
        {
            //target.GetComponentInParent<Rigidbody>().AddForce((-target.transform.forward) * force/10.0f);

            //Shake the target
            target.GetComponentInParent<NavMeshAgent>().enabled = false;
            float shakeFactor = Mathf.Sin(Time.time * 30f) * 1;
            target.transform.position = new Vector3(target.transform.position.x + shakeFactor , target.transform.position.y +shakeFactor, target.transform.position.z);
            target.GetComponentInParent<NavMeshAgent>().enabled = true;

            Debug.Log("Combat: " + target.name + " applied a force of " + force + " in direction " + (-target.transform.forward));
        }


    }

    public void Initialize(float damage, float impactForce, AnimatorStateInfo animatorStateInfo, GameObject target, GameObject combatVFX, int hitCount)
    {
        this.damage = damage;
        this.impactForce = impactForce;
        this.animatorStateInfo = animatorStateInfo;
        this.target = target;
        this.combatVFX = combatVFX;
        this.hitCount = hitCount;
        this.currentHitCount = 0;
        this.initialized = true;
    }

}