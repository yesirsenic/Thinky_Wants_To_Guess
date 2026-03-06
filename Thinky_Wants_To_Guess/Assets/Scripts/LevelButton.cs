using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelNum;
    public string suggestWord;
    public string suggestKey;
    public string category;
    public bool isClear;

    [SerializeField]
    Text suggestText;

    [SerializeField]
    GameObject blackPanel;

    public void Setup(StageLevelData data)
    {
        levelNum = data.Level_Num;
        suggestWord = data.Suggest_Word;
        isClear = data.IsClear;
        suggestKey = data.Key_Suggest;

        suggestText.gameObject.GetComponent<LocalizedSuggestWord>().SetWord(suggestKey);
        

        if(!isClear)
        {
            if(blackPanel != null)
            {
                blackPanel.SetActive(true);
                gameObject.GetComponent<Button>().interactable = false;
            }

        }
    }

    public void SetSuggestWord()
    {
        GameManager.Instance.suggestWord = suggestWord;
        GameManager.Instance.suggestKey = suggestKey;
    }
}
