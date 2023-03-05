using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터가 공격 시 타겟과 충돌체크를 하기 위해
public class MonsterHitBox : MonoBehaviour
{
    MonsterController monster;

    private void Awake()
    {
        monster = gameObject.GetComponentInParent<MonsterController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)Define.ObjectType.Player && collision.gameObject.tag == "Player")
        {
            monster.OnAttack(collision);
        }
    }
}
