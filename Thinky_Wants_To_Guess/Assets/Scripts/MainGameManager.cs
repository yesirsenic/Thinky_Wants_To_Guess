using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    Wait,
    Check,
    Explanation,
    End
}

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    public bool isClear;

    [Header("Pre Game")]
    [SerializeField]
    Text suggestWord;

    [SerializeField]
    DrawingCanvas drawingCanvas;

    [Header("Gaming")]
    [SerializeField]
    Image character_Sprite;

    [SerializeField]
    Text ai_Text;

    [SerializeField]
    Sprite[] CharacterSprites;

    [Header("Game End")]
    [SerializeField]
    GameObject gameEndPanel;

    [SerializeField]
    GameObject black_Paenl;

    [SerializeField]
    Image drawing;

    [SerializeField]
    Text ai_Text_EndPaenl;

    [SerializeField]
    GameObject[] gameEndCollection;

    [SerializeField]
    GameObject GameEndNextButton;

    GameState state;

    Coroutine dotsCoroutine;

    float endInterval;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("MainGameManager duplicate destroyed");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        isClear = false;
        endInterval = 2.5f;
    }

    private void Start()
    {
        character_Sprite.sprite = CharacterSprites[0];

        suggestWord.GetComponent<LocalizedSuggestWord>().SetWord(GameManager.Instance.suggestKey);

        state = GameState.Wait;

        dotsCoroutine = StartCoroutine(IntervalDots(ai_Text));

    }

    private void StageEnd()
    {
        Texture2D tex = drawingCanvas.GetTexture();

        Sprite sprite = Sprite.Create(
        tex,
        new Rect(0, 0, tex.width, tex.height),
        new Vector2(0.5f, 0.5f)
        );

        drawing.sprite = sprite;

        ai_Text_EndPaenl.text = ai_Text.text;

        if(isClear)
        {
            gameEndCollection[0].SetActive(true);
            LevelClear();

        }

        else
        {
            gameEndCollection[1].SetActive(true);
        }
    }

    private void LevelClear()
    {
        if (PlayerPrefs.GetInt("MaxStage") != GameManager.Instance.stageNum || PlayerPrefs.GetInt("MaxLevel") != GameManager.Instance.stageLevelNum)
            return;

        if(PlayerPrefs.GetInt("MaxLevel") == GameManager.Instance.maxLevelStage)
        {
            PlayerPrefs.SetInt("MaxLevel", 1);
            PlayerPrefs.SetInt("MaxStage", PlayerPrefs.GetInt("MaxStage") + 1);
        }

        else
        {
            PlayerPrefs.SetInt("MaxLevel", PlayerPrefs.GetInt("MaxLevel") + 1);
        }

        //20스테이지의 경우
        if(GameManager.Instance.stageNum == 20)
        {
            GameEndNextButton.SetActive(false);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator IntervalDots(Text text, float interval = 0.25f)
    {
        int dotCount = 0;

        while (true)
        {
            dotCount++;
            if (dotCount > 3) dotCount = 0;

            string baseText = text.text.TrimEnd('.');
            text.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator StageEndStart()
    {
        yield return new WaitForSeconds(endInterval);

        gameEndPanel.SetActive(true);

        black_Paenl.SetActive(true);

        StageEnd();
    }

    public void SubmitEnd(GameObject submitButton)
    {
        submitButton.SetActive(false);

        ai_Text.GetComponent<LocalizedText>().key = "Text_THINKING";

        ai_Text.GetComponent<LocalizedText>().UpdateTextPublic();

        character_Sprite.sprite = CharacterSprites[1];

        state = GameState.Check;
    }

    public void ResponseExplanation(string comment)
    {
        StopCoroutine(dotsCoroutine);

        ai_Text.text = comment;

        character_Sprite.sprite = CharacterSprites[2];

        state = GameState.Explanation;

        StartCoroutine(StageEndStart());
    }

    public void NextStage()
    {
        StageLevelData leveldata = GameManager.Instance.stageLevelDataList[GameManager.Instance.stageLevelNum];

        GameManager.Instance.stageLevelNum++;
        GameManager.Instance.suggestKey = leveldata.Key_Suggest;
        GameManager.Instance.suggestWord = leveldata.Suggest_Word;

        ReloadScene();


    }

    public void Retry()
    {
        ReloadScene();
    }

    public void ToLevelSelectScene()
    {
        SceneManager.LoadScene("LevelSelct");
    }

    
}
