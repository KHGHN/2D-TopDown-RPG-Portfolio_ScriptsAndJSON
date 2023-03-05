using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 인벤토리 창 드래그 위해
public class InventoryWindowUI_Draggable : MonoBehaviour, IDragHandler
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }
}
