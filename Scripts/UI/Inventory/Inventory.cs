using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// 인벤토리 창
public class Inventory : MonoBehaviour
{
    //private Canvas canvas;
    //private InventorySlot[] slots;
    //private Dictionary<int, InventorySlot> SlotDic;
    //private Dictionary<int, Item> inventoryItemDic;

    private Inventory inventory;
    public List<InventorySlot> SlotList { get; set; }
    public List<Item> inventoryItemList { get; set; }
    
    private GridLayoutGroup gridSlot;

    private int inventorySlotNumberLimit;   // 최대 슬롯 수

    private int inventorySlotNumber;        // 현재 슬롯에 아이템이 몇개 찼는지 확인하는 필드

    public int deleteItemIndex;             // 인벤토리에 휴지통 아이콘에 아이템 드롭했을 때 삭제될 아이템 리스트의 Index 필드

    // Start is called before the first frame update
    void Awake()
    {
        InventoryInit();
    }

    // 인벤토리창 켜지고 꺼질 때 사운드 출력
    public void PlaySoundOpenAndQuitInventoryWindow(bool _active)
    {

        if(_active)   // 켜기
        {
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/Buttons/GenericButton15", Define.Sound.Effect);
        }
        else // 끄기
        {
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/Buttons/GenericButton14", Define.Sound.Effect);
        }
        
    }

    // 인벤토리 창 초기화
    public void InventoryInit()
    {
        inventorySlotNumberLimit = 30;
        inventory = gameObject.GetComponent<Inventory>();
        inventoryItemList = new List<Item>();
        inventoryItemList.Capacity = 40;
        SlotList = new List<InventorySlot>();
        gridSlot = inventory.GetComponentInChildren<GridLayoutGroup>();

        // 빈 아이템 생성
        for (int i = 0; i < inventorySlotNumberLimit; i++)
        {
            Item item = CreateEmptyItem();
            inventoryItemList.Add(item);
        }

        // 빈 슬롯 생성
        for (int i = 0; i < inventorySlotNumberLimit; i++)
        {
            MakeItemEmptySlotUI(i);
        }

        // 생성 된 모든 슬롯 인벤토리 gridslot에 붙이기
        for (int i = 0; i < gridSlot.transform.childCount; i++)
        {
            SlotList.Add(gridSlot.transform.GetChild(i).GetComponent<InventorySlot>());
        }
    }

    // 빈 아이템 생성
    public Item CreateEmptyItem()
    {
        Item item = Managers.Resource.Load<Item>($"Prefabs/UI/Inventory/EmptyITem");
        item.Item_type = Define.ItemType.Empty;

        return item;
    }

    // 아이템을 주웠을 경우 인벤토리에 아이템 넣는 처리
    public void PickItem(Item _item) 
    {
        // 더 이상 인벤토리 슬롯이 없는 경우
        if(inventorySlotNumber == inventorySlotNumberLimit)
        {
            Debug.Log("아이템슬롯이 꽉 찼습니다");
            return;
        }

        // 슬롯에 아무 아이템도 없다면 채워넣기
        if (inventorySlotNumber == 0) 
        {
            inventoryItemList[0] = _item;   // 전부 빈 아이템이기 때문에 첫칸에 넣어주기
            PutSlotItemInfo(_item);         // 슬롯 정보 입력
            inventorySlotNumber += 1;       // 현재 슬롯에 있는 아이템 수 증가
        }
        // 슬롯에 아이템이 1개 이상 있다면
        else 
        {
            // 소모품 또는 중첩가능하면서 인벤토리내에 있는 아이템과 같은 아이템이라면
            if ((_item.Item_type == Define.ItemType.Consumables ||           
                _item.Item_type == Define.ItemType.IncreasableCount) &&
                inventoryItemList.FindIndex(item => item.Item_id == _item.Item_id) != -1)
            {
                Item item = CreateEmptyItem();  // 빈 아이템 생성 후 여기에 획득한 아이템 받아서
                item = _item;
                IncreaseItemCount(item);        // 같은 아이템 중첩
            }
            else // 다른종류라면 ( 장비, 중첩 불가능한 타입 등등 )
            {
                // 아이템데이터 저장하고 새로운 슬롯만들어서 아이템 넣기
                PutItemToEmptyList(_item);
                PutSlotItemInfo(_item);
            }

        }
    }

    // 슬롯 비우기
    public void CleanItemSlotUI(int index) 
    {
        Item item = CreateEmptyItem(); // 밑에 sprite를 none으로 만들기 위해 빈 아이템을 생성

        SlotList[index].Icon.sprite = item.itemIcon;
        SlotList[index].Icon.color = new Color(1, 1, 1, 0);
        SlotList[index].ItemCount_Text.text = "";
        SlotList[index].item_id = 0;
        SlotList[index].isEmpty = true;
    }

