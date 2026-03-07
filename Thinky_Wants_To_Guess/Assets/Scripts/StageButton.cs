using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public int stageNum;

    public void Setup(StageData data)
    {
        stageNum = data.Stage_Num;

        if (stageNum > PlayerPrefs.GetInt("MaxStage"))
        {
            if (transform.childCount != 0)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }
}
