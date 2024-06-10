using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ContextMenuButtonLogic : MonoBehaviour
{
    //Func<> objectFunction;
    // Start is called before the first frame update
    public Text butonText;
    GuineaPigControler gpcontrol;
    //Organization.PlayerFunctionDelegate buttonAction;

    void Start()
    {
        
    }

    public void PopulateButton(GuineaPigControler gpc, string text = "no text")
    {
        butonText.text = text;
        gpcontrol = gpc;

        //buttonAction = action;
    } 
    public void ButtonPress()
    {
        gpcontrol.ChangeNN(butonText.text);
        //Debug.Log(buttonAction);
        //buttonAction();
    }

}
