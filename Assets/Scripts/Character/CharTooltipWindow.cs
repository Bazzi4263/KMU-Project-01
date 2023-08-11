using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class CharTooltipWindow : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;

    public static CharTooltipWindow Instance;
    private RectTransform rect;
    RectTransform buffrect;
    public Character chara;
    List<GameObject> bufficons = new List<GameObject>();
    GameObject infotext;
    TextMeshProUGUI charinfo, attackpower, armor, speed, turngauge, exp;

    private void Awake()
    {
        Instance = this;
        rect = transform.GetComponent<RectTransform>();
        transform.SetParent(GameObject.Find("Canvas").gameObject.transform);
        canvasRect = GameObject.Find("Canvas").gameObject.GetComponent<RectTransform>();
        buffrect = GameObject.Find("Buff").gameObject.GetComponent<RectTransform>();
        bufficons = Util.GetAllChilds(transform.Find("Buff/buffs").gameObject);
        infotext = GameObject.Find("Buff/infotext").gameObject;
        charinfo = GameObject.Find("Stat/infotext").GetComponent<TextMeshProUGUI>();
        attackpower = GameObject.Find("Stat/stats/AttackPower").GetComponent<TextMeshProUGUI>();
        armor = GameObject.Find("Stat/stats/Armor").GetComponent<TextMeshProUGUI>();
        speed = GameObject.Find("Stat/stats/Speed").GetComponent<TextMeshProUGUI>();
        turngauge = GameObject.Find("Stat/stats/TurnGauge").GetComponent<TextMeshProUGUI>();
        exp = GameObject.Find("Stat/stats/Exp").GetComponent<TextMeshProUGUI>();
        foreach (GameObject b in bufficons) b.SetActive(false);
        gameObject.SetActive(false);
    }

    void Update()
    {
        CanvasScaler scaler = GetComponentInParent<CanvasScaler>();
        Vector2 anchoredpos = new Vector2(Input.mousePosition.x * scaler.referenceResolution.x / Screen.width, Input.mousePosition.y * scaler.referenceResolution.y / Screen.height);
        if (anchoredpos.x + rect.rect.width * rect.localScale.x > canvasRect.rect.width)
            anchoredpos.x = canvasRect.rect.width - rect.rect.width * rect.localScale.x;
        if (anchoredpos.y + rect.rect.height * rect.localScale.y > canvasRect.rect.height - 70)
            anchoredpos.y = canvasRect.rect.height - rect.rect.height * rect.localScale.y - 70;
        rect.anchoredPosition = anchoredpos;
        turngauge.text = $"턴 게이지: {Math.Round(chara.turngauge, 0)}%" + $"<color=green>(+{Math.Round(20f * Manager.Battle.turnspeed * 1.5f * (float)Math.Round(Math.Sqrt(chara._buffedstats.speed), 2), 2)}/s)</color>";
    }

    public void show(List<Basebuff> buffs)
    {
        float recty = 180 + buffs.Count * 130;
        if (buffs.Count == 0) infotext.SetActive(false);
        else {
            recty += 40;
            infotext.SetActive(true);
        }

        for (int i = 0; i<buffs.Count; i++)
        {
            bufficons[i].SetActive(true);
            bufficons[i].transform.Find("BuffImage").gameObject.GetComponent<Image>().sprite = buffs[i].sprite;
            bufficons[i].transform.Find("BuffDesText").gameObject.GetComponent<TextMeshProUGUI>().text = buffs[i].description;
            bufficons[i].transform.Find("effectname").gameObject.GetComponent<TextMeshProUGUI>().text = buffs[i].buffname;
            if (buffs[i].stackable && buffs[i].bufftype == Define.buffType.Permanant)
            {
                bufficons[i].transform.Find("leftturn").gameObject.GetComponent<TextMeshProUGUI>().text = $"중첩 수: {buffs[i].stack}";
            }
            else if (buffs[i].bufftype == Define.buffType.Shield)
            {
                bufficons[i].transform.Find("leftturn").gameObject.GetComponent<TextMeshProUGUI>().text = $"남은 양: {chara.shield}";
            }
            else
            {
                if (buffs[i].bufftype == Define.buffType.Charge && buffs[i].leftdur <= 0)
                    bufficons[i].transform.Find("leftturn").gameObject.GetComponent<TextMeshProUGUI>().text = $"준비 완료!";
                else
                    bufficons[i].transform.Find("leftturn").gameObject.GetComponent<TextMeshProUGUI>().text = $"남은 턴: {buffs[i].leftdur}";
            }
        }
        charinfo.text = $"Lv.{chara._stats.level} {chara._stats.name}";
        attackpower.text = $"공격력: {chara._stats.attack_power}" + $"{(chara._buffedstats.attack_power == chara._stats.attack_power ? "" :(chara._buffedstats.attack_power > chara._stats.attack_power ? $"<color=green>(+{chara._buffedstats.attack_power - chara._stats.attack_power})</color>": $"<color=red>(-{chara._stats.attack_power - chara._buffedstats.attack_power})</color>"))}";
        armor.text = $"방어력: {chara._stats.armor}" + $"{(chara._buffedstats.armor == chara._stats.armor ? "" : (chara._buffedstats.armor > chara._stats.armor ? $"<color=green>(+{chara._buffedstats.armor - chara._stats.armor})</color>" : $"<color=red>(-{chara._stats.armor - chara._buffedstats.armor})</color>"))}";
        speed.text = $"속도: {chara._stats.speed}" + $"{(chara._buffedstats.speed == chara._stats.speed ? "" : (chara._buffedstats.speed > chara._stats.speed ? $"<color=green>(+{chara._buffedstats.speed - chara._stats.speed})</color>" : $"<color=red>(-{chara._stats.speed - chara._buffedstats.speed})</color>"))}";
        if (chara is Player)
        {
            charinfo.text = $"Lv.{chara._stats.level} {chara._stats.name}";
            exp.text = $"경험치: {chara._stats.exp} / {(chara._stats.level >= 10 ? "MAX" : $"{Manager.Data.LevelDict[chara._stats.level].exp}")}";
        } else if (chara is Monster)
        {
            charinfo.text = $"{chara._stats.name}";
            exp.text = "";
            if (chara.bufflist.Count == 0) recty -= 40;
        }

        rect.sizeDelta = new Vector2(320, recty);
        buffrect.sizeDelta = new Vector2(0, recty - 180);
        gameObject.SetActive(true);
    }
    public void hide()
    {   
        gameObject.SetActive(false);
        foreach (GameObject b in bufficons) b.SetActive(false);
    }
}
