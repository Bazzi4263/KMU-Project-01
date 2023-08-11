using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    protected abstract void Init();

    //오브젝트 사전에 추가
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] name = Enum.GetNames(type);

        UnityEngine.Object[] objects = new UnityEngine.Object[name.Length];
        _objects.TryAdd(typeof(T), objects);
        for (int i = 0; i < name.Length; i++)
        {
            if(typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, name[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, name[i], true);
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    //자주 사용하니 좀 더 편하게 이용 가능
    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected TextMeshProUGUI GetTextMeshProUGUI(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }

    //UIEvent type에 따라 action에 설정해둔 행동이 이벤트형식으로 뿌려져서 실행됨
    public void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
            case Define.UIEvent.On:
                evt.OnEnterHandler -= action;
                evt.OnEnterHandler += action;
                break;
            case Define.UIEvent.EndDrag:
                evt.EndDragHandler -= action;
                evt.EndDragHandler += action;
                break;
            case Define.UIEvent.BeginDrag:
                evt.BeginDragHandler -= action;
                evt.BeginDragHandler += action;
                break;
            case Define.UIEvent.Exit:
                evt.ExitHandler -= action;
                evt.ExitHandler += action;
                break;
                //((PointerEventData data) => { evt.gameObject.transform.position = data.position; });
        }
    }
}
