using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class ScreenController : MonoBehaviour
{
    public static ScreenController Instance;

    [Header("Referencias a Pantallas")]
    public RectTransform background;
    public GameObject pantallaMainMenu;
    public GameObject pantallaCategories;
    public GameObject pantallaScroll;
    public GameObject pantallaDetail;

    [Header("Referencias a Botones")]
    public Button backButton;
    public Button exitButton;

    private GameObject currentScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ShowScreen(pantallaMainMenu);

        exitButton.onClick.AddListener(() =>
        {
            SaveJsonFile();
            ShowScreen(pantallaMainMenu);
        });
    }

    public void ShowScreen(GameObject screen)
    {
        if (currentScreen != null)
            currentScreen.SetActive(false);

        currentScreen = screen;
        currentScreen.SetActive(true);
    }

    public void SaveJsonFile()
    { 
        // Save MetasPddDataModel to file
        if (MetasPddDataModel.Instance != null)
        {
            MetasPddDataModel.Instance.SaveToFile("metas_pdd_save.json");
        }
    }
    public void OnExitGame()
    {
        SaveJsonFile();
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SaveJsonFile();
    }
}