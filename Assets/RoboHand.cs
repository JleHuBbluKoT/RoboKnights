using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboHand : MonoBehaviour
{
    public DriveAttempt shoulder;
    public DriveAttempt arm;
    public DriveAttempt palm;
    public List<DriveAttempt> fingers;
    public Transform sniffer;
    public Transform product;
    public Transform pedestal;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveFingers(float strength)
    {
        foreach (var f in fingers)
        {
            f.moveDrive( Vector3.right * strength);
        }
    }
}
