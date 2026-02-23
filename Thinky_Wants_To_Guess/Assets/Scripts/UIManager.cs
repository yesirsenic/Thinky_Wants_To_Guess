using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels To Manage")]
    public GameObject[] panels;

    public GameObject blocker;

    private GameObject currentOpenPanel;

    void Start()
    {
        blocker.SetActive(false);

        foreach (var p in panels)
            p.SetActive(false);
    }

    public void TogglePanel(GameObject panel)
    {
        if (currentOpenPanel == panel)
        {
            CloseAll();
            return;
        }

        CloseAll();

        panel.SetActive(true);
        blocker.SetActive(true);
        currentOpenPanel = panel;
    }

    public void CloseAll()
    {
        foreach (var p in panels)
            p.SetActive(false);

        blocker.SetActive(false);
        currentOpenPanel = null;
    }
}