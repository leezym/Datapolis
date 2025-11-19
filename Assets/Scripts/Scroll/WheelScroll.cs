using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class WheelScroll : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform viewport;
    public RectTransform content;
    public ScrollRect scrollRect;
    public VerticalLayoutGroup verticalLayoutGroup;
    public GameObject itemPrefab;


    public string currentArea;
    public string[] items;
    public SelectedMetaInfo selectedData;
    public UnityAction<SelectedMetaInfo> onItemSelected;
    private List<GameObject> itemObjects = new List<GameObject>();
    private Vector2 oldVelocity;
    private bool isUpdated;

    public void SetupItems()
    {
        // Clear existing items
        foreach (GameObject obj in itemObjects)
        {
            Destroy(obj);
        }

        itemObjects.Clear();

        if (items != null && items.Length > 0)
        {
            int itemsPerPage = Mathf.CeilToInt(viewport.rect.height / (itemPrefab.GetComponent<RectTransform>().rect.height + verticalLayoutGroup.spacing));

            // Instantiate items
            if (itemPrefab != null)
            {
                isUpdated = false;
                oldVelocity = Vector2.zero;

                for (int i = 0; i < items.Length; i++)
                {
                    GameObject itemObj = Instantiate(itemPrefab, content);

                    TMP_Text text = itemObj.GetComponentInChildren<TMP_Text>();
                    if (text != null)
                    {
                        text.text = items[i];
                    }
                    Button button = itemObj.GetComponent<Button>();
                    if (button != null)
                    {
                        int index = i; // Capture for closure
                        button.onClick.AddListener(() => OnItemClicked(index));
                    }

                    itemObjects.Add(itemObj);
                }

                for (int i = 0; i < itemsPerPage; i++)
                {
                    GameObject itemObj = Instantiate(itemPrefab, content);
                    itemObj.transform.SetAsLastSibling();

                    TMP_Text text = itemObj.GetComponentInChildren<TMP_Text>();
                    if (text != null)
                    {
                        text.text = items[i % items.Length];
                    }
                    Button button = itemObj.GetComponent<Button>();
                    if (button != null)
                    {
                        int index = i % items.Length; // Capture for closure
                        button.onClick.AddListener(() => OnItemClicked(index));
                    }

                    itemObjects.Add(itemObj);
                }
            }
            else
            {
                Debug.LogError("WheelScroll: Prefab not assigned.");
            }

            // Force layout update
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
        else
        {
            Debug.LogError("WheelScroll: Items not assigned.");
        }

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, (itemPrefab.GetComponent<RectTransform>().rect.height + verticalLayoutGroup.spacing) * items.Length);
    }

    private void OnItemClicked(int index)
    {
        string selectedNumeracion = items[index];

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

            // Trigger event
            onItemSelected?.Invoke(selectedData);
        }
    }

    void Update()
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = oldVelocity;
        }
       
        if (content.anchoredPosition.y < 0)
        {
            Canvas.ForceUpdateCanvases();
            oldVelocity = scrollRect.velocity;
            content.anchoredPosition += new Vector2(0, (itemPrefab.GetComponent<RectTransform>().rect.height + verticalLayoutGroup.spacing) * items.Length);
            isUpdated = true;
        }

        if (content.anchoredPosition.y > (itemPrefab.GetComponent<RectTransform>().rect.height + verticalLayoutGroup.spacing) * items.Length)
        {
            Canvas.ForceUpdateCanvases();
            oldVelocity = scrollRect.velocity;
            content.anchoredPosition -= new Vector2(0, (itemPrefab.GetComponent<RectTransform>().rect.height + verticalLayoutGroup.spacing) * items.Length);
            isUpdated = true;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
