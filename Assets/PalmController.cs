using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmController : MonoBehaviour
{
    public RoboHand roboHand;
    public List<DriveAttempt> fingers;
    public List<FingerSensor> fingersSensors;
    public void moveFingers(float x, float y, float z)
    {
        foreach (var f in fingers)
        {
            f.moveDrive(x, y, z);
        }
    }
    public void resetFingers()
    {
        foreach (var f in fingers)
        {
            f.SetRotation(Vector3.zero);
        }
    }
    
    public float fingersTouching() // ѕринимает значение от отрицательного до положительного количества пальцев
    {
        int reward = 0;
        foreach (var f in fingersSensors)
        {
            reward += f.state;
        }
        return reward;
    }

}
