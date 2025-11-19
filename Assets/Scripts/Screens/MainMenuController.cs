using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using Unity.Mathematics;

[DefaultExecutionOrder(0)]
public class MainMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup content;
    public Button continueButton;

    [Header("Tiempos")]
    public float slideDuration = 2f;
    public float fadeDuration = 1f;

    private MetasPddDataModel dataModel;

    private void OnEnable()
    {
        content.alpha = 0;

        ScreenController.Instance.backButton.GetComponent<CanvasGroup>().alpha = 0;
        ScreenController.Instance.exitButton.GetComponent<CanvasGroup>().alpha = 0;

        if (TransitionManager.isFirstBackgroundSlide) {
            ScreenController.Instance.background.anchoredPosition = new Vector2(0, Screen.height);
        }

        if (continueButton != null)
        {
            string savePath = Path.Combine(Application.persistentDataPath, "metas_pdd_save.json");
            continueButton.interactable = File.Exists(savePath);
        }
        
        StartCoroutine(ShowMenu());
    }

    IEnumerator ShowMenu()
    {
        yield return TransitionManager.Instance.SlideBackgroundFullScreen(ScreenController.Instance.background, slideDuration);
        yield return TransitionManager.Instance.FadeCanvasGroup(content, 0, 1, fadeDuration);
    }

    public void OnStartGame()
    {
        // Initialize MetasPddDataModel
        dataModel = new MetasPddDataModel(Path.Combine(Application.persistentDataPath, "metas_pdd.json"));
        dataModel.Initialize();

        ScreenController.Instance.ShowScreen(ScreenController.Instance.pantallaCategories);
    }

    public void OnContinueGame()
    {
        // Load MetasPddDataModel from saved file
        if (MetasPddDataModel.Instance == null)
        {
            dataModel = new MetasPddDataModel(Path.Combine(Application.persistentDataPath, "metas_pdd.json"));
            dataModel.LoadFromFile("metas_pdd_save.json");
        }
        else
        {
            MetasPddDataModel.Instance.LoadFromFile("metas_pdd_save.json");
        }

        ScreenController.Instance.ShowScreen(ScreenController.Instance.pantallaCategories);
    }
}