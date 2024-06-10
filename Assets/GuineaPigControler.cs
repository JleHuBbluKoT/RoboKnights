using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine.UI;

public class GuineaPigControler : MonoBehaviour
{
    public RoboHand RH;
    public BehaviorParameters behPar;
    public ContextMenuLogic modelPick;
    public Text currentModel;
    public CameraScript myCamera;
    public List<string> modelNames;
    public List<Unity.Barracuda.NNModel> modelProper;
    public Dictionary<string, Unity.Barracuda.NNModel> modelList;
    

    private void Start()
    {
        modelList = new Dictionary<string, Unity.Barracuda.NNModel>();
        for (int i = 0; i < modelNames.Count; i++) {
            modelList.Add(modelNames[i], modelProper[i]);
        }
        modelPick.PopulateMenu(modelNames, this);
        CamLock();

    }

    public void ChangeNN(string modelName)
    {
        behPar.Model = modelList[modelName];
        RH.EndEpisode();
        Debug.Log(modelName);
        currentModel.text = modelName;
    }

    public void EpisodeDuration(InputField field)
    {
        RH.MaxStep = int.Parse( field.text);
    }
    public void EndEpisode()
    {
        RH.EndEpisode();
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void CamLock()
    {
        if (myCamera.canMove == 1)
        {
            myCamera.canMove = 0;
            RH.AllowHeuristic = true;
        } else
        {
            myCamera.canMove = 1;
            RH.AllowHeuristic = false;
        }
    }
    public void BallMode()
    {
        myCamera.BallMode = myCamera.BallMode ? false : true;
    }

    public void CamReset()
    {
        myCamera.ResetPos();
    }

}
