using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public static Define.Scenes Scene { get; protected set; } = Define.Scenes.UnKnownScene;

    protected virtual void Init()
    {
        //Scene에 EventSystem추가
        UnityEngine.Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Manager.Resource.Instantiate("EventSystem").name = "@EventSystem";

        //Scene에 UI를 담는 GameObject생성
        GameObject go = Manager.UI.Root();
        GameObject go1 = Manager.UI.Root("Scene");
        GameObject go2 = Manager.UI.Root("PopUp");
        go1.transform.SetParent(go.transform);
        go2.transform.SetParent(go.transform);
    }

    public virtual void Clear(Define.Scenes type)
    {
        Manager.UI.Clear();
        Manager.Sound.Clear();
        Scene = type;
    }
}
