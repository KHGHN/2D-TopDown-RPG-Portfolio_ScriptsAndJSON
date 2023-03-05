using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 스텟
public class PlayerStat : Stat
{
    public int Gold { get; set; }
    public int Exp { get; set; }

    void Start()
    {
        Level = 1;
        Exp = 0;
        Defense = 5;
        Gold = 0;

        SetStat(Level);
    }

    // 스텟 세팅
    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;

        Data.Stat stat = dict[level];


        Hp = stat.maxHp;
        MaxHp = stat.maxHp;
        Attack = stat.attack;

    }

    // 죽었을 때
    protected override void OnDead(Stat attacker)
    {
        Debug.Log("Player Dead");
    }
}
