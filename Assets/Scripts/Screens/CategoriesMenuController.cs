using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

[DefaultExecutionOrder(0)]
public class CategoriesMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup content;
    public CanvasGroup text;
    public Button[] botonesCategorias;

    [Header("Pantalla Categories Scroll")]
    public CategoriesScrollMenuController categoriesScrollMenu;

    [Header("Tiempos")]
    public float slideDuration = 2f;
    public float fadeDuration = 1f;

    private void OnEnable()
    {
        content.alpha = 0;
        text.alpha = 1;

        ScreenController.Instance.backButton.GetComponent<CanvasGroup>().alpha = 0;
        ScreenController.Instance.exitButton.GetComponent<CanvasGroup>().alpha = 1;        

        foreach (Button b in botonesCategorias)
        {
            b.GetComponent<CanvasGroup>().alpha = 1;
            b.onClick.AddListener(() => OnCategoriaSeleccionada(b));
        }

        StartCoroutine(ShowCategorias());
    }

    private void OnDisable()
    {
        foreach (Button b in botonesCategorias)
            b.onClick.RemoveAllListeners();
    }

    IEnumerator ShowCategorias()
    {
        yield return TransitionManager.Instance.SlideRect(ScreenController.Instance.background, ScreenController.Instance.background.anchoredPosition, Vector2.zero, slideDuration);
        yield return TransitionManager.Instance.FadeCanvasGroup(content, 0, 1, fadeDuration);
    }

    public void OnCategoriaSeleccionada(Button button)
    {
        StartCoroutine(MoveCategories(button));
    }

    IEnumerator MoveCategories(Button button)
    {
        Button buttonScroll = null;
        int indexScroll = -1;
        foreach (var (b, index) in botonesCategorias.Select((b, index) => (b, index)))
        {
            if (b != button)
                StartCoroutine(TransitionManager.Instance.FadeCanvasGroup(b.GetComponent<CanvasGroup>(), 1, 0, fadeDuration));
            else
            {
                buttonScroll = b;
                indexScroll = index;
            }
        }
        if (buttonScroll == null) yield break;

        yield return TransitionManager.Instance.FadeCanvasGroup(text, 1, 0, fadeDuration);

        CanvasGroup cg = buttonScroll.GetComponent<CanvasGroup>();
        RectTransform rt = buttonScroll.GetComponent<RectTransform>();
        Sprite sprite = buttonScroll.GetComponent<Image>().sprite;

        float aspectRatio = sprite.rect.height / sprite.rect.width;
        float width = rt.rect.height / aspectRatio;

        Vector2 startPos = rt.anchoredPosition;

        Vector3 exitButtonWorld = ScreenController.Instance.exitButton.transform.position;
        Vector3 backButtonLocal = rt.parent.InverseTransformPoint(exitButtonWorld);

        Vector3 scrollWorldTop = rt.position;
        Vector3 scrollLocalTop = rt.parent.InverseTransformPoint(scrollWorldTop);

        Vector2 endPos =  new Vector2(startPos.x, startPos.y + backButtonLocal.y - scrollLocalTop.y);

        StartCoroutine(TransitionManager.Instance.SlideRect(ScreenController.Instance.background, ScreenController.Instance.background.anchoredPosition, new Vector2(0, Screen.height / 2), slideDuration));
        yield return TransitionManager.Instance.SlideRect(rt, startPos, endPos, slideDuration);

        categoriesScrollMenu.MatchStartPosition(rt, sprite, buttonScroll.GetComponentInChildren<TMP_Text>().text, indexScroll, width);

        ScreenController.Instance.ShowScreen(ScreenController.Instance.pantallaScroll);
    }
}