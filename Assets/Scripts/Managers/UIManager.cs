using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{//sort order 관리

    int _order = 10;
    public bool Issetting = false;
    Stack<UI_PopUp> _popupstack = new Stack<UI_PopUp>();
    UI_PopUp _popup = null;
    Stack<UI_Scene> _scenestack = new Stack<UI_Scene>();
    //전투 씬의 Dialog와 Button 관리
    public UI_Panel Panel;
    //맵 씬의 Dialog와 Button 관리
    public UI_MapDialog MapDialog;
    public UI_Artefact Artefact;
    public Info Info;

    public GameObject Root(string name = "Root")
    {
        GameObject root = GameObject.Find($"@UI_{name}");
        if (root == null)
            root = new GameObject { name = $"@UI_{name}" };
        return root;
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {//order를 관리함
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Util.GetOrAddComponent<CanvasScaler>(go).uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.overrideSorting = true;//캔버스 안에 캔버스가 있을 때 자신의 sort order를 가질 것
        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }

        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public void SetDialog(string dialog)
    {
        if (Panel != null)
            Panel.Dialog(dialog);

        else if(MapDialog != null)
            MapDialog.Dialog(dialog);
    }

    public void SetDialog(string dialog, float time)
    {
        StartCoroutine(SetDialogCoroutine(dialog, time));
    }

    IEnumerator SetDialogCoroutine(string dialog, float time)
    {
        yield return new WaitForSeconds(time);

        if (Panel != null)
            Panel.Dialog(dialog);

        else if (MapDialog != null)
            MapDialog.Dialog(dialog);
    }

    public void SetButton(Define.BattleState state)
    {
        if (Panel != null)
            Panel.SetButton(state);
    }

    public void AddUIArtefact()
    {
        Artefact.AddArtefact();
    }

    public void SetInfo(bool isactive)
    {
        Info.gameObject.SetActive(isactive);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Manager.Resource.Instantiate($"UI/Scene/{name}");

        T scene = Util.GetOrAddComponent<T>(go);
        _scenestack.Push(scene);
        //부모를 만들고 넣기
        go.transform.SetParent(Root("Scene").transform, false);

        return scene;
    }

    public T ShowPopUpUI<T>(string name = null) where T : UI_PopUp
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Manager.Resource.Instantiate($"UI/PopUp/{name}");

        T popup = Util.GetOrAddComponent<T>(go);
        
        _popupstack.Push(popup);
        
        go.transform.SetParent(Root("PopUp").transform, false);
        return popup;
    }

    /// <summary>  
    /// 마지막으로 열린 UI를 닫는다
    /// </summary>
    /// <param name="lbl"></param>
    public void ClosePopUpUI()
    {
        if (_popupstack.Count == 0)
            return;//팝업창이 열려있지 않으면 닫을 게 없음

        _popup = _popupstack.Pop();
        Manager.Resource.Destroy(_popup.gameObject);
        _popup = null;

        _order--;
    }

    public void CloseAllPopUpUI() {
        while(_popupstack.Count != 0)
            ClosePopUpUI();
    }

    public void CloseAllSceneUI()
    {
        while (_scenestack.Count != 0)
        {
            UI_Scene scene = _scenestack.Pop();
            Manager.Resource.Destroy(scene.gameObject);
            scene = null;
        }
    }

    public void Clear()
    {
        CloseAllPopUpUI();
        CloseAllSceneUI();
    }
}
