using UnityEngine;
using System.Collections;
using System;

[DefaultExecutionOrder(-100)]
public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    public static bool isFirstBackgroundSlide = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // -----------------------
    // Fade
    // -----------------------
    public IEnumerator FadeCanvasGroup(CanvasGroup canvas, float from, float to, float duration)
    {
        float elapsed = 0f;
        canvas.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvas.alpha = to;
    }

    // -----------------------
    // Slide RectTransform
    // -----------------------
    public IEnumerator SlideRect(RectTransform rt, Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;
        rt.anchoredPosition = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        rt.anchoredPosition = to;
    }

    // -----------------------
    // Slide background pantalla completa
    // -----------------------
    public IEnumerator SlideBackgroundFullScreen(RectTransform background, float duration)
    {
        // Obtener canvas padre para conocer el scaleFactor
        Canvas canvas = background.GetComponentInParent<Canvas>();
        float canvasScale = (canvas != null) ? canvas.scaleFactor : 1f;

        // Forzar actualización de layout para que rect.height y demás estén correctos
        Canvas.ForceUpdateCanvases();

        // --- Ajustar tamaño proporcional al ancho de pantalla (convertido a unidades de canvas) ---
        Sprite fondoSprite = background.GetComponent<UnityEngine.UI.Image>()?.sprite;

        if (fondoSprite != null)
        {
            float spriteWidth = fondoSprite.rect.width;
            float spriteHeight = fondoSprite.rect.height;
            float aspectRatio = spriteHeight / spriteWidth;

            // newHeight en píxeles de pantalla
            float newHeightPx = Screen.width * aspectRatio;

            // convertir a unidades de canvas (RectTransform usa unidades del canvas)
            float newHeightCanvas = newHeightPx / canvasScale;

            // Si el background está stretch horizontal, mantenemos x=0 y cambiamos height
            Vector2 sd = background.sizeDelta;
            sd.y = newHeightCanvas;
            sd.x = 0; // si quieres mantener stretch horizontal
            background.sizeDelta = sd;
        }

        // Forzar otra actualización por si sizeDelta cambió el rect
        Canvas.ForceUpdateCanvases();

        // Set pivot and anchors for correct animation
        background.pivot = new Vector2(0.5f, 0.5f);
        background.anchorMin = new Vector2(0, 0.5f);
        background.anchorMax = new Vector2(1, 0.5f);

        // --- Calcular posiciones en UNIDADES DE CANVAS ---
        float screenHeightCanvas = Screen.height / canvasScale;
        float fondoHeight = background.sizeDelta.y; // ya está en unidades de canvas

        // start: borde inferior del background en borde inferior de la pantalla
        float startY = (-screenHeightCanvas / 2f) + (fondoHeight / 2f);

        // target: borde superior del background en borde superior de la pantalla
        float targetY = (screenHeightCanvas / 2f) - (fondoHeight / 2f);

        Vector2 fromPos;
        if (isFirstBackgroundSlide) {
            fromPos = new Vector2(0, startY);
            isFirstBackgroundSlide = false;
        } else {
            fromPos = new Vector2(0, background.anchoredPosition.y);
        }
        
        // --- Animación ---
        yield return SlideRect(background, fromPos, new Vector2(0, targetY), duration);
    }
}