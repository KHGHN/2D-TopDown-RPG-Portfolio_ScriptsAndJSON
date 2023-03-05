using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 몬스터 컨트롤러
public class MonsterController : BaseController
{
   
    public StateMachine<MonsterController> StateMachine { get; set; }   // 몬스터 상태

    private Dictionary<Define.MonsterState, IState<MonsterController>> dicState = new Dictionary<Define.MonsterState, IState<MonsterController>>(); // 상태를 생성하여 딕셔너리에 저장할 용도

    public Dictionary<Define.MonsterState, IState<MonsterController>> DicState { get { return dicState; } set { dicState = value; } }

    private Stat stat;          // 플레이어 스텟

    private float site = 5.0f;  // 플레이어 추적 범위 필드

    [SerializeField]
    public GameObject objTarget { get; set; }   // 추적 범위에 있는 타겟 필드 ( 플레이어 )

    //public Collider2D AttackRangeCollider { get; set; }

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        stat = GetComponent<Stat>();
    }

    void Start()
    {
        Init();

    }

    // 초기화
    void Init()
    {

        // 상태 생성
        IState<MonsterController> idle = new StateIdle<MonsterController>();
        IState<MonsterController> walk = new StateWalk<MonsterController>();
        IState<MonsterController> attack = new StateAttack<MonsterController>();
        IState<MonsterController> attacked = new StateAttacked<MonsterController>();
        IState<MonsterController> die = new StateDie<MonsterController>();

        //키입력 등에 따라서 언제나 상태를 꺼내 쓸 수 있게 딕셔너리에 보관
        dicState.Add(Define.MonsterState.Idle, idle);
        dicState.Add(Define.MonsterState.Walk, walk);
        dicState.Add(Define.MonsterState.Attack, attack);
        dicState.Add(Define.MonsterState.Attacked, attacked);
        dicState.Add(Define.MonsterState.Die, die);

        // 기본 상태 Idle로 설정
        StateMachine = new StateMachine<MonsterController>(idle);

        // DecideIdleOrWalk 매서드 호출 
        Invoke("DecideIdleOrWalk", 3);
    }

    void Update()
    {
        //Debug.Log("현재 슬라임 상태 : " + StateMachine.CurrentState.ToString());

        // 각 상태에 따른 상태 실행
        StateMachine.DoOperateUpdate(this);
    }

    private void FixedUpdate()
    {
        // Idle 상태 체크
        //if (Rb.velocity == Vector2.zero && !(stateMachine.CurrentState == dicState[MonsterState.Attack] ||
        //    stateMachine.CurrentState == dicState[MonsterState.Die]))
        //{
        //    stateMachine.SetState(dicState[MonsterState.Idle], this);
        //}

        // 몬스터 이동시키기
        Rigid.velocity = new Vector2(Direction.x * MoveSpeed, Direction.y * MoveSpeed);
    }


    // Idle인지 Walk인지 결정해서 상태 변경
    public void DecideIdleOrWalk()
    {
        // Idle 또는 Move 선택 Random값 뽑기
        int decideValue = 0;
        decideValue = Random.Range(0, 2);

        // 움직일 방향 Random값 뽑기
        float horizontalInput = Random.Range(-1, 2);
        float verticalInput = Random.Range(-1, 2);

        // 대각선 입력 방지 , 대각선 입력이 들어왔을 때 &연산으로 1이면 Idle상태로 바꿈
        int isDiagonalInput = (int)(Mathf.Abs(horizontalInput)) & (int)(Mathf.Abs(verticalInput));

        // Input이 0,0일 경우 애니메이션은 움직이는데 오브젝트는 가만히 서있는 경우 방지하여 Idle상태로 만듬
        if ((horizontalInput == 0 && verticalInput == 0) || isDiagonalInput == 1)
        {
            decideValue = 0;
        }

        switch ((Define.MonsterState)decideValue)
        {
            case Define.MonsterState.Idle:

                StateMachine.SetState(dicState[Define.MonsterState.Idle], this);
                break;
            case Define.MonsterState.Walk:
                // 위에서 뽑은 방향 전달
                Direction = Direction = new Vector2(horizontalInput, verticalInput).normalized;
                StateMachine.SetState(dicState[Define.MonsterState.Walk], this);
                break;
        }

        // 같은 매서드를 호출하여 스스로 돌아다니게 만듬
        Invoke("DecideIdleOrWalk", 3);


    }

    // Idle 또는 Walk 상태에서 추적범위에 플레이어가 있는지 검색
    public void FindProcess()
    {
        // 플레이어가 추적범위에 들어왔는지 확인하는 필드
        Collider2D AttackRangeCollider = Physics2D.OverlapCircle(this.transform.position, site, LayerMask.GetMask("Player"));

        // 오브젝트가 앞에 있는지 확인하는 레이캐스트
        Debug.DrawRay(Rigid.position, Direction, new Color(1, 0, 0));
        RaycastHit2D raycastHitToObject = Physics2D.Raycast(Rigid.position, Direction, 0.7f, LayerMask.GetMask("Object"));

        // 캐릭터 추적 범위에 들어왔는지
        if (AttackRangeCollider) 
        {
            CancelInvoke();                                                     // Attack 상태로 가야하기 때문에 앞에서 호출하고 있던 매서드들 취소
            objTarget = AttackRangeCollider.gameObject;                         // 타겟을 플레이어로 설정
            StateMachine.CurrentState = dicState[Define.MonsterState.Attack];   // 어택상태로 변경
            

        }
        // 앞에 장애물 있는데 계속 뛰는 애니메이션 방지
        if (raycastHitToObject && StateMachine.CurrentState == dicState[Define.MonsterState.Walk])
        {
            MoveSpeed = 0;                                                      // 앞에 장애물이 있으니 멈추게
            StateMachine.CurrentState = dicState[Define.MonsterState.Idle];     // Idle상태로 변경

        }
    }

    // 어택상태에서 추적범위에 플레이어를 쫓아가고 공격하고 플레이어가 도망갔다고 판단하는 매서드
    public void TargetTrackingProcess()
    {
        // 모든 조건은 어택상태에서만 이루어지게 만듬, 상태가 바뀌면서 Update매서드로 이전 상태가 한번 더 들어오는거 방지
        if (StateMachine.CurrentState == dicState[Define.MonsterState.Attack])
        {
            if (objTarget != null) // 플레이어가 타겟이 됐다면
            {
                Collider2D AttackRangeCollider = Physics2D.OverlapCircle(this.transform.position, site, LayerMask.GetMask("Player"));

                // 쫓아가는 방향, 위치벡터 구하기
                Vector2 vTargetPos = objTarget.transform.position;
                Vector2 vPos = this.transform.position;
                Vector2 vDist = vTargetPos - vPos;
                Vector2 vDir = vDist.normalized;
                float fDist = vDist.magnitude;
                float fMove = MoveSpeed * Time.deltaTime;

                // 추적범위 내에서 플레이어가 앞에 있는지 확인하는 레이캐스트
                Debug.DrawRay(Rigid.position, vDir, new Color(1, 0, 0));
                RaycastHit2D raycastHitToPlayer = Physics2D.Raycast(Rigid.position, vDir, 0.7f, LayerMask.GetMask("Player"));

                if (fDist > fMove && AttackRangeCollider && !raycastHitToPlayer)   // 플레이어 쫓아가기, 추적범위 내이면서 레이캐스트에 맞지 않았다면 계속 쫓아감
                {
                    //Debug.Log("쫓아가기!!");

                    MoveSpeed = 2.0f;

                    // -1과 1값이 나오도록 만들기
                    Direction = new Vector2(Mathf.Round(vDir.x), Mathf.Round(vDir.y));

                    // 같은 애니메이션을 계속 불러서 애니메이션이 씹히는 현상 막기
                    PreventSameAnimation();

                    

                }
                else if (AttackRangeCollider && raycastHitToPlayer) // 공격범위에 들어왔다면 공격
                {
                    if (raycastHitToPlayer == false)                // 만약 플레이어가 추적범위에서 벗어 났다면
                        return;

                    if (!Anim.GetBool("isAttacked"))                // 공격 당하지 않았다면
                    {
                        //Debug.Log("공격 스탠드!!");
                        MoveSpeed = 0;                              // 제자리에서 공격하도록 함
                        Anim.SetBool("isChange", false);
                        Anim.SetBool("isIdle", false);              // 제자리에서 Idle상태로 변경되는것 막기
                        Anim.SetBool("isAttackStand", true);        // 제자리 공격 애니메이션 상태로 변경
                        StartCoroutine(AttackCoroutine());          // 공격 애니메이션을 위한 코루틴
                        Anim.SetBool("isAttackStand", false);

                        
                    }
                }
                else if (AttackRangeCollider == false && raycastHitToPlayer == false) // 플레이어가 추적범위에도 없고 공격범위에도 없다면
                {
                    StartCoroutine(StopAttackState());  // 따라가다가 일정시간 되면 공격 멈추게 하는 코루틴
                }
            }
            else if (objTarget == null)                 // 타겟이 없다면 Idle상태로 변경하고 Idle과 Walk중에 결정하는 매서드 다시 호출
            {
                StateMachine.CurrentState = dicState[Define.MonsterState.Idle];

                Invoke("DecideIdleOrWalk", 3);
                return;
            }
        }

        
    }

    // 따라가다가 일정시간 되면 공격 멈추게 하는 코루틴
    private IEnumerator StopAttackState()
    {

        yield return new WaitForSeconds(3);
        Debug.Log("추적 종료");
        objTarget = null;
        MoveSpeed = 0;
        Anim.SetBool("isIdle", true);
        Anim.SetBool("isChange", true);
        Anim.SetBool("isAttack", false);
        Anim.SetBool("isAttackStand", false);
    }

    // 몬스터가 이동할 때 같은 애니메이션 계속 불러서 씹히는 현상 막기
    public void PreventSameAnimation()
    {
        // * 캐릭터 애니메이션
        Anim.SetInteger("speed", (int)MoveSpeed);
        // 애니메이션이 계속 처음으로 반복되지 않도록 함 ( 애니메이션이 한번 끝까지 실행 되어야 하는데 계속 앞에만 반복되는 현상 때문 )
        if (Anim.GetInteger("hAxisRaw") != Direction.x)
        {
            Anim.SetBool("isChange", true);
            Anim.SetInteger("hAxisRaw", (int)Direction.x);
        }
        else if (Anim.GetInteger("vAxisRaw") != Direction.y)
        {
            Anim.SetBool("isChange", true);
            Anim.SetInteger("vAxisRaw", (int)Direction.y);
        }
        else
        {
            Anim.SetBool("isChange", false);
        }
    }

    // Attack 애니메이션 코루틴
    public IEnumerator AttackCoroutine()
    {
        Anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.5f);
        Anim.SetBool("isAttack", false);


    }

    // 공격했을 때 처리 매서드
    public void OnAttack(Collider2D collision)
    {
        // 맞은 타겟 ( 플레이어 ) 에게 데미지를 전달하여 처리
        PlayerStat targetStat = collision.gameObject.GetComponent<PlayerStat>();
        bool isDead = targetStat.OnAttacked(stat, Define.ObjectType.Monster);

        // 죽었을 때 애니메이션 및 상태 변경
        if(isDead)
        {
            //Debug.Log("플레이어가 죽었습니다");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.StateMachine.CurrentState = player.DicState[Define.PlayerState.Die];
            player.Anim.SetBool("isDead", true);
            player.StartCoroutine(DeadDelay(player));
        }
    }

    // 몬스터(슬라임)가 공격 시 사운드 출력
    public void PlayAttackSound()
    {
        Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/GenericNotification11", Define.Sound.Effect);
    }


    // 죽으면 개체가 바로 사라지지 않도록 하는 코루틴
    public IEnumerator DeadDelay(PlayerController player)
    {
        yield return new WaitForSeconds(0.4f);
        player.gameObject.SetActive(false);
    }

    // 플레이어 추적범위에 있는 원을 화면에 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, site);
    }


}
