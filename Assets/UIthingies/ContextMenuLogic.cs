using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenuLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform mainRect;
    public GameObject buttonSpace;
    public GameObject buttonExample;
    public List<ContextMenuButtonLogic> myButtons;


    void Start()
    {
        //mainRect.sizeDelta = new Vector2Int(100, 200);
    }

    public void PopulateMenu(List<string> nameList, GuineaPigControler gpc)
    {
        myButtons = new List<ContextMenuButtonLogic>();
        //List<string> labels = organization.GetFunctions();

        for (int i = 0; i < nameList.Count; i++)
        {
            myButtons.Add(Instantiate(buttonExample).GetComponent<ContextMenuButtonLogic>());
            myButtons[i].transform.SetParent(buttonSpace.transform);
            myButtons[i].PopulateButton(gpc, nameList[i]);
        }
    }
}
