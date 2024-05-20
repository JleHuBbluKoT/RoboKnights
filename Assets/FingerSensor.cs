using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerSensor : MonoBehaviour
{
    public PalmController palmController;
    public int state = 0;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("works");
        if (collision.gameObject.tag == "Wall")
        {
            state = -1;
            Debug.Log("works");
        }
        else if (collision.gameObject.tag == "Pellet")
        {
            state = 1;
        } else
        {
            state = 0;
        }
    }

    private void Update()
    {
        
    }
}
