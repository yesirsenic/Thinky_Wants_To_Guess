using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    Text suggestWord;

    [SerializeField]
    Image character_Sprite;

    [SerializeField]
    Text ai_Text;

    [SerializeField]
    Sprite[] CharacterSprites;

    GameState state;

    Coroutine dotsCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("MainGameManager duplicate destroyed");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        character_Sprite.sprite = CharacterSprites[0];

        suggestWord.GetComponent<LocalizedSuggestWord>().SetWord(GameManager.Instance.suggestKey);

        state = GameState.Wait;

        dotsCoroutine = StartCoroutine(IntervalDots(ai_Text));

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


    }
}
