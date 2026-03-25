using UnityEngine;
using UnityEngine.UI;

public class InventoryDragManager : MonoBehaviour
{
    public static InventoryDragManager Instance { get; private set; }

    [Header("Drag Visual")]
    [SerializeField] private Image dragIcon;
    [SerializeField] private Canvas rootCanvas;

    private InventorySlotUI sourceSlotUI;
    private bool isDragging;

    public InventorySlotUI SourceSlotUI => sourceSlotUI;
    public bool IsDragging => isDragging;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dragIcon != null)
            dragIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isDragging || dragIcon == null || rootCanvas == null)
            return;

        RectTransform canvasRect = rootCanvas.transform as RectTransform;
        RectTransform iconRect = dragIcon.rectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            rootCanvas.worldCamera,
            out localPoint
        );

        iconRect.localPosition = localPoint;
    }

    public void BeginDrag(InventorySlotUI slotUI)
    {
        if (slotUI == null || slotUI.CurrentSlot == null || slotUI.CurrentSlot.IsEmpty())
            return;

        sourceSlotUI = slotUI;
        isDragging = true;

        if (dragIcon != null)
        {
            dragIcon.sprite = slotUI.CurrentSlot.item.itemData.icon;
            dragIcon.enabled = true;
            dragIcon.gameObject.SetActive(true);
        }
    }

    public void EndDrag()
    {
        isDragging = false;
        sourceSlotUI = null;

        if (dragIcon != null)
        {
            dragIcon.sprite = null;
            dragIcon.enabled = false;
            dragIcon.gameObject.SetActive(false);
        }
    }
}