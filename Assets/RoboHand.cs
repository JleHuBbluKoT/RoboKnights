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
    public ProductLogic product;
    public ItemStand pedestal; // target pedestal

    int goToDefault; // нейросеть должна вернуть руку на начальную позицию

    public Transform EnvCenter;
    public StageSet stageSet;

    public int level; // стадии обучения
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
        level = 1;
        product.stay = 0;
        product.pedestal = 0;
        product.inhand = 0;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float shoulderSwingX = actions.ContinuousActions[0] * strength;
        float shoulderTwistY = actions.ContinuousActions[1] * strength;
        float armSwingX = actions.ContinuousActions[2] * strength;
        float palmSwingX = actions.ContinuousActions[3] * strength;
        float palmTwistY = actions.ContinuousActions[4] * strength;
        float fingerSwingX = actions.ContinuousActions[5] * strength;
        // 6 переменных скорости изменения вращения соединения

        shoulder.moveDrive(shoulderSwingX, shoulderTwistY, 0);
        arm.moveDrive(armSwingX, 0, 0);
        palm.moveDrive(palmSwingX, palmTwistY, 0);
        palmController. moveFingers(fingerSwingX,0,0);

    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(EnvPos(sniffer));
        sensor.AddObservation(EnvPos(product.transform));
        sensor.AddObservation(EnvPos(pedestal.targetPoint));
        sensor.AddObservation(DistanceSnifferBall());
        sensor.AddObservation(DistanceBallPedestal());
        sensor.AddObservation(goToDefault * differenceToDefault());

        sensor.AddObservation(shoulder.CurrentRotation());// две оси, вверх/вниз и вращение
        sensor.AddObservation(arm.CurrentRotation()); // одна ось
        sensor.AddObservation(palm.CurrentRotation()); // две оси, вверх/вниз и вращение
        sensor.AddObservation(palmController.fingers[0].CurrentRotation()); // одна ось, вращение у всех одинаковое, не важно чье считывать


        

        
        // итого 6 осей, 6 выходов для действий
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

    private void LevelOneCheck() { // рука должна приблизиться к шару, не опрокинув его
        float ballDist = 0.002f * Mathf.Pow(DistanceSnifferBall() -2.2f, 3) * -1;
        AddReward(ballDist);
        //Debug.Log(ClawOpenTooFarFromBall());
        float clawFar = product.inhand * ClawOpenTooFarFromBall() *  0.005f + (product.inhand - 1) * ClawOpenTooFarFromBall() * 0.001f;
        AddReward(clawFar);
        float baseB = BrokenBase() * -0.1f;
        AddReward(baseB);
        float touchProduct = product.inhand * 0.06f;
        AddReward(touchProduct);
        //float rew = Mathf.Pow(palmController.fingersTouching(), 3) * 0.1f;
        //AddReward(rew);
        if (doDebug)
        {
            Debug.Log("distance: " + ballDist + " | clawFar: " + clawFar + " | brokenBase: " + baseB + " | touching: " + touchProduct + " | total: " + (ballDist + clawFar + baseB + touchProduct) + " | ballH"  + EnvPos(product.transform).y);
        }
        if (EnvPos(product.transform).y > 2.8f)
        {
            this.level = 2;
            AddReward(20f);
        }
        if (product.stay > 50)
        {
            level = 3;
            AddReward(20f);
        }

    }

    private void LevelTwoCheck()
    { // рука должна поднять шар как можно выше
        float ballDist = 0.002f * Mathf.Pow(DistanceSnifferBall() - 2.8f, 3) * (-1);
        AddReward(ballDist);
        float ballAlt = (EnvPos(product.transform).y + 1) *0.001f;
        AddReward(ballAlt);

        // должна приблизиться
        float ballpedestal = 0.0007f * Mathf.Pow(DistanceBallPedestal() - 7.0f, 3) * (-1);

        AddReward(ballpedestal);
        float touchProduct = product.inhand * 0.02f;
        AddReward(touchProduct);

        if (doDebug)
        {
            Debug.Log("distance: " + ballDist + " | altitude: " + ballAlt + " | ball_pedestal: " + ballpedestal + " | touching: " + touchProduct + " | total" + (ballDist + ballAlt + ballpedestal + touchProduct));
        }
        if (product.stay > 50)
        {
            level = 3;
            AddReward(20f);
        }

    }
    private void LevelThreeCheck()
    { // рука должна положить шар на другой пьедестал
        float ballpedestal = 0.004f * Mathf.Pow(DistanceBallPedestal() - 4.0f, 3) * (-1);
        AddReward(ballpedestal);
        float defaultPosD = -0.0001f * differenceToDefault();
        AddReward(defaultPosD);
        float clawFar = product.inhand * -1 * 0.01f *  ClawOpenTooFarFromBall();
        AddReward(clawFar);
        float ballInPedestal = product.pedestal * 0.1f;
        AddReward(ballInPedestal);
        if (doDebug)
        {
            Debug.Log("ball_pedestal: " + ballpedestal + " | posD " + defaultPosD + " | clawFar: " + clawFar + " | pedestal: " + ballInPedestal + " | total" + (ballpedestal + defaultPosD + clawFar + ballInPedestal));
        }
    }

    public Vector3 EnvPos(Transform other)
    {
        return other.position - EnvCenter.position;
    }
    public float DistanceSnifferBall()
    {
        return Vector3.Distance(sniffer.position, product.transform.position);
    }
    public float DistanceBallPedestal()
    {
        return Vector3.Distance( pedestal.targetPoint.position, product.transform.position);
    }
    public float DistanceSnifferPedestal()
    {
        return Vector3.Distance(pedestal.targetPoint.position, sniffer.position);
    }
    public void Punish(float punishment)
    {
        AddReward(punishment);
    }
    public float differenceToDefault() // разница между дефолтной позицией и текущей
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
        float ret = palmController.fingers[0].Vrotation.x ;

        return ret;
    }

    public void IAmAFailure()
    {
        AddReward(-1f);
        EndEpisode();
    }
    public Vector3 shoulderDefault = Vector3.zero;
    public Vector3 armDefault = Vector3.zero;
    public Vector3 palmDefault = Vector3.zero;
    public Vector3 fingerDefault = Vector3.zero;
}
