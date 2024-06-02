using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSet : MonoBehaviour
{
    public ItemStand startPedestal;
    public ItemStand finishPedestal;
    public GameObject floor;
    public GameObject objective;
    public void ResetStage(RoboHand hand, int level)
    {
        float fi = Random.Range(-Mathf.PI, Mathf.PI);
        float lamdaorsomething = Random.Range(3f, 3.8f);
        float rnd = Random.Range(0f, 1f);

        if (Random.Range(0f, 1f) < 0.0001f)
        {
            fi = 3;
            lamdaorsomething = 3;
        }

        startPedestal.transform.localPosition = new Vector3(Mathf.Cos(fi), 0, Mathf.Sin(fi)) * lamdaorsomething + floor.transform.localPosition + Vector3.up;
        finishPedestal.transform.localPosition = new Vector3(Mathf.Cos(fi + lamdaorsomething + rnd ), 0, Mathf.Sin(fi + lamdaorsomething + rnd)) * lamdaorsomething + floor.transform.localPosition + Vector3.up;
        objective.transform.position = startPedestal.targetPoint.position;
        objective.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //print(lamdaorsomething + " " + fi);

    }
}
