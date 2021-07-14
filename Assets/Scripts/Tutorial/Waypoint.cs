using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private bool _playerReachedWaypoint = false;
    public enum WaypointType {Gameplay, Tutorial};
    public WaypointType waypointType;


    public int id;

    [Header("TutorialController")]
    public TutorialController tutorialController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "P1")
        {
            UpdateTutorialController();
            _playerReachedWaypoint = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "P1")
        {
            _playerReachedWaypoint = false;
        }
    }

    public bool HasPlayerReachedWaypoint()
    {
        return _playerReachedWaypoint;
    }

    public int GetID()
    {
        return id;
    }
    
    //Returns the Tutorial Controller 
    public TutorialController GetTutorialController()
    {
        return this.tutorialController;
    }

    //Sets the Tutorial Controller reference on this object
    public void SetTutorialController(TutorialController tc)
    {
        tutorialController = tc;
    }

    //Updates Tutorial Controller
    public void UpdateTutorialController()
    {
        if(GetTutorialController() != null)
        {
            GetTutorialController().PlayerReachedWaypoint(GetID());
        }
    }
}
