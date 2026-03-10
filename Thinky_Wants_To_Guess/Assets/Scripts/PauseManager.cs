using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public GameObject pausePopupPrefab;

    private GameObject currentPopup;
    private bool isOpen = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsMenuScene())
                return;

            TogglePopup();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClosePopup();
    }

    bool IsMenuScene()
    {
        return SceneManager.GetActiveScene().name == "MainMenu";
    }

    public void TogglePopup()
    {
        if (isOpen)
            ClosePopup();
        else
            OpenPopup();
    }

    void OpenPopup()
    {
        if (currentPopup != null) return;

        if (pausePopupPrefab == null) return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        currentPopup = Instantiate(pausePopupPrefab, canvas.transform);

        isOpen = true;
    }

    public void ClosePopup()
    {
        if (currentPopup != null)
            Destroy(currentPopup);

        currentPopup = null;
        isOpen = false;
    }

    public void GoToMenu()
    {
        ClosePopup();
        SceneManager.LoadScene("MainMenu");
    }
}