using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

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
    public float strength;  // Важно видеть
    public GameObject handItself;
    public bool doDebug; // Важно видеть
    public bool smallDebug;

    public float levelMultiplier;
    public Vector2 L1clawLimits;
    public Vector2 ballPowerDist;
    public float ballTouchReward;
    public Vector2 pedestalPowerDist;

    public BehaviorParameters BhParam;

    public override void OnEpisodeBegin()
    {
        if (smallDebug)
        {
            if (level == 1)
            {
                Debug.Log("Level 1 quit");
            }
            if (level ==2 )
            {
                Debug.Log("Level 2 quit");
            }
            if (level == 3)
            {
                Debug.Log("Level 3 quit");
            }
        }
        stageSet.ResetStage(this, 0);
        shoulder.SetRotation(Vector3.zero);
        arm.SetRotation(Vector3.zero);
        palm.SetRotation(Vector3.zero);
        palmController.resetFingers();
        goToDefault = 0;
        handItself.transform.rotation = Quaternion.Euler(0, 0, 0);
        handItself.transform.localPosition = new Vector3(0f, -1.65f, 0f);
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

        //Debug.LogFormat("snifferPos: {0} | product: {1} | pedestal: {2} | snifferBall: {3} | BallPedestal: {4}", EnvPos(sniffer), EnvPos(product.transform), EnvPos(pedestal.targetPoint), DistanceSnifferBall(), DistanceBallPedestal());
        

        
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
        
        if (Vector3.Distance(EnvPos(this.transform), Vector3.zero) > 10f)
        {
            IAmAFailure();
        }
        if (Vector3.Distance(EnvPos(product.transform), Vector3.zero) > 15f)
        {
            IAmAFailure();
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("registered");
            if (BhParam.BehaviorType == BehaviorType.Default)
            {
                Debug.Log("was default, swithced to heuristic");
                BhParam.BehaviorType = BehaviorType.HeuristicOnly;
            }
            else
            {
                Debug.Log(" swithced to default");
                BhParam.BehaviorType = BehaviorType.Default;
            }
        }
    }

    private void LevelOneCheck() { // рука должна приблизиться к шару, не опрокинув его
        float lvlMultiplier = 1 + ((level - 1) * levelMultiplier);

        float ballDist = ballPowerDist.x * Mathf.Pow(DistanceSnifferBall() - ballPowerDist.y, 3) * -1;
        float clawFar = product.inhand * ClawOpenTooFarFromBall() * L1clawLimits.x + (product.inhand - 1) * ClawOpenTooFarFromBall() * L1clawLimits.y;
        float touchProduct = product.inhand * ballTouchReward;

        ballDist *= lvlMultiplier; clawFar *= lvlMultiplier; touchProduct *= lvlMultiplier;

        AddReward(ballDist * lvlMultiplier);
        AddReward(clawFar * lvlMultiplier);
        AddReward(touchProduct * lvlMultiplier);
        if (doDebug)
        {
            Debug.Log("distance: " + ballDist + " | clawFar: " + clawFar +  " | touching: " + touchProduct  + " | total: " + (ballDist + clawFar  + touchProduct) + " | ballH"  + EnvPos(product.transform).y);
        }
        if (EnvPos(product.transform).y > 2.7f)
        {
            this.level = 2;
            AddReward(20f);
        }
        if (product.stay > 50)
        {
            level = 3;
            AddReward(20f);
            //goToDefault = 1;
        }

    }

    private void LevelTwoCheck()
    { // рука должна поднять шар как можно выше
        float lvlMultiplier = 1 + ((level - 1) * levelMultiplier);

        float ballDist = ballPowerDist.x * Mathf.Pow(DistanceSnifferBall() - ballPowerDist.y, 3) * (-1);
        float ballAlt = (EnvPos(product.transform).y + 2) *0.001f;
        float ballpedestal = pedestalPowerDist.x * Mathf.Pow(DistanceBallPedestal() - pedestalPowerDist.y, 1) * (-1);
        float touchProduct = product.inhand * ballTouchReward;
        float clawFar = product.inhand * ClawOpenTooFarFromBall() * L1clawLimits.x + (product.inhand - 1) * ClawOpenTooFarFromBall() * L1clawLimits.y;

        ballDist *= lvlMultiplier; ballAlt *= lvlMultiplier; ballpedestal *= lvlMultiplier; touchProduct *= lvlMultiplier;

        AddReward(ballDist );
        AddReward(ballAlt );
        AddReward(ballpedestal );
        AddReward(touchProduct );
        AddReward(clawFar);


        if (doDebug)
        {
            Debug.Log("distance: " + ballDist + " | altitude: " + ballAlt + " | ball_pedestal: " + ballpedestal + " | touching: " + touchProduct + " | claw: "  +clawFar  + " | total" + (ballDist + ballAlt + ballpedestal + touchProduct + clawFar));
        }
        if (product.stay > 50)
        {
            level = 3;
            AddReward(20f);
            //goToDefault = 1;
        }

    }
    private void LevelThreeCheck()
    { // рука должна положить шар на другой пьедестал
        float lvlMultiplier = 1 + ((level - 1) * levelMultiplier);

        float ballpedestal = pedestalPowerDist.x * Mathf.Pow(DistanceBallPedestal() - pedestalPowerDist.y, 1) * (-1);
        float defaultPosD = -0.0001f * differenceToDefault();
        float clawFar = product.inhand * -1 * 0.04f *  ClawOpenTooFarFromBall();
        float ballInPedestal = product.pedestal * (0.7f + 0.7f);

        ballpedestal *= lvlMultiplier; defaultPosD *= lvlMultiplier; clawFar *= lvlMultiplier; ballInPedestal *= lvlMultiplier;

        AddReward(ballpedestal);
        AddReward(defaultPosD);
        AddReward(clawFar);
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
    private Vector3 shoulderDefault = Vector3.zero;
    private Vector3 armDefault = Vector3.zero;
    private Vector3 palmDefault = Vector3.zero;
    private Vector3 fingerDefault = Vector3.zero;
}