    // 초기화 시 인벤토리에 비어있는 최대개수 슬롯 생성
    public void MakeItemEmptySlotUI(int i)
    {
        // 슬롯 프리팹을 들고와서 gridslot에 붙여주고
        GameObject slotPrefab = Managers.Resource.Instantiate("UI/Inventory/Slot");
        slotPrefab.transform.SetParent(gridSlot.transform, false);
        slotPrefab.transform.name = slotPrefab.transform.name + i.ToString();

        // 슬롯에 아이템 정보 넣어주고 흰색 sprite가 보이면 안되므로 알파값 0
        InventorySlot slot = slotPrefab.GetComponent<InventorySlot>();
        slot.Icon.color = new Color(1, 1, 1, 0);
        slot.ItemCount_Text.text = "";
        slot.item_id = 0;
    }

    // 슬롯에 아이템 정보 입력
    public void PutSlotItemInfo(Item _item)
    {
        // 비어있는 슬롯의 인덱스 반환 후 해당 슬롯에 아이템이 들어있다고 변경
        int emptyIndex = SlotList.FindIndex(slot => slot.isEmpty == true);
        SlotList[emptyIndex].isEmpty = false;

        // 반환받은 인덱스의 slot에 정보 입력해주고 sprite가 보여야 되므로 알파값 1
        SlotList[emptyIndex].Icon.sprite = _item.itemIcon;
        SlotList[emptyIndex].Icon.color = new Color(1, 1, 1, 1);
        SlotList[emptyIndex].ItemCount_Text.text = _item.Item_count.ToString();
        if (!(_item.Item_type == Define.ItemType.Consumables || _item.Item_type == Define.ItemType.IncreasableCount))
        {
            SlotList[emptyIndex].ItemCount_Text.color = new Color(0, 0, 0, 0);
        }
        SlotList[emptyIndex].item_id = _item.Item_id;


    }

    // 획득 시작위치에서 가까운 빈자리가 있는지 검사 후 슬롯에 넣고 아이템 데이터도 넣음
    public void PutItemToEmptyList(Item _item)
    {
        int nearEmptyIndex = inventoryItemList.FindIndex(item => item.Item_type == Define.ItemType.Empty);
        if (nearEmptyIndex != -1) // 빈자리가 있다면
        {
            inventoryItemList[nearEmptyIndex] = _item;
            inventorySlotNumber += 1;
        }
    }

    // 아이템 개수 증가 및 슬롯에 아이템 개수 텍스트 변경
    public void IncreaseItemCountAndUpdateSlotInfo(int index, Item _item)
    {
        inventoryItemList[index].item_count += _item.item_count;
        SlotList[index].ItemCount_Text.text = (int.Parse(SlotList[index].ItemCount_Text.text) + _item.Item_count).ToString();
    }

    // 중첩할 때 중첩당하는 아이템과 중첩하는 아이템을 더해서 중첩 당하는 아이템이 최대개수가 되고 중첩하는 아이템은 개수가 남는 경우 처리
    public int IncreaseItemCanCountAndUpdateSlotInfo(int index, Item _item)
    {
        int canIncreaseCount = inventoryItemList[index].MaxCount - inventoryItemList[index].item_count;
        inventoryItemList[index].item_count += canIncreaseCount;
        SlotList[index].ItemCount_Text.text = (int.Parse(SlotList[index].ItemCount_Text.text) + canIncreaseCount).ToString();
        _item.item_count -= canIncreaseCount;

        return canIncreaseCount;
    }

    // 같은 종류의 아이템 중에 최대 개수와 가장 가까운 아이템 검색
    public Item FindNearMaxCountItem(List<Item> sameItems)
    {
        int index = 0;
        for (int i = sameItems.Count - 1; i > 0; i--) // 제일 마지막부터 최대 개수에 제일 가까운 아이템을 찾음 ex)99가 최대면 98 이런식으로
        {
            if (sameItems[i].item_count == sameItems[i].MaxCount) // 최대개수랑 같은 아이템은 continue로 넘기고
            {
                continue;
            }
            if (sameItems[i].item_count < sameItems[i].MaxCount) //  최대개수보다 작으면 인벤토리내 아이템 중에 중첩개수가 제일 많으므로 반환하여 저장
            {
                index = i;
            }
        }
        return sameItems[index];
    }

