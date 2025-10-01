using UnityEngine;
using UnityEngine.UI;

public class ScreenController : MonoBehaviour
{
    public static ScreenController Instance;

    [Header("Referencias a Pantallas")]
    public RectTransform background;
    public GameObject pantalla2MainMenu;
    public GameObject pantalla3Categories;
    public GameObject pantalla4Scroll;
    public GameObject pantalla5Detail;

    [Header("Referencias a Botones")]
    public Button homeButton;
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
        ShowScreen(pantalla2MainMenu);
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