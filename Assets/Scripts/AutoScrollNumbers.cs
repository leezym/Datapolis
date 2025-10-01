using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Solo si usas escenas

public class AutoScrollNumbers : MonoBehaviour
{
    [Header("Referencias UI")]
    public ScrollRect scrollRect;
    public GameObject itemPrefab;
    public Transform content;

    [Header("Configuración")]
    public int numberCount = 50;
    public float scrollSpeed = 0.3f;
    private readonly float scrollDuration = 3f; // fijo en 3 segundos
    public float decelerationRate = 0.95f;

    [Header("Efectos")]
    public Color highlightColor = Color.yellow;

    private Coroutine scrollCoroutine;

    private void Start()
    {
        FillWithRandomNumbers();
        scrollCoroutine = StartCoroutine(AutoScrollThenStop());
    }

    void FillWithRandomNumbers()
    {
        for (int i = 0; i < numberCount; i++)
        {
            GameObject item = Instantiate(itemPrefab, content);
            int randomNumber = Random.Range(0, 100);
            item.GetComponentInChildren<Text>().text = randomNumber.ToString();
        }
    }

    IEnumerator AutoScrollThenStop()
    {
        float elapsed = 0f;
        float currentSpeed = scrollSpeed;

        // 🔹 Scroll durante 3 segundos exactos
        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            scrollRect.verticalNormalizedPosition -= currentSpeed * Time.deltaTime;

            if (scrollRect.verticalNormalizedPosition <= 0)
                scrollRect.verticalNormalizedPosition = 1;

            yield return null;
        }

        // 🔹 Desaceleración suave
        while (currentSpeed > 0.001f)
        {
            currentSpeed *= decelerationRate;
            scrollRect.verticalNormalizedPosition -= currentSpeed * Time.deltaTime;
            yield return null;
        }

        // 🔹 Centrar en el ítem más cercano
        yield return StartCoroutine(CenterOnClosestItem());
    }

    IEnumerator CenterOnClosestItem()
    {
        RectTransform viewport = scrollRect.viewport;
        RectTransform contentRect = content.GetComponent<RectTransform>();

        float viewportCenter = viewport.rect.height / 2f;
        float closestDistance = Mathf.Infinity;
        RectTransform closestItem = null;

        foreach (Transform child in content)
        {
            RectTransform item = child as RectTransform;
            Vector3 worldPos = item.position;
            Vector3 localPos = viewport.InverseTransformPoint(worldPos);

            float distance = Mathf.Abs(localPos.y + (item.rect.height / 2) - viewportCenter);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        if (closestItem != null)
        {
            // Animación de centrado
            float duration = 0.5f;
            float elapsed = 0f;

            Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
            Vector2 childLocalPosition = closestItem.localPosition;
            Vector2 result = new Vector2(
                0,
                -(childLocalPosition.y) - (closestItem.rect.height / 2) - viewportLocalPosition.y
            );

            float normalizedPos = 1 - (result.y / (contentRect.rect.height - scrollRect.viewport.rect.height));
            float startPos = scrollRect.verticalNormalizedPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, normalizedPos, elapsed / duration);
                yield return null;
            }

            scrollRect.verticalNormalizedPosition = normalizedPos;

            // Resalta el número centrado
            HighlightItem(closestItem);

            // ✅ Ahora sí → transición automática
            GoToNextScreen();
        }
    }

    void HighlightItem(RectTransform item)
    {
        Text txt = item.GetComponentInChildren<Text>();
        if (txt != null)
        {
            txt.color = highlightColor;
            txt.fontStyle = FontStyle.Bold;
            txt.fontSize += 4;
        }
    }

    void GoToNextScreen()
    {
        Debug.Log("👉 Transición automática a la siguiente pantalla.");

        // Ejemplo con SceneManager:
        // SceneManager.LoadScene("PantallaSiguiente");

        // O si usas tu propio sistema de transiciones, lo llamas aquí.
    }
}