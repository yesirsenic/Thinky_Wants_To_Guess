using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    public int stageNum;
    public bool isClear;

    public void Setup(StageData data)
    {
        stageNum = data.Stage_Num;
        isClear = data.IsClear;

        Debug.Log($"스테이지:{stageNum} / 클리어:{isClear}");
    }
}
