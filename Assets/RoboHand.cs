using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class RoboHand : Agent
{
    public DriveAttempt shoulder;
    public DriveAttempt arm;
    public DriveAttempt palm;
    public PalmController palmController;
    public Transform sniffer;
    public Transform product;
    public ItemStand pedestal; // target pedestal

    int goToDefault; // ��������� ������ ������� ���� �� ��������� �������

    public Transform EnvCenter;
    public StageSet stageSet;

    public int level; // ������ ��������
    public float strength;
    public GameObject handItself;
    public bool doDebug;

public override void OnEpisodeBegin()
    {
        stageSet.ResetStage(this, 0);
        shoulder.SetRotation(Vector3.zero);
        arm.SetRotation(Vector3.zero);
        palm.SetRotation(Vector3.zero);
        palmController.resetFingers();
        goToDefault = 0;
        handItself.transform.rotation = Quaternion.Euler(0, 0, 0);
        handItself.transform.localPosition = new Vector3(0f, 0f, 0f);
        //level = 0;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float shoulderSwingX = actions.ContinuousActions[0] * strength;
        float shoulderTwistY = actions.ContinuousActions[1] * strength;
        float armSwingX = actions.ContinuousActions[2] * strength;
        float palmSwingX = actions.ContinuousActions[3] * strength;
        float palmTwistY = actions.ContinuousActions[4] * strength;
        float fingerSwingX = actions.ContinuousActions[5] * strength;
        // 6 ���������� �������� ��������� �������� ����������

        shoulder.moveDrive(shoulderSwingX, shoulderTwistY, 0);
        arm.moveDrive(armSwingX, 0, 0);
        palm.moveDrive(palmSwingX, palmTwistY, 0);
        palmController. moveFingers(fingerSwingX,0,0);

    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(EnvPos(sniffer));
        sensor.AddObservation(EnvPos(product));
        sensor.AddObservation(EnvPos(pedestal.targetPoint));
        sensor.AddObservation(DistanceSnifferBall());
        sensor.AddObservation(DistanceBallPedestal());
        sensor.AddObservation(goToDefault * differenceToDefault());

        sensor.AddObservation(shoulder.CurrentRotation());// ��� ���, �����/���� � ��������
        sensor.AddObservation(arm.CurrentRotation()); // ���� ���
        sensor.AddObservation(palm.CurrentRotation()); // ��� ���, �����/���� � ��������
        sensor.AddObservation(palmController.fingers[0].CurrentRotation()); // ���� ���, �������� � ���� ����������, �� ����� ��� ���������


        

        
        // ����� 6 ����, 6 ������� ��� ��������
    }
    public override void Heuristic(in ActionBuffers actionsOut)
        {
        ActionSegment<float> ContiniousAction = actionsOut.ContinuousActions;
        ContiniousAction[0] = Input.GetAxisRaw("Vertical");
        ContiniousAction[1] = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.R)) ContiniousAction[2] = 1;
        else if (Input.GetKey(KeyCode.F)) ContiniousAction[2] = -1;
        else ContiniousAction[2] = 0;

        if (Input.GetKey(KeyCode.Y)) ContiniousAction[3] = 1;
        else if (Input.GetKey(KeyCode.H)) ContiniousAction[3] = -1;
        else ContiniousAction[3] = 0;
        if (Input.GetKey(KeyCode.G)) ContiniousAction[4] = 1;
        else if (Input.GetKey(KeyCode.J)) ContiniousAction[4] = -1;
        else ContiniousAction[4] = 0;

        if (Input.GetKey(KeyCode.I)) ContiniousAction[5] = 1;
        else if (Input.GetKey(KeyCode.K)) ContiniousAction[5] = -1;
        else ContiniousAction[5] = 0;
        //Debug.Log("i am on tv");
    }



    void FixedUpdate() {
        switch (level)
        {
            case 1:
                LevelOneCheck();
                break;
            case 2:
                LevelTwoCheck();
                break;
            case 3:
                LevelThreeCheck();
                break;
            default:
                break;
        }
        
    }

    private void LevelOneCheck() { // ���� ������ ������������ � ����, �� ��������� ���
        float ballDist = 0.001f * Mathf.Pow(DistanceSnifferBall() - 1.0f, 3) * (-1);
        AddReward(ballDist);
        //Debug.Log(ClawOpenTooFarFromBall());
        float clawFar = Mathf.Clamp( 0.002f * ClawOpenTooFarFromBall(), 0, 1f);
        AddReward(clawFar);
        float baseB = BrokenBase() * -0.5f;
        AddReward(baseB);
        float rew = Mathf.Pow(palmController.fingersTouching(), 3) * 0.1f;
        AddReward(rew);
        if (doDebug)
        {
            Debug.Log("distance: " + ballDist + " | clawFar: " + clawFar + " | broken base: " + baseB + " | fingies: " + rew);
        }
        
    }

    private void LevelTwoCheck()
    { // ���� ������ ������� ��� ��� ����� ����
        AddReward(0.001f / Mathf.Max(DistanceSnifferBall(), 0.1f));
        AddReward( (EnvPos(product).y + 1) * 0.01f);
    }
    private void LevelThreeCheck()
    { // ���� ������ �������� ��� �� ������ ���������
        if (goToDefault == 0) { // ������ ������������
            AddReward(0.001f / Mathf.Max(DistanceSnifferBall(), 0.1f));
            AddReward(0.01f / Mathf.Max(DistanceBallPedestal(), 0.1f));
            if (DistanceBallPedestal()< 0.2f)
            {
                goToDefault = 0;
            }
        } else
        {
            AddReward(0.01f / Mathf.Max(DistanceBallPedestal(), 0.1f));
            AddReward(-0.0001f * differenceToDefault());
        }
       
    }

    public Vector3 EnvPos(Transform other)
    {
        return other.position - EnvCenter.position;
    }
    public float DistanceSnifferBall()
    {
        return Vector3.Distance(sniffer.position, product.position);
    }
    public float DistanceBallPedestal()
    {
        return Vector3.Distance(pedestal.targetPoint.position, sniffer.position);
    }
    public void Punish(float punishment)
    {
        AddReward(punishment);
    }
    public float differenceToDefault() // ������� ����� ��������� �������� � �������
    {
        float shoulderD = Mathf.Abs(shoulderDefault.x - shoulder.Vrotation.x) + Mathf.Abs(shoulderDefault.y - shoulder.Vrotation.y);
        float armD = Mathf.Abs(armDefault.x - arm.Vrotation.x);
        float palmD = Mathf.Abs(palmDefault.x - palm.Vrotation.x) + Mathf.Abs(palmDefault.y - palm.Vrotation.y);
        float fingerD = Mathf.Abs(fingerDefault.x - palmController.fingers[0].Vrotation.x);
        return shoulderD + armD + palmD + fingerD;
    }
    public float BrokenBase()
    {
        return Mathf.Abs(handItself.transform.rotation.x) + Mathf.Abs(handItself.transform.rotation.y) + Mathf.Abs(handItself.transform.rotation.z);
    }

    public float ClawOpenTooFarFromBall()
    {
        float closeness = (DistanceSnifferBall() - 1.0f) * (-1);
        float ret = palmController.fingers[0].Vrotation.x * closeness;

        return ret;
    }
    public Vector3 shoulderDefault = Vector3.zero;
    public Vector3 armDefault = Vector3.zero;
    public Vector3 palmDefault = Vector3.zero;
    public Vector3 fingerDefault = Vector3.zero;
}
