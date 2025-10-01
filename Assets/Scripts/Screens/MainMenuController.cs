using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    RectTransform background;
    public CanvasGroup content;
    public Button continueButton;

    [Header("Tiempos")]
    public float slideDuration = 2f;
    public float fadeDuration = 1f;

    private MetasPddDataModel dataModel;
    
    private void Start()
    {
        background = ScreenController.Instance.background;
        background.anchoredPosition = new Vector2(0, Screen.height);
        ScreenController.Instance.backButton.GetComponent<CanvasGroup>().alpha = 0;

        StartCoroutine(ShowMenu());
    }

    private void OnEnable()
    {
        content.alpha = 0;
        
        if (continueButton != null)
        {
            string savePath = Path.Combine(Application.persistentDataPath, "metas_pdd_save.json");
            Debug.Log(Application.persistentDataPath);
            continueButton.interactable = File.Exists(savePath);
        }
    }

    private void OnDisable()
    {
    }

    IEnumerator ShowMenu()
    {
        yield return TransitionManager.Instance.SlideBackgroundFullScreen(background, slideDuration);
        yield return TransitionManager.Instance.FadeCanvasGroup(content, 0, 1, fadeDuration);
    }

    public void OnStartGame()
    {
        // Initialize MetasPddDataModel
        dataModel = new MetasPddDataModel("Assets/Data/metas_pdd.json");
        dataModel.Initialize();

        ScreenController.Instance.ShowScreen(ScreenController.Instance.pantalla3Categories);
    }

    public void OnContinueGame()
    {
        // Load MetasPddDataModel from saved file
        if (MetasPddDataModel.Instance == null)
        {
            dataModel = new MetasPddDataModel("Assets/Data/metas_pdd.json");
            dataModel.LoadFromFile("metas_pdd_save.json");
        }
        else
        {
            MetasPddDataModel.Instance.LoadFromFile("metas_pdd_save.json");
        }

        ScreenController.Instance.ShowScreen(ScreenController.Instance.pantalla3Categories);
    }
}