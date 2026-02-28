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
        // 현재 언어를 드롭다운에 반영
        dropdown.value = (int)LocalizationManager.Instance.currentLanguage;
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    public void OnLanguageChanged(int index)
    {
        LocalizationManager.Instance.ChangeLanguage(
            (LocalizationManager.Language)index
        );
    }

}