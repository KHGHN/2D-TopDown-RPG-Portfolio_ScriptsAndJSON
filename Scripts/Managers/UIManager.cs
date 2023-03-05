using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 팝업창 관리를 위해 만듬
public class UIManager
{
    public GameObject ItemDeletePopup { get; private set; }
    public GameObject ItemInfoPopup { get; private set; }


    public void Init()
    {
        // 아이템 휴지통 아이콘에 놓았을 때 뜨는 팝업창
        ItemDeletePopup = Managers.Resource.Instantiate("UI/Inventory/ItemDeletePopup");
        ItemDeletePopup.gameObject.SetActive(false);
    }


}
