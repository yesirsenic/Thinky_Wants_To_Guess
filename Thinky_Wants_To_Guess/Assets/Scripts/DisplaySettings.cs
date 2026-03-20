using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    public Dropdown screenModeDropdown;

    public void SetScreenMode(int modeIndex)
    {
        switch (modeIndex)
        {
            case 0:
                Debug.Log("창모드");
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(1280, 720, false);
                break;

            case 1:
                Debug.Log("전체 화면 (진짜)");

                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;

                // 🔥 핵심: 모니터 해상도 맞추기
                Resolution res = Screen.currentResolution;
                Screen.SetResolution(res.width, res.height, true);

                break;

            case 2:
                Debug.Log("보더리스");

                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

                // 보더리스는 해상도 설정 안 해도 됨
                break;
        }

        PlayerPrefs.SetInt("ScreenMode", modeIndex);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        if (screenModeDropdown == null)
        {
            screenModeDropdown = GetComponent<Dropdown>();
        }

        int savedMode = PlayerPrefs.GetInt("ScreenMode", 1);

        // 🔥 드롭다운 먼저 맞춰주기
        screenModeDropdown.value = savedMode;

        // 🔥 UI 갱신
        screenModeDropdown.RefreshShownValue();

        // 🔥 실제 적용
        SetScreenMode(savedMode);
    }
}
