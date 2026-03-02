using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedSuggestWord : MonoBehaviour
{
    private Text text;
    private string currentKey;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += Refresh;
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= Refresh;
    }

    public void SetWord(string key)
    {
        currentKey = key;
        Refresh();
    }

    void Refresh()
    {
        if (string.IsNullOrEmpty(currentKey)) return;

        text.text = LocalizationManager.Instance.GetText(currentKey);
    }
}
