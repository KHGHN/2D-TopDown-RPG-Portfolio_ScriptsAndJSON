using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 아이템 클래스
public class Item : MonoBehaviour
{
    public int Item_id { get; private set; }
    public string Item_name { get; private set; }
    public string Item_description { get; private set; }
    public Define.ItemType Item_type { get; set; }
    public Sprite itemIcon { get; private set; }
    public int Effect { get; private set; }

    [SerializeField]
    public int Item_count { get { return item_count; } set { item_count = value; } }

    [SerializeField]
    public int item_count;


    [SerializeField]
    public int MaxCount { get; private set; }


    private void Awake()
    {
        SetItem();
    }

    public void SetItem(int _itemCount = 1)
    {
        // 복제된 오브젝트 뒤에 붙는 Clone 때문에 오류나는거 막기
        if (gameObject.name.Contains("Clone"))
        {
            int index = gameObject.name.IndexOf("(Clone)");
            if (index > 0)
                gameObject.name = gameObject.name.Substring(0, index);
        }

        int _item_id = int.Parse(gameObject.name);
        Dictionary<int, Data.Item> dict = Managers.Inventory.Item_dict;
        Data.Item item = dict[_item_id];

        Item_id = item.item_id;
        Item_count = _itemCount; // 기본 생성 시 1개로
        MaxCount = item.max_count;
        Item_name = item.item_name;
        Item_description = item.item_description;
        Item_type = (Define.ItemType)item.item_type;
        Effect = item.effect;
        itemIcon = Managers.Resource.Load<SpriteRenderer>("Prefabs/UI/Inventory/" + Item_id).sprite;

    }

    public void SetItemCount(int count)
    {
        item_count = count;
    }
    public void SetItemMaxCount(int count)
    {
        MaxCount = count;
    }
}
