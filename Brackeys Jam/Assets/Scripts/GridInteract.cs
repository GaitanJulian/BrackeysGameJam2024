using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{

    InventoryController inventoryController;
    ItemGrid itemGrid;

    private void Awake()
    {
        itemGrid = GetComponent<ItemGrid>();
        inventoryController = FindObjectOfType<InventoryController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = null;
    }


}
