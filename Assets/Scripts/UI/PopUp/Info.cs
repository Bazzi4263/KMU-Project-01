using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Info : UI_PopUp
{
    public TextMeshProUGUI text;
    private Image image;

    enum Texts
    {
        Text
    }

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Manager.UI.Info = this;
        Bind<TextMeshProUGUI>(typeof(Texts));
        image = GetImage((int)Define.Images.Image);
        text = GetTextMeshProUGUI((int)Texts.Text);
        gameObject.SetActive(false);
    }

    public void Setlocal(Vector2 local)
    {
        image.gameObject.transform.position = local;
        text.gameObject.transform.position = local;
    }
}
