using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



// 아이템을 드래그 앤 드롭할 때 관련 처리, 드롭은 InvenSlotDroppableUI 클래스에 있음
public class ItemDraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private PlayerStat stat;

    // 인벤토리창 캔버스, 인벤토리창 
    private Canvas canvas;
    private Inventory inventory;
    private RectTransform rect;

    // 아이템 드래그앤드롭(스왑) 관련
    public Transform PreviousParent { get; private set; }
    private CanvasGroup canvasGroup;

    // 아이템 정보뜨는 팝업 관련
    UI_Popup ItemInfopop;
    Image mouseOverItemImage;
    int mouseOverItemImageIndex;

    // 아이템 사용 관련 ( 더블클릭 )
    float interval;             // 첫번째 클릭과 두번째 클릭간의 시간 차
    float doubleClickedTime;    // 첫번째 클릭 시간 입력
    bool isDoubleClicked;       // 더블클릭 했는지

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 더블클릭 판단 관련 필드 초기화
        interval = 0.25f;
        doubleClickedTime = -1.0f;
        isDoubleClicked = false;
    }

    private void Start()
    {
        canvas = Managers.Inventory.InvenList[0];
        inventory = canvas.GetComponentInChildren<Inventory>();
        ItemInfopop = inventory.GetComponentInChildren<UI_Popup>(true);
    }


    // 아이템 위에 마우스 커서 올렸을 때 아이템 정보창 뜨게 하려고
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverItemImage = gameObject.GetComponent<Image>();
        mouseOverItemImageIndex = inventory.inventoryItemList.FindIndex(image => image.itemIcon == mouseOverItemImage.sprite);
        if (mouseOverItemImageIndex == -1)
        {
            return;
        }

        Sprite sprite = inventory.inventoryItemList[mouseOverItemImageIndex].itemIcon;
        string text = inventory.inventoryItemList[mouseOverItemImageIndex].Item_description;
        Define.ItemType type = inventory.inventoryItemList[mouseOverItemImageIndex].Item_type;
        ItemInfopop.PopupItemInfo(sprite, text, type);

        //Debug.Log("마우스가 위에 있습니다");
    }

    // 아이템 사용을 위해
    public void OnPointerClick(PointerEventData eventData)
    {
        // 첫번 째 클릭시간과 두번째 클릭시간이 설정한 interval보다 작다면 더블클릭 판단
        if (Time.time - doubleClickedTime < interval)
        {
            isDoubleClicked = true;
            doubleClickedTime = -1.0f;
        }
        // 그냥 클릭한 경우 첫번째 클릭 시간 저장
        else
        {
            isDoubleClicked = false;
            doubleClickedTime = Time.time;
        }

        // 더블클릭 했는 경우
        if (isDoubleClicked)
        {
            // 더블클릭으로 아이템 사용!
            if (eventData.pointerClick.GetComponent<Image>() != null)
            {

                Image image = eventData.pointerClick.GetComponent<Image>();
                InventorySlot _slots = image.gameObject.GetComponentInParent<InventorySlot>();

                int index = inventory.SlotList.FindIndex(slot => slot.gameObject.name == _slots.gameObject.name);
                Debug.Log(inventory.inventoryItemList[index] + " 아이템이 더블클릭됐습니다.");

                stat = canvas.transform.root.GetComponent<PlayerStat>();

                switch (inventory.inventoryItemList[index].Item_type)
                {
                    
                    case Define.ItemType.Consumables: // 소모품인 경우
                        // 플레이어에게 아이템 effect 적용
                        if (inventory.inventoryItemList[index].Item_id == (int)Define.ItemID.SmallHpPotion) // 소형 HP포션의 경우
                        {
                            if (stat.Hp == stat.MaxHp)
                            {
                                Debug.Log("HP가 가득찬 상태입니다");
                                // 가득찼다고 알리는 사운드 출력
                                Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/Success12a", Define.Sound.Effect);
                            }
                            else
                            {
                                stat.Hp += inventory.inventoryItemList[index].Effect;
                                if( (stat.Hp += inventory.inventoryItemList[index].Effect) > stat.MaxHp) // hp가 회복됐는데 max를 넘는경우 max로 입력
                                {
                                    stat.Hp = stat.MaxHp;
                                }

                                Debug.Log("플레이어의 HP가 " + inventory.inventoryItemList[index].Effect + "만큼 회복됐습니다");
                                // 아이템 리스트랑 슬롯 리스트 정보 변경
                                if (inventory.inventoryItemList[index].item_count == 1) // 아이템 개수가 1이라면 
                                {
                                    inventory.inventoryItemList[index] = inventory.CreateEmptyItem(); //아이템 리스트 인덱스에 빈 아이템을 넣고
                                    inventory.CleanItemSlotUI(index);  // 슬롯 비우기
                                }
                                else
                                {
                                    inventory.inventoryItemList[index].item_count--; // 1개 이상이라면 1개 차감
                                    inventory.SlotList[index].ItemCount_Text.text = // 슬롯에도 1개 차감
                                        (int.Parse(inventory.SlotList[index].ItemCount_Text.text) - 1).ToString();

                                }

                                // 포션 회복 사운드 출력
                                Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/GenericNotification9", Define.Sound.Effect);
                            }
                        }
                        break;
                }
            }

        }
        //Debug.Log("아이템이 클릭됐습니다.");
    }


    // 마우스가 아이템 위에서 사라질 때 아이템 정보창 닫게 하려고
    public void OnPointerExit(PointerEventData eventData)
    {

        Sprite sprite = inventory.inventoryItemList[mouseOverItemImageIndex].itemIcon;
        string text = inventory.inventoryItemList[mouseOverItemImageIndex].Item_description;
        Define.ItemType type = inventory.inventoryItemList[mouseOverItemImageIndex].Item_type;
        ItemInfopop.PopupItemInfoClean(sprite, text, type);
        ItemInfopop.gameObject.SetActive(false);
    }

    // 현재 오브젝트를 드래그 하기 시작할 때 1회 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 직전에 소속되어 있던 부모 tranform 정보 저장
        PreviousParent = transform.parent;

        // 현재 드래그중인 UI가 화면의 최상단에 출력되도록 하기 위해
        transform.SetParent(canvas.transform);        // 부모 오브젝트를 Canvas로 지정
        transform.SetAsLastSibling();       // 가장 앞에 보이도록 마지막 자식으로 설정

        // 드래그 가능한 오브젝트가 하나가 아닌 자식들을 가지고 있을 수도 때문에 canvasGroup으로 통제
        // 알파값을 0.6으로 설정하고 , 광선 충돌처리가 되지 않도록 한다.
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // 드래그 시 사운드 출력
        Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/Popup4b", Define.Sound.Effect);

    }
    // 현재 오브젝트를 드래그 중일 때 매 프레임 호출
    public void OnDrag(PointerEventData eventData)
    {
        // 현재 스크린상의 마우스 위치를 UI 위치로 설정 ( UI가 마우스를 쫓아다니는 상태 )
        rect.transform.position = eventData.position;
    }

    // 현재 오브젝트의 드래그를 종료할 때 1회 호출
    public void OnEndDrag(PointerEventData eventData)
    {

        // 드래그를 시작하면 부모가 canvas로 설정되기 때문에
        // 드래그를 종료할 때 부모가 canvas이면 아이템 슬롯이 아닌 엉뚱한 곳에
        // 드롭을 했다는 뜻이기 때문에 드래그 직전에 소속되어 있던 아이템 슬롯으로 아이템 이동
        // 알파값을 1로 설정하고 , 광선 충돌처리가 되도록 함.
        transform.SetParent(PreviousParent.transform);
        rect.transform.position = PreviousParent.position;
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        // 드래그가 끝날 때 사운드 출력
        Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/Popup1", Define.Sound.Effect);
    }
}
