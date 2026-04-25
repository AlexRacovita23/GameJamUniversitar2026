using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GridInventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject countContainer;

    [Header("Configuration")]
    [SerializeField] private ItemData assignedItem;

    private int _currentCount;
    private GridInventoryUI _parentUI;

    public ItemData AssignedItem => assignedItem;
    public int CurrentCount => _currentCount;

    public void Init(GridInventoryUI parentUI)
    {
        _parentUI = parentUI;

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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _currentCount > 0)
        {
            _parentUI.ShowContextMenu(this, eventData.position);
        }
    }
}