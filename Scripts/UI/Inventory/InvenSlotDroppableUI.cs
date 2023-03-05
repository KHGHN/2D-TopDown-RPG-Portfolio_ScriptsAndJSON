using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


// 인벤토리의 슬롯에 일어나는 것들을 처리하기 위해
public class InvenSlotDroppableUI : MonoBehaviour, IDropHandler  //IPointerExitHandler, IPointerEnterHandler
{
    private Canvas canvas;          // 인벤토리 캔버스
    private Inventory inventory;    // 인벤토리 창

    private void Start()
    {
        canvas = Managers.Inventory.InvenList[0];
        inventory = canvas.transform.GetComponentInChildren<Inventory>();
    }

    // 마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //}

    // 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //}

    // 현재 아이템 슬롯 영역 내부에서 드롭을 했을 때 1회 호출
    
    // 마우스를 드래그 해서 드랍했을 때 처리
    public void OnDrop(PointerEventData eventData)
    {

        // 인벤토리의 아이템 데이터 리스트와 슬롯리스트를 불러옴
        List<Item> items = inventory.inventoryItemList;
        List<InventorySlot> inventorySlots = inventory.SlotList;

        // 드래그를 시작한 슬롯과 드롭했는 슬롯을 구함
        InventorySlot preSlot = eventData.pointerDrag.transform.GetComponent<ItemDraggableUI>().PreviousParent.GetComponent<InventorySlot>();
        InventorySlot dropSlot = transform.GetComponent<InventorySlot>();

        int preSlotIndex = inventorySlots.FindIndex(slots => slots == preSlot);
        int dropSlotIndex = inventorySlots.FindIndex(slots => slots == dropSlot);

        // 아이템을 드래그 해서 인벤토리의 휴지통 아이콘에 드랍했다면
        if(gameObject.name == "InventoryDeleteButton")
        {
            //Debug.Log("휴지통 아이콘에 드롭했습니다");


            // 진짜 지우겠냐는 창 팝업
            GameObject ItemDeletePopup = Managers.UI.ItemDeletePopup;
            ItemDeletePopup.gameObject.SetActive(true);
            inventory.deleteItemIndex = preSlotIndex;
            //bool isDeleteYes = false;

            // 지우겠냐는 창 팝업 시 사운드 출력
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/GenericNotification10b", Define.Sound.Effect);

            return;
        }

        // 들었다 놓는경우 오류 방지
        if(preSlot == dropSlot)
        {
            return;
        }

        // 빈 슬롯만 있을 때 오류 방지
        if(items[preSlotIndex].Item_type == Define.ItemType.Empty && items[dropSlotIndex].Item_type == Define.ItemType.Empty)
        {
            return;
        }

        // 들어있는 슬롯과 빈 슬롯이랑 스왑하는 경우
        else if (items[dropSlotIndex].Item_type == Define.ItemType.Empty) 
        {
            // 찬슬롯 -> 빈슬롯
            ListSwap(items, preSlotIndex, dropSlotIndex);
            SwapToEmptyDataSet(inventorySlots, preSlotIndex, dropSlotIndex);
        }
        else // 아이템 들어있는 슬롯끼리 스왑하는 경우
        {
            // 중첩 불가능 한 경우
            if (inventorySlots[preSlotIndex].item_id != inventorySlots[dropSlotIndex].item_id) // 다른 종류의 아이템끼리 중첩하려는 경우 그냥 스왑
            {
                ListSwap(items, preSlotIndex, dropSlotIndex);
                SwapDataSet(inventorySlots, preSlotIndex, dropSlotIndex);
            }
            //중첩 가능한 경우
            else if ((inventorySlots[preSlotIndex].item_id == inventorySlots[dropSlotIndex].item_id) &&     // 두개가 같은 종류의 아이템이면서 마우스로 드래그한 아이템이 소모품이나 중첩가능 타입이 일때
                items[preSlotIndex].Item_type == Define.ItemType.Consumables || items[preSlotIndex].Item_type == Define.ItemType.IncreasableCount)
            {
                int result = inventory.IncreaseItemCount(items[preSlotIndex],dropSlotIndex); // drop에는 pre의 개수를 더 해주고
                if(result != -1)  // 최대개수 등으로 중첩에 실패하지 않았다면
                {
                    Item item = inventory.CreateEmptyItem();          //pre 아이템리스트는 empty로 교체하고 
                    items[preSlotIndex] = item;
                    inventory.CleanItemSlotUI(preSlotIndex);  // 드래그한쪽은 슬롯을 비워주고

                }
                else if(result == -1) // 최대개수 등으로 중첩에 실패했는 경우 그냥 스왑
                {
                    ListSwap(items, preSlotIndex, dropSlotIndex);
                    SwapDataSet(inventorySlots, preSlotIndex, dropSlotIndex);
                }
            }
        }

        // 아이템 드롭 시 사운드 출력
        Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/Popup1", Define.Sound.Effect);

    }

