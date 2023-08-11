using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class CharGUI : MonoBehaviour
{
    public Image curhpbar, curtgbar, curshieldbar;
    float curhp, maxhp, curtg, totalhp, curshield;
    Character chara;
    GameObject charname;
    TextMeshProUGUI hptext;
    GameObject hp, buffs;
    Color greencolor;
    Canvas canvas;
    RectTransform rect;

    void Awake()
    {   
        chara = transform.parent.gameObject.GetComponent<Character>();
        curtg = 0f;
        curshield = 0f;
        hp = transform.Find("hptg").gameObject;
        buffs = transform.Find("buffs").gameObject;
        curhpbar = transform.Find("hptg/curHp").gameObject.GetComponent<Image>();
        curhpbar.fillAmount = 1f;
        curshieldbar = transform.Find("hptg/curShield").gameObject.GetComponent<Image>();
        curshieldbar.fillAmount = 0f;
        greencolor = curhpbar.color;
        curtgbar = transform.Find("hptg/curTg").gameObject.GetComponent<Image>();
        curtgbar.fillAmount = 0f;
        hptext = transform.Find("hptg/HPtext").gameObject.GetComponent<TextMeshProUGUI>();
        charname = transform.Find("charname").gameObject;
        canvas = GetComponent<Canvas>();
        rect = GetComponent<RectTransform>();
        switch (chara)
        {
            case Monster _:
                hp.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 1.07f, 0);
                buffs.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0.18f, 0);
                //charname.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0.23f, 0);
                break;
            case Player _:
                charname.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -0.85f, 0);
                break;
        }
        HideGUI();
    }
    private void Start()
    {
        canvas.sortingLayerName = "Characters";
        curhp = maxhp = chara._stats.maxhp;
        hptext.text = $"HP: {curhp}/{maxhp}";
        charname.GetComponent<TextMeshProUGUI>().text = chara._stats.name;
        switch (chara)
        {
            case Monster _:
                rect.localScale = new Vector2(4 / chara.transform.localScale.x, 4 / chara.transform.localScale.y);
                rect.sizeDelta = new Vector2(chara.gameObject.GetComponent<BoxCollider2D>().size.x / rect.localScale.x
                    , chara.gameObject.GetComponent<BoxCollider2D>().size.y / rect.localScale.y);
                break;
            case Player _:
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
        maxhp = chara._stats.maxhp;

    }
    // Update is called once per frame
    void Update()
    {
        totalhp = curhp + curshield;

        if (curhp > chara._stats.currenthp)
        {
            curhp -= maxhp / 100;
            if (curhp < chara._stats.currenthp) curhp = chara._stats.currenthp;
            curhp = (float)System.Math.Round(curhp, 2);
        }
        else if (curhp < chara._stats.currenthp)
        {
            curhp += maxhp / 100f;
            if (curhp > chara._stats.currenthp) curhp = chara._stats.currenthp;
            curhp = (float)System.Math.Round(curhp, 2);
        }
        if (curshield > chara.shield)
        {
            curshield -= chara.shield / 100;
            if (curshield < chara.shield) curshield = chara.shield;
            curshield = (float)System.Math.Round(curshield, 2);
        }
        else if (curshield < chara.shield)
        {
            curshield += chara.shield / 100;
            if (curshield > chara.shield) curshield = chara.shield;
            curshield = (float)System.Math.Round(curshield, 2);
        }
        if (chara.shield != 0)
        {
            if (curhp == maxhp)
            {
                curhpbar.fillAmount = 0.9f;
                curshieldbar.fillAmount = 1f;
            }
            else if (curhp / maxhp < 0.9f)
            {
                curhpbar.fillAmount = curhp / maxhp;
                curshieldbar.fillAmount = totalhp / maxhp;
            }
        }
        else
        {
            curshieldbar.fillAmount = 0f;
            curhpbar.fillAmount = curhp / maxhp;
        }
        hptext.text = $"HP: {curhp}/{maxhp}";
        if (curtg != chara.turngauge)
        {
            curtg += 0.5f;
            if (curtg > chara.turngauge) curtg = chara.turngauge;
            curtg = (float)System.Math.Round(curtg, 2);
            curtgbar.fillAmount = curtg / 100;
        }
        if (curhp / maxhp < 0.3f)
        {
            curhpbar.color = Color.red;
        }
        else if (curhp / maxhp < 0.6f)
        {
            curhpbar.color = Color.yellow;
        }
        else if (curhp / maxhp > 0.6f)
        {
            curhpbar.color = greencolor;
        }
    }

    public void ShowGUI()
    {
        hptext.DOFade(1, 0.1f);
        charname.GetComponent<TextMeshProUGUI>().DOFade(1, 0.1f);
    }
    public void HideGUI()
    {
        hptext.DOFade(0, 0.1f);
        charname.GetComponent<TextMeshProUGUI>().DOFade(0, 0.1f);
    }

    public void SetBuffGUI(Basebuff buff)
    {
        GameObject buf = new GameObject();
        buf.name = buff.GetType().Name;
        buf.AddComponent<Image>().sprite = buff.sprite;
        buf.transform.SetParent(buffs.transform);
        buf.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        if (buff.stackable)
        {
            GameObject bufstack = new GameObject();
            bufstack.name = "stack";
            TextMeshProUGUI bufstacktxt = bufstack.AddComponent<TextMeshProUGUI>();
            bufstack.transform.SetParent(buf.transform);
            Util.GetOrAddComponent<RectTransform>(bufstack).sizeDelta = new Vector2(1.5f, 1.5f);
            bufstacktxt.alignment = TextAlignmentOptions.BottomRight;
            bufstacktxt.text = "x1";
            bufstacktxt.fontSize = 0.7f;
        }
    }

    public void UpdateBuffStack(Basebuff buff)
    {
        buffs.transform.Find(buff.GetType().Name).Find("stack").GetComponent<TextMeshProUGUI>().text = $"x{buff.stack}";
    }

    public void DelBuffGUI(Basebuff buff)
    {
        Destroy(buffs.transform.Find(buff.GetType().Name).gameObject);
    }
}
