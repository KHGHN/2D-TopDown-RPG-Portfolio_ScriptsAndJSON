using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 싱글톤으로 매니저들 구현
public class Managers : MonoBehaviour
{
    static Managers s_Instance;

    static Managers Instance { get { Init();  return s_Instance; } }

    InputManager input = new InputManager();
    DataManager data = new DataManager();
    ResourceManager resource = new ResourceManager();
    InventoryManager inventory = new InventoryManager();
    UIManager ui = new UIManager();
    SoundManager sound = new SoundManager();

    public static InputManager Input { get { return Instance.input; } }
    public static DataManager Data { get { return Instance.data; } }
    public static ResourceManager Resource { get { return Instance.resource; } }
    public static InventoryManager Inventory { get { return Instance.inventory; } }
    public static UIManager UI { get { return Instance.ui; } }
    public static SoundManager Sound { get { return Instance.sound; } }


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        input.OnUpdate();
    }

    static void Init()
    {
        if(s_Instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<Managers>();

            s_Instance.data.Init();

            s_Instance.inventory.Init();

            s_Instance.ui.Init();

            s_Instance.sound.Init();
        }
    }
}
