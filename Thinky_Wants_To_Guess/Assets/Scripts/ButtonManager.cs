using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    //메인 메뉴
    public void ToStageSelectScene()
    {
        string savedKey = PlayerPrefs.GetString("User_OpenAI_Key");
        int stageAllClear = PlayerPrefs.GetInt("StageAllClear", 0); 

        if (stageAllClear != 1 || !string.IsNullOrEmpty(savedKey))
        {
            SceneManager.LoadScene("StageSelect");
        }
    }

    public void Exit_Button()
    {
        Application.Quit();
    }

    //스테이지 선택
    public void ToLevelSelectScene(int num)
    {
        GameManager.Instance.stageNum = num;


        SceneManager.LoadScene("LevelSelect");
    }

    public void ToMainGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void LoadOtherScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //소리
    public void ButtonSound()
    {
        SFXManager.Instance.Play(SFXManager.SFXType.ButtonClick);
    }

}
