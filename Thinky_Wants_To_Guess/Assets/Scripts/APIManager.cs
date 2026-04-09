using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    [SerializeField]
    GameObject APIComp;

    [SerializeField]
    GameObject[] APITexts;

    [Header("UI 연결")]
    [Tooltip("API 키를 입력받는 Input Field (Legacy)")]
    public InputField apiKeyInputField;

    [Tooltip("에러나 성공 메시지를 띄워줄 텍스트 (Legacy - 선택 사항)")]
    public Text statusMessageText;

    [Tooltip("입력 완료 후 닫을 팝업창 객체")]
    public GameObject replayPopup;

    // PlayerPrefs에 저장할 키 이름
    private const string PREFS_API_KEY = "User_OpenAI_Key";

    // 키 유효성을 테스트할 OpenAI의 가장 가벼운 엔드포인트
    private const string TEST_ENDPOINT = "https://api.openai.com/v1/models";


    public void StageClearCheck()
    {
        string savedKey = PlayerPrefs.GetString("User_OpenAI_Key");

        if (PlayerPrefs.GetInt("StageAllClear") == 1 && string.IsNullOrEmpty(savedKey))
        {
            APIComp.SetActive(true);
        }
    }

    /// <summary>
    /// UI의 'Play!' 버튼의 OnClick 이벤트에 연결할 함수입니다.
    /// </summary>
    public void OnClickPlayButton()
    {
        foreach(GameObject ob in APITexts)
        {
            ob.SetActive(false);
        }

        // Legacy InputField의 text 값을 가져옵니다.
        string inputKey = apiKeyInputField.text.Trim();

        // 1차 검사: 빈칸이거나 형식(sk-)이 맞지 않는 경우
        if (string.IsNullOrEmpty(inputKey) || !inputKey.StartsWith("sk-"))
        {
            UpdateStatusMessage(0);
            return;
        }

        // 2차 검사: 서버 통신을 통한 실제 유효성 검증
        StartCoroutine(ValidateAndSaveRoutine(inputKey));
    }

    private IEnumerator ValidateAndSaveRoutine(string keyToTest)
    {
        // UnityWebRequest를 사용해 OpenAI 서버에 Get 요청
        using (UnityWebRequest request = UnityWebRequest.Get(TEST_ENDPOINT))
        {
            // 헤더에 테스트할 API 키 세팅
            request.SetRequestHeader("Authorization", "Bearer " + keyToTest);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 검증 성공 -> PlayerPrefs에 안전하게 저장
                PlayerPrefs.SetString(PREFS_API_KEY, keyToTest);
                PlayerPrefs.Save();

                // 1초 뒤에 팝업을 닫고 게임을 시작하도록 딜레이
                Invoke(nameof(ClosePopupAndStartGame), 0.1f);
            }
            else
            {
                // 검증 실패 (보통 401 Unauthorized 에러 발생)
                UpdateStatusMessage(1);
                Debug.LogWarning($"API Key Validation Failed: {request.error}");
            }
        }
    }

    private void UpdateStatusMessage(int num)
    {
        APITexts[num].SetActive(true);
    }

    private void ClosePopupAndStartGame()
    {
        if (replayPopup != null)
        {
            replayPopup.SetActive(false);
        }

        // TODO: 여기에 재플레이할 스테이지 씬을 로드하거나 게임을 재개하는 코드를 추가하세요.
        SceneManager.LoadScene("StageSelect");
        Debug.Log("게임 플레이 시작 로직 실행");
    }
}
