using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {

        // 1. Origianl을 이미 들고 있으면 바로 사용
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to Load prefab : {path}");
            return null;
        }

        //// 2. 혹시 풀링된 오브젝트가 있나?
        //if (original.GetComponent<Poolable>() != null)              // poolable 컴포넌트를 가지고 있으면 풀링된 오브젝트가 있으니까 반환해줌
        //    return Managers.Pool.Pop(original, parent).gameObject;  // 처음 실행하는거면 생성하면서 반환해주고 아니면 반환만 해줌


        GameObject go = Object.Instantiate(original, parent);   // 앞에 Object를 붙인 이유는 유니티에 만들어져 있는 함수가 아니라 위에 있는 Instantiate 함수를 호출하기 때문
        //int idx = go.name.IndexOf("(Clone)");
        //if (idx > 0)
        //    go.name = go.name.Substring(0, idx);        // substring을 하고 다시 받아서 저장해줘야함
        go.name = original.name;

        return go;
    }
}
