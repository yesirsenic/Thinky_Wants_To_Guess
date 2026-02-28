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
}