using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine;

public class ScreenshakeEvent : MonoBehaviour
{
    public UnityEvent shake;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShakeScreen", .1f);
        //InvokeRepeating("ShakeScreen", 3f, 4f);
    }

    private void ShakeScreen()
    {
        shake.Invoke();
    }
}
