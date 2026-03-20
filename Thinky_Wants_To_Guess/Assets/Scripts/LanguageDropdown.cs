using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    private Dropdown dropdown;

    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }

    void Start()
    {
        int languageIndex;

        // 저장된 값 있으면 사용, 없으면 영어(1번이라고 가정)
        if (PlayerPrefs.HasKey("Language"))
        {
            languageIndex = PlayerPrefs.GetInt("Language");
        }
        else
        {
            languageIndex = (int)LocalizationManager.Language.English;

            // 처음이면 영어로 설정도 같이 해줌
            LocalizationManager.Instance.ChangeLanguage(
                LocalizationManager.Language.English
            );
        }

        dropdown.value = languageIndex;
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    public void OnLanguageChanged(int index)
    {
        PlayerPrefs.SetInt("Language", index);
        PlayerPrefs.Save();

        LocalizationManager.Instance.ChangeLanguage(
            (LocalizationManager.Language)index
        );
    }
}