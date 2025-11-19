using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class AboutMenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public CanvasGroup content;
    public GameObject left;
    public CanvasGroup right;
    public string[] textos;
    public GameObject images;

    [Header("Tiempos")]
    public float slideDuration = 2f;
    public float fadeDuration = 1f;

    int index;

    private void OnEnable()
    {
        content.alpha = 0;
        left.SetActive(false);
        right.alpha = 0;
        content.gameObject.SetActive(true);
        images.SetActive(false);

        index = 0;

        ScreenController.Instance.backButton.GetComponent<CanvasGroup>().alpha = 0;
        ScreenController.Instance.exitButton.GetComponent<CanvasGroup>().alpha = 1;

        content.gameObject.GetComponent<TMP_Text>().text = textos[index];

        StartCoroutine(ShowInfo());
    }

    IEnumerator ShowInfo()
    {
        yield return TransitionManager.Instance.FadeCanvasGroup(content, 0, 1, fadeDuration);
        yield return TransitionManager.Instance.FadeCanvasGroup(right, 0, 1, fadeDuration);
    }

    public void Left()
    {
        index--;
        content.gameObject.GetComponent<TMP_Text>().text = textos?[index];
        content.gameObject.SetActive(true);
        images.SetActive(false);
        right.gameObject.SetActive(true);
        
        if (index <= 0)
        {
            left.SetActive(false);
        }
        else
        {
            left.SetActive(true);
        }

    }
    public void Right()
    {        
        index++;
        left.SetActive(true);

        if (index >= textos.Length)
        {
            content.gameObject.SetActive(false);
            images.SetActive(true);
            right.gameObject.SetActive(false);
        }
        else
        {
            content.gameObject.GetComponent<TMP_Text>().text = textos?[index];
            content.gameObject.SetActive(true);
            images.SetActive(false);
            right.gameObject.SetActive(true);
        }
    }
}
