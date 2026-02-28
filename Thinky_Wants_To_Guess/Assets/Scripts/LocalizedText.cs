using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;

    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    void UpdateText()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("LocalizationManager not found");
            return;
        }

        text.text = LocalizationManager.Instance.GetText(key);
    }
}