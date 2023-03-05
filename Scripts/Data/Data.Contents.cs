using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// JSON 데이터 처리를 위한 클래스들
namespace Data
{

    #region Stat
    [Serializable]          // 리플렉션 
    public class Stat
    {
        public int level;       // 반드시 public으로 해야 밑에 FromJson이 작동함 public이 아니라 private으로 하고 싶다면 [SerializeField]를 변수에 붙이자
        public int maxHp;
        public int attack;
        public int totalExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
            {
                dict.Add(stat.level, stat);
            }
            return dict;
        }
    }
    #endregion

    #region Item
    [Serializable]          // 리플렉션 
    public class Item
    {
        // 반드시 public으로 해야 밑에 FromJson이 작동함 public이 아니라 private으로 하고 싶다면 [SerializeField]를 변수에 붙이자
        public int item_id;                 // 아이템 ID
        public string item_name;            // 아이템 이름
        public string item_description;     // 아이템 설명
        public int item_type;               // 아이템 타입
        public int max_count;               // 최대 개수
        public int effect;                  // 사용 시 효과를 구현하기 위한 필드
    }

    [Serializable]
    public class ItemData : ILoader<int, Item>
    {
        public List<Item> items = new List<Item>();

        public Dictionary<int, Item> MakeDict()
        {
            Dictionary<int, Item> dict = new Dictionary<int, Item>();
            foreach (Item item in items)
            {
                //Debug.Log("type : "+ item.item_type);
                dict.Add(item.item_id, item);
            }
            return dict;
        }
    }
    #endregion
}
