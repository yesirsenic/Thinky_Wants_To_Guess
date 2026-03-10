using UnityEngine;

public class PausePopupUI : MonoBehaviour
{
    public void Resume()
    {
        PauseManager.Instance.ClosePopup();
    }

    public void GoToMenu()
    {
        PauseManager.Instance.GoToMenu();
    }

    public void ButtonSound()
    {
        SFXManager.Instance.Play(SFXManager.SFXType.ButtonClick);
    }
}