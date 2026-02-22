using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public int levelNum;
    public string suggestWord;
    public bool isClear;

    public void Setup(StageData data)
    {
        levelNum = data.Level_Num;
        suggestWord = data.Suggest_Word;
        isClear = data.IsClear;

        Debug.Log($"레벨:{levelNum} / 단어:{suggestWord} / 클리어:{isClear}");
    }
}
