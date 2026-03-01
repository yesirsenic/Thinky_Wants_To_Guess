using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySettings : MonoBehaviour
{
    public void SetScreenMode(int modeIndex)
    {
        switch (modeIndex)
        {
            case 0:
                Debug.Log("창모드!");
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case 1:
                Debug.Log("전체 화면");
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;

            case 2:
                Debug.Log("보더리스");
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        PlayerPrefs.SetInt("ScreenMode", modeIndex);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        int savedMode = PlayerPrefs.GetInt("ScreenMode", 1);
        SetScreenMode(savedMode);
    }
}
