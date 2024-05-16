using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class AgentController : Agent
{
    public float speed;
    public Vector3 startPos;
    [SerializeField]
    private Transform target;


    public override void OnEpisodeBegin() {
        this.transform.localPosition = startPos;
        float fi = Random.Range(-Mathf.PI, Mathf.PI);
        float lamdaorsomething = Random.Range(3f, 7f);

        target.localPosition = new Vector3(Mathf.Cos(fi), 0, Mathf.Sin(fi)) * lamdaorsomething + startPos;
    }
    public override void OnActionReceived(ActionBuffers actions) {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        transform.localPosition +=  new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed;
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> ContiniousAction = actionsOut.ContinuousActions;
        ContiniousAction[0] = Input.GetAxisRaw("Horizontal");
        ContiniousAction[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pellet")
        {
            AddReward(10f);
            EndEpisode();
        }
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-5f);
            EndEpisode();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
