using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어가 공격 시 타겟과 충돌체크를 하기 위해
public class PlayerHitBox : MonoBehaviour
{
    PlayerController player;

    private void Awake()
    {
        player = gameObject.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)Define.ObjectType.Monster && collision.gameObject.tag == "Slime")
        {
            player.OnAttack(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
}
