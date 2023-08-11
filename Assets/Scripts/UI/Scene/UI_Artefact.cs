using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Artefact : UI_Scene
{
    GameObject _Contents;

    enum GameObjects
    {
        Panel
    }

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        GameObject scroll = Util.FindChild(Get<GameObject>((int)GameObjects.Panel), "Scroll");
        _Contents = Util.FindChild(Util.FindChild(scroll, "Viewport"), "Content");
        Manager.UI.Artefact = this;

        for (int i = 0; i < Manager.Artefact.currentArtefacts.Count; i++)
        {
            SetArtefact(i);
        }
    }

    public void AddArtefact()
    {
        SetArtefact(Manager.Artefact.currentArtefacts.Count - 1);
    }

    private void SetArtefact(int num)
    {
        GameObject artefact = Manager.Resource.Instantiate("UI/Scene/Artefact");
        Image artefactimage = Util.GetOrAddComponent<Image>(artefact);
        artefactimage.sprite = Manager.Artefact.Sprites[Manager.Artefact.currentArtefacts[num].spritenum];
        AddUIEvent(artefact, (PointerEventData data) =>
        {
            Manager.UI.SetInfo(true);
            Manager.UI.Info.Setlocal(data.position);
            Manager.UI.Info.text.text = $"{Manager.Artefact.currentArtefacts[num].name_KR}\n{Manager.Artefact.currentArtefacts[num].description}";
        }, Define.UIEvent.On);

        AddUIEvent(artefact, (PointerEventData) =>
        {
            Manager.UI.SetInfo(false);
        }, Define.UIEvent.Exit);

        artefact.transform.SetParent(_Contents.transform, false);
    }
}
