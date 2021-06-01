using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    //Asteroid Force
    public float asteroidForce = 3;
    public Rigidbody mantleRb;
    void Start()
    {
        //Select a random direction for the asteroid to move
        int direction = Random.Range(0,3);
        //Apply force in direction: 0 = up, 1 = down, 2 = left, 3 = right
        switch (direction)
        {
            case 0:
                this.GetComponent<Rigidbody>().AddForce(transform.up * asteroidForce); 
                break;
            case 1:
                this.GetComponent<Rigidbody>().AddForce(-transform.up * asteroidForce); 
                break;
            case 2:
                this.GetComponent<Rigidbody>().AddForce(transform.right * asteroidForce); 
                break;
            case 3:
                this.GetComponent<Rigidbody>().AddForce(-transform.right * asteroidForce); 
                break;
            default:
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "P1")
        {
            print("Asteroid: Colliding with player");

            //Accessibility feature, when a player lands on a moving platform, have the platform stop moving so the player can keep their footing.
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