    // 아이템 중첩 처리 매서드
    public int IncreaseItemCount(Item _item, int dropIndex = -1)   // -1은 입력 안한경우이고 다른 인덱스일 경우 같은 물건 중첩 시 씀
    {
        if(dropIndex == -1) // 키보드의 획득키를 눌러 아이템을 획득한 경우
        {
            int index = inventoryItemList.FindIndex(item => item.Item_id == _item.Item_id); // 같은 아이템 종류중에 제일 앞쪽에 있는 인덱스 반환

            // 들어온 아이템과 슬롯내에 있는 같은종류 아이템 중에 최대중첩개수랑 가까운 아이템에 더해지게 만들기 위해 
            // 같은 종류의 아이템을 모은 다음 개수 순서대로 정렬
            // 정렬한것 중에 마지막값부터 확인해서 최대개수에 가까운 아이템을 찾아서 정렬
            List<Item> sameItems = new List<Item>();
            sameItems = inventoryItemList.FindAll(item => item.Item_id == _item.Item_id);
            sameItems.Sort((x, y) => { return x.item_count.CompareTo(y.item_count); });


            // 같은종류의 아이템을 먹었는데 인벤토리에 1개만 있고 그 아이템의 개수가 최대개수보다 적을 경우
            if(sameItems.Count == 1 && sameItems[0].item_count < 99)
            {
                if(_item.item_count == 1 || (sameItems[0].item_count + _item.item_count == _item.MaxCount))   // 획득한 아이템의 개수가 1개인 경우 또는 두개를 더해서 최대개수인 경우
                {
                    IncreaseItemCountAndUpdateSlotInfo(index, _item);
                    return 0;

                }
                else if(sameItems[0].item_count + _item.item_count > 99)   // 획득한 아이템과 인벤토리내에 있던 아이템을 더해서 개수가 99개 이상이면 새슬롯에 저장
                {
                    int increaseIndex = inventoryItemList.FindIndex(item => item.Item_count == sameItems[0].item_count);
                    int canIncreaseCount = sameItems[0].MaxCount - sameItems[0].item_count;
                    inventoryItemList[increaseIndex].item_count += canIncreaseCount;
                    SlotList[increaseIndex].ItemCount_Text.text = (int.Parse(SlotList[increaseIndex].ItemCount_Text.text) + canIncreaseCount).ToString();
                    _item.item_count -= canIncreaseCount;

                    // 남는 개수는 새슬롯에 생성
                    PutItemToEmptyList(_item);
                    PutSlotItemInfo(_item);
                    return 0;
                }
            }
            else if(sameItems.Count == 1 && sameItems[0].item_count == sameItems[0].MaxCount)    // 최대개수인 경우
            {
                PutItemToEmptyList(_item);
                PutSlotItemInfo(_item);
            }
            else if(sameItems.Count > 1) // 인벤토리 내에 획득한 아이템과 같은 아이템이 1개 이상인 경우
            {
                //(Item nearItem, int nearIndex) tuple = (null, -1);        // 튜플로 그냥 해봄
                //tuple = FindNearMaxCountItem(sameItems);
                //nearMaxCountItem = tuple.nearItem;      // sameItem에서 찾은 아이템
                //nearMaxCountItemindex = tuple.nearIndex; // 위 아이템의 인덱스

                Item nearMaxCountItem = CreateEmptyItem();
                nearMaxCountItem = FindNearMaxCountItem(sameItems);
                int nearMaxCountItemindex = -1;


                if (nearMaxCountItem != null) // 위에서 찾았는 경우 , 유니티의 null체크는 흔히 쓰던 그 null이 아니다!!
                {
                    nearMaxCountItemindex = inventoryItemList.FindLastIndex(item => item.item_count == nearMaxCountItem.item_count); // 위에서 찾은 아이템을 다시 원래 아이템 리스트에 들어있는 인덱스를 구해서 더 해줌
                    if(inventoryItemList[nearMaxCountItemindex].item_count + _item.item_count > _item.MaxCount) // 더했는데 최대 개수가 넘어가는 경우 새슬롯에 저장 후 리턴
                    {
                        IncreaseItemCanCountAndUpdateSlotInfo(nearMaxCountItemindex, _item);

                        PutItemToEmptyList(_item);
                        PutSlotItemInfo(_item);
                        return 0;
                    }
                    else if(inventoryItemList[nearMaxCountItemindex].item_count + _item.item_count <= _item.MaxCount) // 더 했는데 최대개수거나 최대개수보다 작은 경우
                     {
                        IncreaseItemCountAndUpdateSlotInfo(nearMaxCountItemindex, _item);
                        return 0;
                    }
                }
                else if(nearMaxCountItem == null) // sameItem들이 전부 최대개수인경우
                {
                    PutItemToEmptyList(_item);
                    PutSlotItemInfo(_item);
                    return 0;
                }
            }

        }
        else if(dropIndex != -1)  // 마우스 드래그앤드랍로 중첩시키는 경우
        {
            if ((inventoryItemList[dropIndex].item_count == inventoryItemList[dropIndex].MaxCount)) // 최대 개수라면 리턴
            {
                Debug.Log("현재 중첩하려는 아이템이 최대개수 입니다");
                return -1;
            }
            else if(inventoryItemList[dropIndex].item_count + _item.Item_count > inventoryItemList[dropIndex].MaxCount) // 최대개수는 아닌데 중첩가능한 경우 2개 더해서 최대개수 넘어가는 경우, 보통 팝업창으로 중첩시킬수있는 개수가 나오지만
                                                                                                                        // 시간관계상 최대개수로 만들어지고 남는건 새슬롯에 생성하는걸로
            {
                if(_item.item_count == _item.MaxCount)
                {
                    return -1;
                }

                IncreaseItemCanCountAndUpdateSlotInfo(dropIndex, _item);
                PutItemToEmptyList(_item);
                PutSlotItemInfo(_item);
                return 0;
            }
            else
            {
                IncreaseItemCountAndUpdateSlotInfo(dropIndex, _item);
                return 0;
            }

        }
        return 0;
    }
}
