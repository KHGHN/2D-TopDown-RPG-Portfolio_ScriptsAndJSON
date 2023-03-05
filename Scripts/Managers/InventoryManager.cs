using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 인벤토리 생성해서 가지고 있게
public class InventoryManager
{
    
    //private Canvas inventory;
    public Dictionary<int, Data.Item> Item_dict { get; private set; }
    public List<Canvas> InvenList { get; private set; }

    //private InventorySlot[] slots;
    //private List<Item> inventoryItemList;
    //private GridLayoutGroup gridSlot;

    public string Path { get; private set; }
    public bool isSelected;

    public void Init()
    {
        // 플레이어용 인벤토리 생성
        InvenList = new List<Canvas>();
        Item_dict = Managers.Data.ItemDict;
        Path = "InventoryCanvas";
    }

    public Canvas MakeInventory()
    {
        Canvas inven = Managers.Resource.Instantiate($"UI/Inventory/{Path}").GetComponent<Canvas>();

        InvenList.Add(inven);

        return inven;
    }
}
