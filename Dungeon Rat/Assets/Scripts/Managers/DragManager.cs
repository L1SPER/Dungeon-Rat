using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance { get; private set; }

    [Header("Drag Visual")]
    [SerializeField] private Image dragIcon;
    [SerializeField] private Canvas rootCanvas;

    [Header("Optional")]
    [SerializeField] private bool dontDestroyOnLoad = false;
    [SerializeField] private Vector2 iconOffset = Vector2.zero;

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

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        ValidateReferences();
        HideDragIconImmediate();
    }

    private void Update()
    {
        if (!isDragging)
            return;

        UpdateDragIconPosition();
    }

    private void ValidateReferences()
    {
        if (dragIcon == null)
        {
            Debug.LogWarning("[DragManager] dragIcon referansı atanmadı.");
        }

        if (rootCanvas == null)
        {
            Debug.LogWarning("[DragManager] rootCanvas referansı atanmadı.");
        }

        if (dragIcon != null)
        {
            dragIcon.raycastTarget = false;
            dragIcon.preserveAspect = true;
            dragIcon.enabled = false;
        }
    }

    private void UpdateDragIconPosition()
    {
        if (dragIcon == null || rootCanvas == null)
            return;

        RectTransform canvasRect = rootCanvas.transform as RectTransform;
        RectTransform iconRect = dragIcon.rectTransform;

        if (canvasRect == null || iconRect == null)
            return;

        Camera cam = null;

        if (rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = rootCanvas.worldCamera;

        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            cam,
            out Vector2 localPoint
        );

        if (!success)
            return;

        iconRect.localPosition = localPoint + iconOffset;
    }

    public void BeginDrag(InventorySlotUI slotUI)
    {
        if (slotUI == null)
            return;

        if (slotUI.CurrentSlot == null || slotUI.CurrentSlot.IsEmpty())
            return;

        if (dragIcon == null || rootCanvas == null)
        {
            Debug.LogWarning("[DragManager] Drag başlatılamadı. dragIcon veya rootCanvas eksik.");
            return;
        }

        Sprite iconSprite = slotUI.CurrentSlot.item?.itemData?.icon;

        if (iconSprite == null)
        {
            Debug.LogWarning("[DragManager] Taşınacak item için icon bulunamadı.");
            return;
        }

        sourceSlotUI = slotUI;
        isDragging = true;

        dragIcon.sprite = iconSprite;
        dragIcon.preserveAspect = true;
        dragIcon.gameObject.SetActive(true);
        dragIcon.enabled = true;

        dragIcon.rectTransform.sizeDelta = new Vector2(100f, 100f);

        UpdateDragIconPosition();
    }

    public void CompleteDrag()
    {
        isDragging = false;
        sourceSlotUI = null;

        HideDragIconImmediate();
    }

    private void HideDragIconImmediate()
    {
        if (dragIcon == null)
            return;

        dragIcon.sprite = null;
        dragIcon.enabled = false;
        dragIcon.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            CompleteDrag();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            if (isDragging)
                CompleteDrag();

            Instance = null;
        }
    }
}