    // 빈 슬롯에 드롭하는 경우 빈 슬롯이랑 스왑
    private void SwapToEmptyDataSet(List<InventorySlot> inventorySlots, int preSlotIndex, int dropSlotIndex)
    {
        SwapDataSet(inventorySlots, preSlotIndex, dropSlotIndex);

        if(inventorySlots[preSlotIndex].isEmpty)
        {
            inventorySlots[preSlotIndex].Icon.color = new Color(
                inventorySlots[preSlotIndex].Icon.color.r, inventorySlots[dropSlotIndex].Icon.color.g,
                inventorySlots[preSlotIndex].Icon.color.b, 0);
        }
        else
        {
            inventorySlots[preSlotIndex].Icon.color = new Color(
                inventorySlots[preSlotIndex].Icon.color.r, inventorySlots[dropSlotIndex].Icon.color.g,
                inventorySlots[preSlotIndex].Icon.color.b, 1);
        }

        if (inventorySlots[dropSlotIndex].isEmpty)
        {
            inventorySlots[dropSlotIndex].Icon.color = new Color(
                inventorySlots[dropSlotIndex].Icon.color.r, inventorySlots[dropSlotIndex].Icon.color.g,
                inventorySlots[dropSlotIndex].Icon.color.b, 0);
        }
        else
        {
            inventorySlots[dropSlotIndex].Icon.color = new Color(
                inventorySlots[dropSlotIndex].Icon.color.r, inventorySlots[dropSlotIndex].Icon.color.g,
                inventorySlots[dropSlotIndex].Icon.color.b, 1);
        }

        // isEmpty Swap

        //bool isPreEmtpy = false;
        //bool isDropEmtpy = false;

        //if (inventorySlots[preSlotIndex].isEmpty)
        //{
        //    isPreEmtpy = true;
        //}

        //if (inventorySlots[dropSlotIndex].isEmpty)
        //{
        //    isDropEmtpy = true;
        //}

        //bool isEmpty = inventorySlots[preSlotIndex].isEmpty;

        //inventorySlots[preSlotIndex].isEmpty = inventorySlots[dropSlotIndex].isEmpty;

        //inventorySlots[dropSlotIndex].isEmpty = isEmpty;


        //if (isPreEmtpy)
        //{
        //    inventorySlots[dropSlotIndex].Icon.color = new Color(
        //        inventorySlots[dropSlotIndex].Icon.color.r, inventorySlots[dropSlotIndex].Icon.color.g,
        //        inventorySlots[dropSlotIndex].Icon.color.b, 0);
        //}
        //else
        //{
        //    inventorySlots[dropSlotIndex].Icon.color = new Color(
        //        inventorySlots[dropSlotIndex].Icon.color.r, inventorySlots[dropSlotIndex].Icon.color.g,
        //        inventorySlots[dropSlotIndex].Icon.color.b, 1);
        //}

        //if (isDropEmtpy)
        //{
        //    inventorySlots[preSlotIndex].Icon.color = new Color(
        //        inventorySlots[preSlotIndex].Icon.color.r, inventorySlots[preSlotIndex].Icon.color.g,
        //        inventorySlots[preSlotIndex].Icon.color.b, 0);
        //}
        //else
        //{
        //    inventorySlots[preSlotIndex].Icon.color = new Color(
        //        inventorySlots[preSlotIndex].Icon.color.r, inventorySlots[preSlotIndex].Icon.color.g,
        //        inventorySlots[preSlotIndex].Icon.color.b, 1);
        //}
    }

