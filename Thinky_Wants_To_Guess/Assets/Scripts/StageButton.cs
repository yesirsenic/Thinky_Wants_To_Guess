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


        if(GameManager.Instance.is_Demo)
        {

            if (stageNum < 2)
                return;

            //데모를 위한 스테이지 락
            for (int i = 2; i<=10; i++)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                gameObject.GetComponent<Button>().interactable = false;
            }
        }
        
    }
}
