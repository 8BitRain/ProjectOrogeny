using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Collision : IComparable<Collision>
{
    public String colliderName;
    public GameObject collisionHit;

    public Collision(String colliderName, GameObject collisionHit){
        this.colliderName = colliderName;
        this.collisionHit = collisionHit;
    }

    public int CompareTo(Collision other){
        //Test this
        return this.colliderName.CompareTo(other.colliderName);
    }
}