    // 스왑된 정보 슬롯에 입력
    private void SwapDataSet(List<InventorySlot> inventorySlots, int preSlotIndex, int dropSlotIndex)
    {
        // Image Swap

        Sprite sprite = inventorySlots[preSlotIndex].Icon.sprite;

        inventorySlots[preSlotIndex].Icon.sprite = inventorySlots[dropSlotIndex].Icon.sprite;

        inventorySlots[dropSlotIndex].Icon.sprite = sprite;

        // Item_Count Swap

        bool isPreTextAlphaZero = false;
        bool isDropTextAlphaZero = false;

        if (inventorySlots[preSlotIndex].ItemCount_Text.color.a == 0)
        {
            isPreTextAlphaZero = true;
        }

        if (inventorySlots[dropSlotIndex].ItemCount_Text.color.a == 0)
        {
            isDropTextAlphaZero = true;
        }


        string text = inventorySlots[preSlotIndex].ItemCount_Text.text;

        inventorySlots[preSlotIndex].ItemCount_Text.text = inventorySlots[dropSlotIndex].ItemCount_Text.text;

        inventorySlots[dropSlotIndex].ItemCount_Text.text = text;



        if (isPreTextAlphaZero)
        {
            inventorySlots[dropSlotIndex].ItemCount_Text.color = new Color(
                inventorySlots[dropSlotIndex].ItemCount_Text.color.r, inventorySlots[dropSlotIndex].ItemCount_Text.color.g,
                inventorySlots[dropSlotIndex].ItemCount_Text.color.b, 0);
        }
        else
        {
            inventorySlots[dropSlotIndex].ItemCount_Text.color = new Color(
                inventorySlots[dropSlotIndex].ItemCount_Text.color.r, inventorySlots[dropSlotIndex].ItemCount_Text.color.g,
                inventorySlots[dropSlotIndex].ItemCount_Text.color.b, 1);
        }

        if (isDropTextAlphaZero)
        {
            inventorySlots[preSlotIndex].ItemCount_Text.color = new Color(
                inventorySlots[preSlotIndex].ItemCount_Text.color.r, inventorySlots[preSlotIndex].ItemCount_Text.color.g,
                inventorySlots[preSlotIndex].ItemCount_Text.color.b, 0);
        }
        else
        {
            inventorySlots[preSlotIndex].ItemCount_Text.color = new Color(
                inventorySlots[preSlotIndex].ItemCount_Text.color.r, inventorySlots[preSlotIndex].ItemCount_Text.color.g,
                inventorySlots[preSlotIndex].ItemCount_Text.color.b, 1);
        }



        // item_id Swap

        int item_id = inventorySlots[preSlotIndex].item_id;

        inventorySlots[preSlotIndex].item_id = inventorySlots[dropSlotIndex].item_id;

        inventorySlots[dropSlotIndex].item_id = item_id;

        // isSelected Swap

        bool isSelected = inventorySlots[preSlotIndex].isSelected;

        inventorySlots[preSlotIndex].isSelected = inventorySlots[dropSlotIndex].isSelected;

        inventorySlots[dropSlotIndex].isSelected = isSelected;

        // isEmpty Swap

        bool isEmpty = inventorySlots[preSlotIndex].isEmpty;

        inventorySlots[preSlotIndex].isEmpty = inventorySlots[dropSlotIndex].isEmpty;

        inventorySlots[dropSlotIndex].isEmpty = isEmpty;



    }

    // 아이템 데이터 리스트 스왑
    public void ListSwap<T>(List<T> list, int from, int to)
    {
        if(to == -1)    // IndexOutofRange 방지
        {
            return;
        }
        T tmp = list[from];
        list[from] = list[to];
        list[to] = tmp;
    }


}
