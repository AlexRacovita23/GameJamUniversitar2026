using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countLabel;

    public ItemData Data { get; private set; }
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

    public void Init(ItemData data, int count)
    {
        Data = data;
        iconImage.sprite = data.icon;
        SetCount(count);
    }

    public void SetCount(int count)
    {
        if (countLabel != null)
            countLabel.text = $"x{count}";
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
        if (WasAcceptedByDropZone)
        {
            _canvasGroup.blocksRaycasts = true;
            return;
        }

        transform.SetParent(_originalParent);
        transform.localPosition = _originalLocalPosition;
        _canvasGroup.blocksRaycasts = true;
    }
}