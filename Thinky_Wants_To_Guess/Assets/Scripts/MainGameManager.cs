using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    [SerializeField]
    Text suggestWord;

    [SerializeField]
    Text ai_Text;

    private void Start()
    {
        suggestWord.GetComponent<LocalizedSuggestWord>().SetWord(GameManager.Instance.suggestKey);

        StartCoroutine(WaitingDots(ai_Text));

    }

    IEnumerator WaitingDots(Text text, float interval = 0.25f)
    {
        string baseText = text.text;
        int dotCount = 0;

        while (true)
        {
            dotCount++;
            if (dotCount > 3) dotCount = 0;

            text.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(interval);
        }
    }
}
