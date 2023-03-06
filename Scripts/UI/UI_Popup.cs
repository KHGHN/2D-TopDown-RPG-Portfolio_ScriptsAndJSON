using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// 여러 팝업창들 관련 처리
public class UI_Popup : MonoBehaviour
{

    // 인벤 아이템 삭제 여부 팝업
    private Canvas invenCanvas;
    private Inventory inven;
    GameObject ItemDeletePopup;
    GameObject ItemInfoPopup;

    UI_Popup ItemInfopop;
    //FindItemInfoPosition findItemInfoPosition;

    Image[] icons;
    Image icon;
    Text text;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        invenCanvas = Managers.Inventory.InvenList[0];
        inven = invenCanvas.transform.GetComponentInChildren<Inventory>();
        //findItemInfoPosition = inven.gameObject.transform.GetComponentInChildren<FindItemInfoPosition>();

        ItemDeletePopup = Managers.UI.ItemDeletePopup;

        ItemInfopop = inven.GetComponentInChildren<UI_Popup>(true);
        //ItemInfoPopup = Managers.UI.ItemInfoPopup;

        icons = ItemInfopop.GetComponentsInChildren<Image>(true);

        for (int i = 0; i < icons.Length; i++)
        {
            if (icons[i].gameObject.name == "ItemImage")
            {
                icon = icons[i];
            }
        }
        text = ItemInfopop.GetComponentInChildren<Text>(true);
    }


    // 인벤토리에서 휴지통 아이콘에 아이템을 드롭했을 때 처리
    public void DeleteItem(Button button)
    {

        // 네 버튼을 눌렀을 때
        if (button.name == "YesButton")
        {
            //Debug.Log("Yes 버튼을 눌렀습니다");

            // 빈 아이템을 생성해서 아이템 리스트의 해당 인덱스에 빈 아이템을 넣고 슬롯도 비우기
            Item item = inven.CreateEmptyItem();
            int preSlotIndex = inven.deleteItemIndex;
            List<Item> items = inven.inventoryItemList;
            items[preSlotIndex] = item;
            inven.CleanItemSlotUI(preSlotIndex);

            // 아이템 삭제 시 출력 사운드
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/Buttons/ClickyButton9a", Define.Sound.Effect);

            ItemDeletePopup.SetActive(false);



        }
        // 아니오 버튼을 눌렀을 때
        else if (button.name == "NoButton")
        {
            // 취소 사운드
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/Buttons/ClickyButton10a", Define.Sound.Effect);
            ItemDeletePopup.SetActive(false);
            return;
        }
    }

    // 인벤토리창을 키보드 키가 아니라 화면에 있는 아이콘을 누를때 켜고 끄기
    public void PopupInventoryWindow()
    {
        if(invenCanvas.gameObject.activeSelf)
        {
            invenCanvas.gameObject.SetActive(false);
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/Buttons/GenericButton14", Define.Sound.Effect);
        }
        else
        {
            invenCanvas.gameObject.SetActive(true);
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/Buttons/GenericButton15", Define.Sound.Effect);
        }
    }

    // 아이템창 정보 출력
    public void PopupItemInfo(Sprite sprite,string _text, Define.ItemType type)
    {
        Init();

        if (type == Define.ItemType.Empty)
        {
            return;
        }

        icon.sprite = sprite;
        text.text = _text;


        //UI_Popup uI_Popup = ItemInfoPopup.GetComponentInChildren<UI_Popup>();

        //uI_Popup.gameObject.transform.position = Camera.main.ViewportToScreenPoint(Input.mousePosition);
        ItemInfopop.gameObject.SetActive(true);
    }

    // 아이템창 닫기
    public void PopupItemInfoClean(Sprite sprite, string _text, Define.ItemType type)
    {
        Init();

        icon.sprite = sprite;
        text.text = _text;
 
        //UI_Popup uI_Popup = ItemInfoPopup.GetComponentInChildren<UI_Popup>();

        //uI_Popup.gameObject.transform.position = Camera.main.ViewportToScreenPoint(Input.mousePosition);
        ItemInfopop.gameObject.SetActive(true);
    }
}
