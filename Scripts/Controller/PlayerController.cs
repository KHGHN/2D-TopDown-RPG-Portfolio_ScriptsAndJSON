using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{

    public StateMachine<PlayerController> StateMachine { get; set; } // 플레이어 상태
    private Dictionary<Define.PlayerState, IState<PlayerController>> dicState = new Dictionary<Define.PlayerState, IState<PlayerController>>(); // 상태를 생성하여 딕셔너리에 저장할 용도

    public Dictionary<Define.PlayerState, IState<PlayerController>> DicState { get { return dicState; } set { dicState = value; } }

    private PlayerStat stat;

    private Vector2 preDir; // 앞에 아이템이 있는지 검색 관련 필드
                            // 상태가 변경 될 때 이전방향을 저장하기 위해

    Canvas invenCanvas;     // 인벤토리 Canvas

    Inventory inventory;    // 인벤토리 창



    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        stat = GetComponent<PlayerStat>();
        Anim = GetComponent<Animator>();

        invenCanvas = Managers.Inventory.MakeInventory();
        invenCanvas.transform.SetParent(this.transform);
        inventory = invenCanvas.GetComponentInChildren<Inventory>();
        invenCanvas.gameObject.SetActive(false);

    }
    void Start()
    {
        Init();
    }

    // 초기화
    void Init()
    {
        preDir = new Vector2(0, -1);

        int nLayer = 1 << LayerMask.NameToLayer("Player");

        //Debug.Log("nLayer : " + nLayer);

        // 상태 생성
        IState<PlayerController> idle = new StateIdle<PlayerController>();
        IState<PlayerController> walk = new StateWalk<PlayerController>();
        IState<PlayerController> dash = new StateDash<PlayerController>();
        IState<PlayerController> attack = new StateAttack<PlayerController>();
        IState<PlayerController> attacked = new StateAttacked<PlayerController>();
        IState<PlayerController> die = new StateDie<PlayerController>();

        //키입력 등에 따라서 언제나 상태를 꺼내 쓸 수 있게 딕셔너리에 보관
        dicState.Add(Define.PlayerState.Idle, idle);
        dicState.Add(Define.PlayerState.Walk, walk);
        dicState.Add(Define.PlayerState.Dash, dash);
        dicState.Add(Define.PlayerState.Attack, attack);
        dicState.Add(Define.PlayerState.Attacked, attacked);
        dicState.Add(Define.PlayerState.Die, die);

        // 기본 상태를 Idle로 설정
        StateMachine = new StateMachine<PlayerController>(idle);

        // Input Manager 키보드 키입력 구독
        Managers.Input.KeyAction -= OnKeyInput;
        Managers.Input.KeyAction += OnKeyInput;

        // Input Manager 마우스 입력 구독
        Managers.Input.MouseAction -= OnMouseInput;
        Managers.Input.MouseAction += OnMouseInput;





        // 테스트용
        Item hpPotion = Managers.Resource.Instantiate("UI/Inventory/101").GetComponent<Item>();
        Item hpPotion2 = Managers.Resource.Instantiate("UI/Inventory/101").GetComponent<Item>();
        Item hpPotion3 = Managers.Resource.Instantiate("UI/Inventory/101").GetComponent<Item>();
        Item hpPotion4 = Managers.Resource.Instantiate("UI/Inventory/101").GetComponent<Item>();
        Item hpPotion5 = Managers.Resource.Instantiate("UI/Inventory/101").GetComponent<Item>();
        Item hpPotion6 = Managers.Resource.Instantiate("UI/Inventory/101").GetComponent<Item>();
        hpPotion.SetItemCount(3);
        hpPotion.SetItemMaxCount(99);
        hpPotion2.SetItemCount(98);
        hpPotion2.SetItemMaxCount(99);
        hpPotion2.transform.position = new Vector3(-3.1f, 0.3f);

        hpPotion3.SetItemCount(97);
        hpPotion3.SetItemMaxCount(99);
        hpPotion3.transform.position = new Vector3(-6.0f, 3.0f);

        hpPotion4.SetItemCount(96);
        hpPotion4.SetItemMaxCount(99);
        hpPotion4.transform.position = new Vector3(-6.0f, 1.0f);

        hpPotion5.SetItemCount(95);
        hpPotion5.SetItemMaxCount(99);
        hpPotion5.transform.position = new Vector3(-6.0f, -1.0f);

        hpPotion6.SetItemCount(94);
        hpPotion6.SetItemMaxCount(99);
        hpPotion6.transform.position = new Vector3(-6.0f, -3.0f);
    }


    // Update is called once per frame
    void Update()
    {
        // 앞에 오브젝트(아이템)이 있는지 검색
        searchObject();
        // 각 상태에 따른 상태 실행
        StateMachine.DoOperateUpdate(this);

        //Debug.Log("현재 플레이어 상태 : " + StateMachine.CurrentState.ToString());

    }

    private void FixedUpdate()
    {
        // Idle 상태 체크
        if (Rigid.velocity == Vector2.zero && !Input.anyKey)
            StateMachine.SetState(dicState[Define.PlayerState.Idle], this);

        // 몬스터 이동시키기
        Rigid.velocity = new Vector2(Direction.x * MoveSpeed, Direction.y * MoveSpeed);


    }

    // 앞에 오브젝트(아이템)이 있는지 검색
    void searchObject()
    {
        Vector2 vDir = Direction;


        // 움직일 때만 레이캐스트가 되고 가만히 있으면 방향이 0, 0 이기 때문에 레이캐스트가 안쏴지는 것 때문에 작성
        if (StateMachine.CurrentState == dicState[Define.PlayerState.Idle])
        {
            vDir = preDir;  // 이전 상태 방향 저장
        }
        else if (StateMachine.CurrentState != dicState[Define.PlayerState.Idle])
        {
            if (Direction == Vector2.zero)  // Idle 넘어오기 직전에 Walk에서 입력이 0,0으로 넘어오는거 방지
                return;
            else
                preDir = Direction;
        }

        // 앞에 아이템이 있는지 확인하는 레이캐스트
        Debug.DrawRay(Rigid.position, vDir, new Color(0, 0, 1));
        RaycastHit2D raycastHit = Physics2D.Raycast(Rigid.position, vDir, 1.2f, LayerMask.GetMask("Item"));

        // 레이캐스트에 맞았다면
        if (raycastHit)
        {
            // 아이템 획득 하기
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Item item = inventory.CreateEmptyItem();            // 비어있는 아이템 생성
                item = raycastHit.collider.gameObject.transform.GetComponent<Item>();
                //Debug.Log("item :" + item.Item_id);
                //Debug.Log("itemtype :" + item.Item_type);
                inventory.PickItem(item);                           // 인벤토리에 아이템 넣기
                raycastHit.collider.gameObject.SetActive(false);    // 필드에 있는 먹은 아이템 비활성화

                // 아이템 획득 사운드 출력
                Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/GenericNotification3", Define.Sound.Effect);

                //Destroy(raycastHit.collider.gameObject);

                // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
                // 절대 Destroy 함부로 하지말것!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            }
        }
    }

    // 키보드키 입력 시 처리 매서드
    void OnKeyInput()
    {
        // 인벤토리창 키기 또는 닫기
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (invenCanvas.gameObject.activeSelf)
            {
                invenCanvas.gameObject.SetActive(false);
                inventory.PlaySoundOpenAndQuitInventoryWindow(false);   // 인벤토리창 닫을 때 사운드 출력
            }
            else
            {
                invenCanvas.gameObject.SetActive(true);
                inventory.PlaySoundOpenAndQuitInventoryWindow(true);    // 인벤토리창 열 때 사운드 출력
            }
        }

        // 대시 기능 , 기능 구현은 아직 못했음
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (StateMachine.CurrentState == dicState[Define.PlayerState.Walk] ||
                StateMachine.CurrentState == dicState[Define.PlayerState.Idle])
            {
                Debug.Log("대시를 눌렀습니다.");
            }
        }

        // 위아래좌우 입력 시 이동상태로 변경
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) &&
            StateMachine.CurrentState != dicState[Define.PlayerState.Attack] && !invenCanvas.gameObject.activeSelf) //인벤토리창 켜졌을 때 이동입력 막기
        {
            StateMachine.SetState(dicState[Define.PlayerState.Walk], this);
        }

        // 스페이스키 눌렀을 때 공격상태로 변경
        if (Input.GetButtonDown("Jump")) 
        {
            StateMachine.SetState(dicState[Define.PlayerState.Attack], this);
            
        }


        //Debug.Log("현재 상태 : " + StateMachine.CurrentState);
    }

    // 대각선으로 이동안되게 하는 매서드
    public Vector2 DecideDirection(float horizontalInput, float verticalInput)
    {
        bool isHorizonMove = false;

        bool hDown = Input.GetButton("Horizontal");
        bool vDown = Input.GetButton("Vertical");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vUp = Input.GetButtonUp("Vertical");

        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (hUp || vUp)
            isHorizonMove = horizontalInput != 0;

        Vector2 moveVec = isHorizonMove ? new Vector2(horizontalInput, 0) : new Vector2(0, verticalInput);
        return moveVec;
    }

    // 마우스 입력 시 처리 매서드
    void OnMouseInput(Define.MouseEvent mouseEvent)
    {

    }

    // 이동 시 같은 애니메이션을 불러서 씹히는 현상 막는 매서드
    public void PreventSameAnimation(float horizontalInput, float verticalInput)
    {
        // * 캐릭터 애니메이션
        Anim.SetInteger("speed", (int)MoveSpeed);
        // 애니메이션이 계속 처음으로 반복되지 않도록 함 ( 애니메이션이 한번 끝까지 실행 되어야 하는데 계속 앞에만 반복되는 현상 때문 )
        if (Anim.GetInteger("hAxisRaw") != horizontalInput)
        {
            Anim.SetBool("isChange", true);
            Anim.SetInteger("hAxisRaw", (int)horizontalInput);
        }
        else if (Anim.GetInteger("vAxisRaw") != verticalInput)
        {
            Anim.SetBool("isChange", true);
            Anim.SetInteger("vAxisRaw", (int)verticalInput);
        }
        else
        {
            Anim.SetBool("isChange", false);
        }
    }

    // Attack 애니메이션
    public IEnumerator AttackCoroutine()
    {
        Anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.3f);

        Anim.SetBool("isAttack", false);

        StateMachine.CurrentState = StateMachine.PreviousState; // 공격 후 이전 상태로 변경

    }

    // 공격 시 처리 매서드
    public void OnAttack(Collider2D collision)
    {
        // 맞은 타겟 ( 몬스터 ) 에게 데미지를 전달하여 처리
        Stat targetStat = collision.gameObject.GetComponent<Stat>();
        bool isDead = targetStat.OnAttacked(stat, Define.ObjectType.Player);

        // 죽었을 때 애니메이션 및 상태 변경
        if (isDead)
        {
            MonsterController monster = collision.gameObject.GetComponent<MonsterController>();
            monster.StateMachine.CurrentState = monster.DicState[Define.MonsterState.Die];
            monster.Anim.SetBool("isDead", true);
            monster.StartCoroutine(DeadDelay(monster));
        }
    }

    // 플레이어가 공격 시 사운드 출력 매서드
    public void PlayAttackSound()
    {
        Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/SlidesAndTransitions/SwooshSlide1b", Define.Sound.Effect);
    }

    // 죽으면 개체가 바로 사라지지 않도록 하는 코루틴
    public IEnumerator DeadDelay(MonsterController monster)
    {
        yield return new WaitForSeconds(0.4f);
        monster.gameObject.SetActive(false);
    }

}

