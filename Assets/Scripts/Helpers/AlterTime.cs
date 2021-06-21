using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AlterTime
{

    public static void SlowTime(float factor, float duration)
    {
        float timeScale = 1*factor;
        Debug.Log("Slowing Time by factor of: " + factor);
        Time.timeScale = timeScale;
        ResetTime(duration);
    }

    public static void SpeedUpTime(float factor, float duration)
    {
        float timeScale = 1*factor;
        Debug.Log("Speeding up Time by factor of: " + factor);
        Time.timeScale = timeScale;
        ResetTime(duration);
    }

    public static void ResetTime(float duration)
    {
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
        }
        Time.timeScale = 1;
        Debug.Log("Resetting Time");
    }

    //direction, 0 = speed up, 1 = slowdown
    /*IEnumerator SlowTime(float factor, float duration)
    {
        float timeScale = 1*factor;

        //Adjust Time
        Time.timeScale = timeScale;

        float timer = 0;
        while(timer < duration)
        {
            timer+= Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1;
    }

        IEnumerator SpeedUpTime(float factor, float duration)
    {
        float timeScale = 1/factor;
        
        //Adjust Time
        Time.timeScale = timeScale;

        float timer = 0;
        while(timer < duration)
        {
            timer+= Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1;
    }*/

    public static IEnumerator AdjustTime(float timeScale, float duration)
    {
        
        //Adjust Time
        Time.timeScale = timeScale;

        float timer = 0;
        while(timer < duration)
        {
            Debug.Log("Timer is running:" + timer);
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Resetting timescale");
        Time.timeScale = 1;
    }
}
