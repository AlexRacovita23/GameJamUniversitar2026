using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum ItemSourceZone
{
    Inventory,
    CraftingTable
}

public class DraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countLabel;

    public ItemData Data { get; private set; }
    public ItemSourceZone SourceZone { get; private set; }
    public List<RectTransform> OtherItems { get; set; } = new();
    public RectTransform ExclusionZone { get; set; }
    public bool WasAcceptedByDropZone { get; set; } = false;
    public bool ChangedZone { get; set; } = false;

    private Transform _originalParent;
    private Vector3 _originalLocalPosition;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Init(ItemData data, int count, ItemSourceZone sourceZone)
    {
        Data = data;
        SourceZone = sourceZone;
        iconImage.sprite = data.icon;
        SetCount(count);
    }

    public void SetCount(int count)
    {
        if (countLabel != null)
            countLabel.text = count > 1 ? $"x{count}" : "";
    }

    public void OnBeginDrag(PointerEventData e)
    {
        _originalParent = transform.parent;
        _originalLocalPosition = transform.localPosition;
        WasAcceptedByDropZone = false;
        ChangedZone = false;

        Canvas rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        transform.SetParent(rootCanvas.transform, true);
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e)
    {
        transform.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        _canvasGroup.blocksRaycasts = true;

        if (WasAcceptedByDropZone && ChangedZone)
        {
            transform.SetParent(_originalParent, false);
            transform.localPosition = _originalLocalPosition;
            return;
        }

        transform.SetParent(_originalParent, true);

        RectTransform parentRect = _originalParent as RectTransform;
        if (parentRect != null)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                e.position,
                e.pressEventCamera,
                out localPos
            );

            Vector2 size = _rectTransform.sizeDelta;
            Vector2 pivot = _rectTransform.pivot;

            float minX = parentRect.rect.xMin + size.x * pivot.x;
            float maxX = parentRect.rect.xMax - size.x * (1f - pivot.x);
            float minY = parentRect.rect.yMin + size.y * pivot.y;
            float maxY = parentRect.rect.yMax - size.y * (1f - pivot.y);

            float clampedX = Mathf.Clamp(localPos.x, minX, maxX);
            float clampedY = Mathf.Clamp(localPos.y, minY, maxY);
            Vector2 clamped = new Vector2(clampedX, clampedY);

            _rectTransform.localPosition = clamped;

            if (ExclusionZone != null && LayoutUtils.Overlaps(_rectTransform, ExclusionZone))
            {
                transform.localPosition = _originalLocalPosition;
                return;
            }

            bool overlaps = false;
            foreach (var other in OtherItems)
            {
                if (other == null) continue;
                if (LayoutUtils.Overlaps(_rectTransform, other))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps)
                transform.localPosition = _originalLocalPosition;
        }
        else
        {
            transform.localPosition = _originalLocalPosition;
        }
    }
}