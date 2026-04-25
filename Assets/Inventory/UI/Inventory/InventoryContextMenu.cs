using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryContextMenu : MonoBehaviour
{
    [SerializeField] private Button consumeButton;
    [SerializeField] private TextMeshProUGUI consumeButtonText;

    private GridInventorySlot _currentSlot;
    private Action<ItemData> _onConsumeClicked;

    private void Awake()
    {
        if (consumeButton != null)
        {
            consumeButton.onClick.AddListener(OnConsumeClicked);
        }

        gameObject.SetActive(false);
    }

    public void Init(Action<ItemData> onConsumeClicked)
    {
        _onConsumeClicked = onConsumeClicked;
    }

    public void Show(GridInventorySlot slot, Vector2 position)
    {
        _currentSlot = slot;

        transform.position = position;
        gameObject.SetActive(true);

        if (consumeButtonText != null)
        {
            consumeButtonText.text = $"Consume {slot.AssignedItem.ItemName}";
        }
    }

    public void Hide()
    {
        _currentSlot = null;
        gameObject.SetActive(false);
    }

    private void OnConsumeClicked()
    {
        if (_currentSlot != null && _currentSlot.AssignedItem != null)
        {
            _onConsumeClicked?.Invoke(_currentSlot.AssignedItem);
        }

        Hide();
    }

    private void Update()
    {
        if (gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            Hide();
        }
    }

    private void OnDestroy()
    {
        if (consumeButton != null)
        {
            consumeButton.onClick.RemoveListener(OnConsumeClicked);
        }
    }
}