using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 베이스 Stat 
public class Stat : MonoBehaviour
{
    public int Level { get ; set ; }
    public int Hp { get ; set ; }
    public int MaxHp { get ; set; }
    public int Attack { get ; set; }
    public int Defense { get ; set; }
    public float MoveSpeed { get; set; }

    private void Start()
    {
        Level = 1;
        Hp = 100;
        MaxHp = 100;
        Attack = 50;
        Defense = 5;
    }

    // 공격당했을 때
    public virtual bool OnAttacked(Stat attacker,Define.ObjectType objectType)
    {
        if(gameObject == null)
        {
            return false;
        }
        bool isDead = false;

        int damage = Mathf.Max(0, attacker.Attack - Defense); // 음수가 안되게 처리 // 음수가 되면 0이 더 크니까 0을 리턴하게 되니까
        Hp -= damage;

        // 공격당했을 때 애니메이션과 사운드 관련 코루틴 실행
        attacker.StartCoroutine(OnAttackedCoroutine(attacker, objectType));

        //Debug.Log(attacker.name + "가 데미지" + damage + "를 줘서 HP : " + this.Hp + "가 되었습니다");
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attacker);

            // 죽었으면 true 반환
            isDead = true;  
        }

        return isDead;

    }

    // 공격당했을 때 애니메이션과 사운드 관련
    public virtual IEnumerator OnAttackedCoroutine(Stat attacker,Define.ObjectType objectType)
    {
        Animator anim = this.gameObject.GetComponent<Animator>();

        if (objectType == Define.ObjectType.Player)
        {
            //Debug.Log("monster Attacked");
            MonsterController monster = this.gameObject.GetComponent<MonsterController>();
            monster.StateMachine.CurrentState = monster.DicState[Define.MonsterState.Attacked];
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/SlidesAndTransitions/Minimize3", Define.Sound.Effect);
        }
        else if(objectType == Define.ObjectType.Monster)
        {
            //Debug.Log("player Attacked");
            PlayerController player = this.gameObject.GetComponent<PlayerController>();
            player.StateMachine.CurrentState = player.DicState[Define.PlayerState.Attacked];
            Managers.Sound.Play("Sounds/Effect/Cyberleaf-ModernUISFX/AlertsAndNotifications/SciFiNotification2", Define.Sound.Effect);
        }

        anim.SetBool("isAttacked", true);

        yield return new WaitForSeconds(2.5f);

        anim.SetBool("isAttacked", false);

    }

    // 몬스터가 죽었으면 플레이어에게 경험치 제공
    protected virtual void OnDead(Stat attacker)
    {

        // 경험치를 주기 위해
        PlayerStat playerStat = attacker as PlayerStat;
        if (playerStat != null)
        {
            playerStat.Exp += 100;            // 보통 몬스터의 exp 정보는 데이터형태로 들고 있다.
        }

        //Managers.Game.Despawn(gameObject); // Stat을 들고 있는 오브젝트를 디스폰해야 되니까

    }

    
}
