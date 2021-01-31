using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    // Start is called before the first frame update
    public float fadeTime = .5f;
    void Start()
    {
        Destroy(this.gameObject, fadeTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
