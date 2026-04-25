using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class GridInventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject countContainer;
    [SerializeField] private Button consumeButton;

    [Header("Configuration")]
    [SerializeField] private ItemData assignedItem;

    private int _currentCount;

    public ItemData AssignedItem => assignedItem;
    public int CurrentCount => _currentCount;

    public event Action<ItemData> OnConsumeClicked;
    public event Action<GridInventorySlot> OnSlotRightClicked;

    private void Awake()
    {
        if (consumeButton != null)
        {
            consumeButton.onClick.AddListener(HandleConsumeClicked);
            consumeButton.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        if (assignedItem != null && itemIcon != null)
        {
            itemIcon.sprite = assignedItem.Icon;
        }

        UpdateDisplay(0);
    }

    public void UpdateDisplay(int count)
    {
        _currentCount = count;

        bool hasItem = count > 0;

        if (itemIcon != null)
        {
            itemIcon.enabled = hasItem;
        }

        if (countContainer != null)
        {
            countContainer.SetActive(count > 1);
        }

        if (countText != null && count > 1)
        {
            countText.text = count.ToString();
        }

        if (!hasItem && consumeButton != null)
        {
            consumeButton.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _currentCount > 0)
        {
            OnSlotRightClicked?.Invoke(this);
            ShowConsumeButton();
        }
    }

    public void ShowConsumeButton()
    {
        if (consumeButton != null)
        {
            consumeButton.gameObject.SetActive(true);
        }
    }

    public void HideConsumeButton()
    {
        if (consumeButton != null)
        {
            consumeButton.gameObject.SetActive(false);
        }
    }

    private void HandleConsumeClicked()
    {
        if (assignedItem != null && _currentCount > 0)
        {
            OnConsumeClicked?.Invoke(assignedItem);
        }

        HideConsumeButton();
    }

    private void OnDestroy()
    {
        if (consumeButton != null)
        {
            consumeButton.onClick.RemoveListener(HandleConsumeClicked);
        }
    }
}