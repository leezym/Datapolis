using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

[Serializable]
public class TableSprites
{
    public Sprite[] sprites;
}

[DefaultExecutionOrder(0)]
public class CategoriesDetailMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup content;
    public RectTransform selectedButton;
    public Sprite[] spriteCategoriasDetail;
    public RectTransform grid;
    public Image[] selectedTable;
    public TableSprites[] spriteTableCategoriesDetail;
    public TMP_Text area;
    public TMP_Text responsable;
    public TMP_Text meta;
    public TMP_Text cumplimiento;

    [Header("Tiempos")]
    public float slideDuration = 2f;
    public float scrollDuration = 3f;
    public float expandDuration = 0.6f;
    public float fadeDuration = 1f;

    private void OnEnable()
    {
        content.alpha = 0;
        ScreenController.Instance.backButton.GetComponent<CanvasGroup>().alpha = 1;
        ScreenController.Instance.exitButton.GetComponent<CanvasGroup>().alpha = 1;

        ScreenController.Instance.backButton.GetComponent<Button>().onClick.AddListener(() => ScreenController.Instance.ShowScreen(ScreenController.Instance.pantallaCategories));
        
        StartCoroutine(ShowDetail());
    }

    private void OnDisable()
    {
        ScreenController.Instance.backButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void SetDetailTexts(SelectedMetaInfo data)
    {
        if (area != null) area.text = data.area;
        if (responsable != null) responsable.text = data.responsable;
        if (meta != null) meta.text = data.meta;
        if (cumplimiento != null) cumplimiento.text = data.cumplimiento;
    }

    public void MatchStartPosition(RectTransform boton, RectTransform rtTextButton, string text, string number, int index, float width)
    {
        Sprite spriteButton = spriteCategoriasDetail[index];

        selectedButton.anchoredPosition = boton.anchoredPosition;
        selectedButton.sizeDelta = boton.sizeDelta;

        Image imgSelectedButton = selectedButton.GetComponent<Image>();
        TMP_Text[] textSelectedButton = selectedButton.GetComponentsInChildren<TMP_Text>();
        RectTransform rtTextSelectedButton = textSelectedButton[0].GetComponent<RectTransform>();
        RectTransform rtContentSelectedButton = textSelectedButton[1].GetComponent<RectTransform>();

        if (imgSelectedButton != null && spriteButton != null && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(number))
        {
            textSelectedButton[0].text = text;
            textSelectedButton[1].text = number;
            imgSelectedButton.sprite = spriteButton;

            float aspectRatio = spriteButton.rect.height / spriteButton.rect.width;
            float height = width * aspectRatio;

            selectedButton.sizeDelta = new Vector2(width, height);

            rtTextSelectedButton.sizeDelta = new Vector2(rtTextButton.rect.width, rtTextButton.rect.height);

            var textBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(selectedButton, rtTextSelectedButton);
            float textTopY = textBounds.max.y;
            float textBottomY = textBounds.min.y;
            float textMidY = (textTopY + textBottomY) / 2f;

            float topY = (1f - selectedButton.pivot.y) * height;
            float bottomY = -selectedButton.pivot.y * height;

            float topOffsetFromParentTop = topY - textMidY;
            rtContentSelectedButton.anchoredPosition = new Vector2(0f, -topOffsetFromParentTop);

            float maxHeight = textMidY - bottomY;
            if (maxHeight < 0f) maxHeight = 0f;

            rtContentSelectedButton.sizeDelta = new Vector2(rtContentSelectedButton.sizeDelta.x, maxHeight);
        }

        if (grid != null)
        {
            grid.anchoredPosition = new Vector2(grid.anchoredPosition.x, selectedButton.anchoredPosition.y - selectedButton.sizeDelta.y - 50);

            Vector3 backButtonWorld = ScreenController.Instance.backButton.transform.position;
            Vector3 backButtonLocal = grid.parent.InverseTransformPoint(backButtonWorld);

            Vector3 selectedButtonWorldBottom = selectedButton.position;
            Vector3 selectedButtonLocalBottom = grid.parent.InverseTransformPoint(selectedButtonWorldBottom);

            float top = selectedButtonLocalBottom.y - selectedButton.sizeDelta.y - 50;
            float bottom = backButtonLocal.y + ScreenController.Instance.backButton.GetComponent<RectTransform>().sizeDelta.y;
            float availableHeight = top - bottom;

            grid.sizeDelta = new Vector2(grid.sizeDelta.x, availableHeight);
        }

        for (int i = 0; i < selectedTable.Length; i++)
        {
            selectedTable[i].sprite = spriteTableCategoriesDetail[index].sprites[i];            
        }
    }

    IEnumerator ShowDetail()
    {
        yield return TransitionManager.Instance.FadeCanvasGroup(content, 0, 1, fadeDuration);
    }
}