using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여러가지 enum들
public class Define
{
    // 플레이어 상태
    public enum PlayerState
    {
        Idle,
        Walk,
        Dash,
        Attack,
        Attacked,
        Die,
    }

    // 몬스터 상태
    public enum MonsterState
    {
        Idle,
        Walk,
        Attack,
        Attacked,
        Die,
    }

    // 오브젝트 타입
    public enum ObjectType
    {
        Player = 6,
        Monster = 7,
    }

    // 마우스 클릭 이벤트 관련 ( InputManager 에서 씀 )
    public enum MouseEvent
    {
        Press,
        PointerDown,            // 맨처음 누를때
        PointerUp,              // 맨처음 누르고 땔 때
        Click,
    }
    
    // 아이템 타입
    public enum ItemType
    {
        Consumables,
        Equipment,
        IncreasableCount,
        Quest,
        ETC,
        Empty,
    }

    // 아이템 ID
    public enum ItemID
    {
        SmallHpPotion = 101
    }

    // 사운드
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

}
