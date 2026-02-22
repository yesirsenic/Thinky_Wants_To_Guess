using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    //메인 메뉴
    public void ToStageSelectScene()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void Exit_Button()
    {
        Application.Quit();
    }

    //스테이지 선택
    public void ToLevelSelectScene(int num)
    {
        GameManager.Instance.stageNum = num;


        SceneManager.LoadScene("LevelSelct");
    }

}
