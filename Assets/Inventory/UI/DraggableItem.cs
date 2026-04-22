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
    public bool WasAcceptedByDropZone { get; set; } = false;

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

        if (WasAcceptedByDropZone)
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

            Vector2 half = _rectTransform.sizeDelta * 0.5f;
            float clampedX = Mathf.Clamp(localPos.x, parentRect.rect.xMin + half.x, parentRect.rect.xMax - half.x);
            float clampedY = Mathf.Clamp(localPos.y, parentRect.rect.yMin + half.y, parentRect.rect.yMax - half.y);
            Vector2 clamped = new Vector2(clampedX, clampedY);

            _rectTransform.localPosition = clamped;

            bool overlaps = false;
            foreach (var other in OtherItems)
            {
                if (other == null) continue;
                if (RectOverlapChecker.Overlaps(_rectTransform, other))
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
