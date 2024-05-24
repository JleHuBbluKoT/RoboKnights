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
        startPedestal.transform.localPosition = new Vector3(Mathf.Cos(fi), 0, Mathf.Sin(fi)) * lamdaorsomething + floor.transform.localPosition + Vector3.up;
        finishPedestal.transform.localPosition = new Vector3(Mathf.Cos(fi + lamdaorsomething), 0, Mathf.Sin(fi + lamdaorsomething)) * lamdaorsomething + floor.transform.localPosition + Vector3.up;
        objective.transform.position = startPedestal.targetPoint.position;
        objective.GetComponent<Rigidbody>().velocity = Vector3.zero;

    }
}
