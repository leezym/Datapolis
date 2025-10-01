using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WheelScroll : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform content;
    public RectTransform viewport;
    public ScrollRect scrollRect;
    public TMP_Text textComponent;
    public string[] items;
    public string currentArea;
    public SelectedMetaInfo selectedData;

    [Header("Parámetros de giro")]
    public float spinAmount;
    public float decelerationSpin;
    public float initialSpeed;
    public AnimationCurve decelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    // Cálculo automático
    private int itemCount;
    private float preferredHeight;

    public void ResetToFirstItem()
    {
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
    }

    public IEnumerator PlayScroll()
    {
        if (textComponent != null && items != null && items.Length > 0)
        {
            itemCount = items.Length;

            string header = items[itemCount - 2] + "\n\n\n" + items[itemCount - 1] + "\n\n\n";
            string body = string.Join("\n\n\n", items);
            string footer = "\n\n\n" + items[0] + "\n\n\n" + items[1];

            textComponent.text = header + body + footer;

            // Ajustar tamaño del content usando preferredHeight
            preferredHeight = textComponent.preferredHeight;
            content.sizeDelta = new Vector2(content.sizeDelta.x, preferredHeight);

            // Forzar actualización del layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            // Posicionar el borde superior del content alineado al borde superior del viewport
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
        }
        else
        {
            Debug.LogError("WheelScroll: TMP_Text o items no asignados.");
            yield break;
        }

        if (scrollRect != null) scrollRect.enabled = false;
        yield return SpinCoroutine();
    }

    private IEnumerator SpinCoroutine()
    {
        // 1. Elegir un selectedItem al azar
        int randomIndex = Random.Range(0, itemCount);
        string selectedNumeracion = items[randomIndex];

        MetaData foundMeta = null;
        if (MetasPddDataModel.Instance != null)
        {
            List<AreaData> areas = MetasPddDataModel.Instance.GetData();
            AreaData areaData = areas.Find(a => a.area.ToLower() == currentArea.ToLower());
            if (areaData != null)
            {
                foundMeta = areaData.datos.Find(m => m.numeracion == selectedNumeracion);
            }
        }

        if (foundMeta != null)
        {
            selectedData = new SelectedMetaInfo
            {
                area = currentArea,
                numeracion = selectedNumeracion,
                meta = foundMeta.meta,
                cumplimiento = foundMeta.cumplimiento,
                responsable = foundMeta.responsable
            };

            // Delete the meta from the data model
            MetasPddDataModel.Instance.DeleteMeta(selectedNumeracion);
        }

        // 2. Calcular posItem
        float posItem = (preferredHeight - viewport.rect.height) * randomIndex / (itemCount - 1);

        // 3. Configurar giro
        float totalTravel = ((spinAmount + decelerationSpin) * preferredHeight) + posItem;
        float phase1Distance = spinAmount * preferredHeight;
        float phase2Distance = decelerationSpin * preferredHeight;
        float currentTravel = 0f;

        while (currentTravel < totalTravel)
        {                
            // Determinar velocidad 
            float speed;
            if (currentTravel < phase1Distance)
            {
                // Velocidad constante durante spinAmount mínimo
                speed = initialSpeed;
            }
            else
            {
                // Deceleración usando la curva
                float progress = Mathf.Clamp01((currentTravel - phase1Distance) / phase2Distance);
                speed = initialSpeed * decelerationCurve.Evaluate(progress);
            }

            // Calcular desplazamiento
            float deltaTravel = speed * Time.deltaTime;
            deltaTravel = Mathf.Min(deltaTravel, totalTravel - currentTravel);
            currentTravel += deltaTravel;

            content.anchoredPosition = new Vector2(content.anchoredPosition.x, currentTravel % preferredHeight);

            yield return null;
        }

        // Rehabilitar ScrollRect
        if (scrollRect != null) scrollRect.enabled = true;
    }
    
    private void OnDisable() {
        StopAllCoroutines();
    }
}
