using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class SelectedMetaInfo
{
    public string area;
    public string numeracion;
    public string meta;
    public string cumplimiento;
    public string responsable;
}

public class CategoriesScrollMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup content;
    public CanvasGroup scrollView;
    public RectTransform selectedButton;
    public RectTransform selectedScroll;
    public Sprite[] spriteCategoriasScroll;
    public WheelScroll wheelScroll;

    [Header("Pantalla Categories Detail")]
    public CategoriesDetailMenuController categoriesDetailMenu;

    [Header("Tiempos")]
    public float slideDuration = 2f;
    public float scrollDuration = 3f;
    public float expandDuration = 0.6f;
    public float fadeDuration = 1f;

    public int indexButton;
    private string currentTextButton;
    public SelectedMetaInfo selectedData;

    private void OnEnable()
    {
        content.alpha = 0;
        scrollView.alpha = 0;

        ScreenController.Instance.homeButton.GetComponent<CanvasGroup>().alpha = 1;
        ScreenController.Instance.backButton.GetComponent<CanvasGroup>().alpha = 1;
        ScreenController.Instance.backButton.GetComponent<Button>().onClick.AddListener(() => ScreenController.Instance.ShowScreen(ScreenController.Instance.pantalla3Categories));
        
        if (wheelScroll != null)
        {
            wheelScroll.ResetToFirstItem();
        }
        StartCoroutine(ShowScroll());
    }

    private void OnDisable()
    {
        ScreenController.Instance.backButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public IEnumerator ShowScroll()
    {
        // Fade in scroll
        StartCoroutine(TransitionManager.Instance.FadeCanvasGroup(scrollView, 0, 1, fadeDuration));
        yield return TransitionManager.Instance.FadeCanvasGroup(content, 0, 1, fadeDuration);

        if (wheelScroll != null && !string.IsNullOrEmpty(currentTextButton) && MetasPddDataModel.Instance != null)
        {
            List<AreaData> areas = MetasPddDataModel.Instance.GetData();
            AreaData matchingArea = areas.FirstOrDefault(a => a.area.ToLower() == currentTextButton.ToLower());
            if (matchingArea != null)
            {
                string[] items = matchingArea.datos.Select(d => d.numeracion).ToArray();
                wheelScroll.items = items;
                wheelScroll.currentArea = currentTextButton.FirstCharacterToUpper();
            }

            yield return wheelScroll.PlayScroll();
        }

        selectedData = wheelScroll.selectedData;
        categoriesDetailMenu.SetDetailTexts(selectedData);

        RectTransform rt = selectedButton.GetComponent<RectTransform>();
        Sprite sprite = selectedButton.GetComponent<Image>().sprite;
        TMP_Text text = selectedButton.GetComponentInChildren<TMP_Text>();
        RectTransform rtText = text.GetComponent<RectTransform>();

        float aspectRatio = sprite.rect.height / sprite.rect.width;
        float width = rt.rect.height / aspectRatio;

        categoriesDetailMenu.MatchStartPosition(rt, rtText, text.text, selectedData.numeracion, indexButton, width);

        yield return new WaitForSeconds(1.5f);

        ScreenController.Instance.ShowScreen(ScreenController.Instance.pantalla5Detail);
    }

    public void MatchStartPosition(RectTransform boton, Sprite spriteButton, string textButton, int index, float width)
    {
        indexButton = index;
        currentTextButton = textButton;

        Sprite spriteScroll = spriteCategoriasScroll[indexButton];

        selectedButton.anchoredPosition = boton.anchoredPosition;
        selectedButton.sizeDelta = boton.sizeDelta;

        selectedScroll.sizeDelta = boton.sizeDelta;

        Image imgSelectedButton = selectedButton.GetComponent<Image>();
        TMP_Text textSelectedButton = selectedButton.GetComponentInChildren<TMP_Text>();
        Image imgSelectedScroll = selectedScroll.GetComponent<Image>();

        if (imgSelectedButton != null && spriteButton != null && !string.IsNullOrEmpty(textButton))
        {
            textSelectedButton.text = textButton;
            imgSelectedButton.sprite = spriteButton;

            float aspectRatio = spriteButton.rect.height / spriteButton.rect.width;
            float height = width * aspectRatio;

            selectedButton.sizeDelta = new Vector2(width, height);

            selectedScroll.anchoredPosition = boton.anchoredPosition - new Vector2(0, height - 50);
        }

        if (imgSelectedScroll != null && spriteScroll != null)
        {
            imgSelectedScroll.sprite = spriteScroll;

            Vector3 backButtonWorld = ScreenController.Instance.backButton.transform.position;
            Vector3 backButtonLocal = selectedScroll.parent.InverseTransformPoint(backButtonWorld);

            Vector3 scrollWorldTop = selectedScroll.position;
            Vector3 scrollLocalTop = selectedScroll.parent.InverseTransformPoint(scrollWorldTop);

            float availableHeight = scrollLocalTop.y - backButtonLocal.y;

            selectedScroll.sizeDelta = new Vector2(width, availableHeight);
        }
    }
}