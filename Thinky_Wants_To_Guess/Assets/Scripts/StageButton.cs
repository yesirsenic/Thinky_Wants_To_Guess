using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public int stageNum;
    public bool isClear;

    public void Setup(StageData data)
    {
        stageNum = data.Stage_Num;
        isClear = data.IsClear;

        if(!isClear)
        {
            if(transform.childCount != 0)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }
}
