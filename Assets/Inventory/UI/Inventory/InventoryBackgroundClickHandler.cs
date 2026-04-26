using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryBackgroundClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GridInventoryUI gridInventoryUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            gridInventoryUI.HideAllConsumeButtons();
        }
    }
}