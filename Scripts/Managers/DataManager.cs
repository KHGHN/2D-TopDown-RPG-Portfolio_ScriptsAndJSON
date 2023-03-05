using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 데이터 매니저
public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();      // 나중에 Monster나 npc나 할때 id로 접근해서 빠르게 추출할 수 잇다.
    public Dictionary<int, Data.Item> ItemDict { get; private set; } = new Dictionary<int, Data.Item>();      // 나중에 Monster나 npc나 할때 id로 접근해서 빠르게 추출할 수 잇다.

    //StatData data;
    public void Init()
    {
        //TextAsset textAsset =  Managers.Resource.Load<TextAsset>($"Data/StatData");

        // 파일에서 클래스형태로 바꿔주는거가 FromJson
        //StatData data = JsonUtility.FromJson<StatData>(textAsset.text);         // C++에서는 이렇게 하려면 엄청난 작업과 노가다가 필요하다고 함..
        //Debug.Log(textAsset.text);

        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        ItemDict = LoadJson<Data.ItemData, int, Data.Item>("ItemData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }

    
}
