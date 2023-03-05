using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 인벤토리 창에서 슬롯정보 관련 처리
public class InventorySlot : MonoBehaviour
{
    public Image Icon { get; set; }
    public Text ItemCount_Text { get; set; }

    public int item_id { get; set; }

    public bool isSelected;

    public bool isEmpty;

    private void Awake()
    {
        isEmpty = true;
        Init();
    }

    public void Init()
    {
        Transform transform = gameObject.transform.GetChild(0);
        Icon = transform.GetComponent<Image>();
        //Transform transform2 = gameObject.transform.GetChild(1);
        //ItemCount_Text = transform2.GetComponent<Text>();
        Transform transform2 = transform.transform.GetChild(0);
        ItemCount_Text = transform2.GetComponent<Text>();
    }

    //public void AddItem(Item _item)
    //{
    //    //itemName_Text.text = _item.Item_name;
    //    Icon.sprite = _item.itemIcon;
    //    if(Define.ItemType.Consumables == _item.Item_type) // 소모품일 경우
    //    {
    //        if (_item.Item_count > 0)
    //            ItemCount_Text.text = "x " + _item.Item_count.ToString();
    //        else
    //            ItemCount_Text.text = "";
    //    }
    //}


    //public void RemoveItem()
    //{
    //    //itemName_Text.text = "";
    //    ItemCount_Text.text = "";
    //    Icon.sprite = null;
    //}

}
