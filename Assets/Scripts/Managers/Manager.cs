using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    static Manager s_instance;
    static Manager Instance { get { Init(); return s_instance; } }//Manager 가져오기

    SceneManagerEx _scene = new SceneManagerEx();
    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();
    ItemManager _item = new ItemManager();
    ArtefactManager _artefact = new ArtefactManager();
    BattleManager _battle = new BattleManager();
    DataManager _data = new DataManager();
    SoundManager _sound = new SoundManager();
    RewardManager _reward = new RewardManager();
    UnknownEventManager _unknown = new UnknownEventManager();

    public static BattleManager Battle { get { return Instance._battle; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static ItemManager Item { get { return Instance._item; } }
    public static ArtefactManager Artefact { get { return Instance._artefact; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static DataManager Data { get { return Instance._data; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static RewardManager Reward { get { return Instance._reward;} }
    public static UnknownEventManager Unknown { get { return Instance._unknown; } }

    void Start()
    {
        Init();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Manager");
            if (go == null)
            {
                go = new GameObject { name = "@Manager" };
                go.AddComponent<Manager>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Manager>();
            s_instance._data.Init();
            s_instance._sound.Init();
        }
    }
}
