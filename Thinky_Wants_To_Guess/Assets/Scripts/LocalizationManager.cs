using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public enum Language
    {
        Korean,
        English,
        Chinese,
        Japanese,
        German,
        PortugueseBR,
        Russian
    }

    public Font fontKR;
    public Font fontJP;
    public Font fontSC;

    public Language currentLanguage = Language.Korean;

    public static event Action OnLanguageChanged;

    private Dictionary<string, string> localizedText = new Dictionary<string, string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentLanguage = (Language)PlayerPrefs.GetInt("LANGUAGE", 0);

            LoadLocalization();
        }
        else
        {
            Destroy(gameObject);
        }

        OnLanguageChanged?.Invoke();
    }


    void LoadLocalization()
    {
        localizedText.Clear();

        TextAsset csv = Resources.Load<TextAsset>("language");
        StringReader reader = new StringReader(csv.text);

        bool isFirstLine = true;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();

            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            string[] data = line.Split(',');

            string key = data[0];
            int langIndex = (int)currentLanguage + 1;

            if (data.Length > langIndex)
                localizedText[key] = data[langIndex];
        }
    }

    public string GetText(string key)
    {
        if (localizedText.ContainsKey(key))
            return localizedText[key];

        Debug.LogWarning("Missing localization key: " + key);
        return key;
    }

    public void ChangeLanguage(Language lang)
    {
        currentLanguage = lang;

        PlayerPrefs.SetInt("LANGUAGE", (int)lang);

        LoadLocalization();
        OnLanguageChanged?.Invoke();
    }

    public string GetLanguageName(Language lang)
    {
        switch (lang)
        {
            case Language.Korean: return "Korean";
            case Language.English: return "English";
            case Language.Chinese: return "Chinese";
            case Language.Japanese: return "Japanese";
            case Language.German: return "German";
            case Language.PortugueseBR: return "Brazilian Portuguese";
            case Language.Russian: return "Russian";
        }

        return "Korean";
    }

    public Font GetCurrentFont()
    {
        switch (currentLanguage)
        {
            case Language.Japanese:
                return fontJP;

            case Language.Chinese:
                return fontSC;

            default:
                return fontKR;
        }
    }
}