using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


// 상태변경 처리
public class StateMachine <T>
{
    public IState<T> CurrentState { get; set; }
    public IState<T> PreviousState { get; set; }

    public StateMachine(IState<T> defaultState)
    {
        CurrentState = defaultState;
        PreviousState = defaultState;
    }

    public void SetState(IState<T> state, T state_object)
    {

        // 같은 행동을 연이어 세팅하지 못하도록 예외처리
        if (CurrentState == state)
        {
            //Debug.Log("현재 이미 해당 상태입니다" + "[ 현재 상태 : " + state + " ]");
            return;
        }


        // 다음 상태로 바뀌기전에 현 상태의 Exit를 호출
        CurrentState.OperateExit(state_object);

        // 이전 상태 저장
        PreviousState = CurrentState;

        // 다음 상태 교체
        CurrentState = state;

        // 다음 상태의 Enter를 호출한다.
        CurrentState.OperateEnter(state_object);

    }

    // 상태 업데이트를 위해 매프레임마다 각 Controller의 Update함수에서 불려질 함수
    public void DoOperateUpdate(T state_object)
    {
        //if ((typeof(T) == typeof(PlayerController)) && state_object != null)
        //{
        //    PlayerController player = state_object as PlayerController;
        //    PlayerStat playerStat = player.GetComponent<PlayerStat>();
        //}

        //if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        //{
        //    MonsterController monster = state_object as MonsterController;
        //    Stat monsterStat = monster.GetComponent<Stat>();
        //}

        CurrentState.OperateUpdate(state_object);
    }


}

public interface IState<T>
{
    void OperateEnter(T state_object);
    void OperateExit(T state_object);
    void OperateUpdate(T state_object);
}

public class StateIdle<T> : IState<T> 
{
    public void OperateEnter(T state_object)
    {
        if ((typeof(T) == typeof(PlayerController)) && state_object != null)
        {
            PlayerController player = state_object as PlayerController;
            player.MoveSpeed = 0;

        }

        if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        {

            MonsterController monster = state_object as MonsterController;

            monster.MoveSpeed = 0;
            //monster.Direction = Vector2.zero;

        }

    }

    public void OperateExit(T state_object)
    {

    }
    public void OperateUpdate(T state_object)
    {
        if((typeof(T) == typeof(PlayerController)) && state_object != null)
        {
            PlayerController player = state_object as PlayerController;

            // 캐릭터 애니메이션
            player.Anim.SetInteger("speed", (int)player.MoveSpeed);
            player.Anim.SetInteger("hAxisRaw", 0);
            player.Anim.SetInteger("vAxisRaw", 0);
            player.Anim.SetBool("isChange", false);
        }

        if((typeof(T) == typeof(MonsterController)) && state_object != null)
        {

            MonsterController monster = state_object as MonsterController;

            monster.FindProcess();

            // 몬스터 애니메이션
            monster.Anim.SetInteger("speed", (int)monster.MoveSpeed);
            monster.Anim.SetInteger("hAxisRaw", 0);
            monster.Anim.SetInteger("vAxisRaw", 0);
            monster.Anim.SetBool("isChange", false);
        }
        
    }
}

public class StateWalk<T> : IState<T>
{
    public void OperateEnter(T state_object)
    {
        if ((typeof(T) == typeof(PlayerController)) && state_object != null)
        {
            PlayerController player = state_object as PlayerController;
            player.MoveSpeed = 6.0f;
        }

        if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        {
            MonsterController monster = state_object as MonsterController;
            monster.MoveSpeed = 2.0f;

        }

    }

    public void OperateExit(T state_object)
    {

    }
    public void OperateUpdate(T state_object)
    {

        if ((typeof(T) == typeof(PlayerController)) && state_object != null)
        {
            PlayerController player = state_object as PlayerController;

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");


            //player.Direction = new Vector2(horizontalInput, verticalInput).normalized;
            player.Direction = player.DecideDirection(horizontalInput,verticalInput);

            player.PreventSameAnimation(horizontalInput, verticalInput);
        }

        if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        {
            MonsterController monster = state_object as MonsterController;

            monster.FindProcess();

            monster.PreventSameAnimation();
        }



    }
}

public class StateDash <T> : IState<T>
{
    public void OperateEnter(T state_object)
    {

    }

    public void OperateExit(T state_object)
    {

    }
    public void OperateUpdate(T state_object)
    {

    }
}

public class StateMove<T> : IState<T>
{
    public void OperateEnter(T state_object)
    {

    }

    public void OperateExit(T state_object)
    {

    }
    public void OperateUpdate(T state_object)
    {

    }
}

public class StateAttack<T> : IState<T>
{
    public void OperateEnter(T state_object)
    {

        //Debug.Log("여기 안들어오니??");
        //float horizontalInput = 0;
        //float verticalInput = 0;

        if ((typeof(T) == typeof(PlayerController)) && state_object != null)
        {
            PlayerController player = state_object as PlayerController;

            player.StartCoroutine(player.AttackCoroutine());

        }
        
        if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        {
            MonsterController monster = state_object as MonsterController;


        }


    }

    public void OperateExit(T state_object)
    {
        if ((typeof(T) == typeof(PlayerController)) && state_object != null)
        {
            PlayerController player = state_object as PlayerController;
        }

        if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        {
            MonsterController monster = state_object as MonsterController;
        }

    }
    public void OperateUpdate(T state_object)
    {
        if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        {
            MonsterController monster = state_object as MonsterController;

            monster.TargetTrackingProcess();
            

        }
    }
}

public class StateAttacked<T> : IState<T>
{
    public void OperateEnter(T state_object)
    {

    }

    public void OperateExit(T state_object)
    {

    }
    public void OperateUpdate(T state_object)
    {
        if ((typeof(T) == typeof(PlayerController)) && state_object != null)
        {
            PlayerController player = state_object as PlayerController;

            player.MoveSpeed = 0;
            player.Rigid.velocity = Vector2.zero;
            
            if (player.Anim.GetBool("isAttacked") == false)     // 공격 당하는 상태가 아니면 이전 상태로 복귀
            {
                player.StateMachine.CurrentState = player.StateMachine.PreviousState;
            }
        }

        if ((typeof(T) == typeof(MonsterController)) && state_object != null)
        {
            MonsterController monster = state_object as MonsterController;

            monster.MoveSpeed = 0;
            monster.Rigid.velocity = Vector2.zero;

            if (monster.Anim.GetBool("isAttacked") == false)    // 공격 당하는 상태가 아니면 이전 상태로 복귀
            {
                monster.StateMachine.CurrentState = monster.StateMachine.PreviousState;
            }
        }
            
    }
}


public class StateDie<T> : IState<T>
{
    public void OperateEnter(T state_object)
    {

    }

    public void OperateExit(T state_object)
    {

    }
    public void OperateUpdate(T state_object)
    {

    }
}