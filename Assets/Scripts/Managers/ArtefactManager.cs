using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using static Define;

public class ArtefactManager
{
    public Dictionary<string, Artefact> _artefacts = new Dictionary<string, Artefact>(); // 모든 유물들을 담고 있음
    public List<Artefact> currentArtefacts = new List<Artefact>(); // 현재 소지중인 유물
    List<Sprite> _sprites = new List<Sprite>();

    public List<Sprite> Sprites { get => _sprites; set => _sprites = value; }

    // 유물 획득
    public void GetArtefact(string name) 
    {
        currentArtefacts.Add(_artefacts[name]);
        TakeEffectArtefact(_artefacts[name]);
        Debug.Log($"{name}유물 적용");
        Manager.UI.AddUIArtefact();
    }

    public List<Artefact> GetRandomArtefacts(int num)
    {
        List<Artefact> temp = new List<Artefact>();

        while (num > 0)
        {
            if (_artefacts.Count - temp.Count - currentArtefacts.Count == 0)
            {
                return temp;
            }


            int rand = UnityEngine.Random.Range(0, 100); // 3% 레전더리, 5% 유니크, 10% 에픽, 15% 레어, 67% 노말
            grade grade;
            if (rand <= 66)
                grade = Define.grade.Normal;
            else if (rand <= 81)
                grade = Define.grade.Rare;
            else if (rand <= 91)
                grade = Define.grade.Epic;
            else if (rand <= 96)
                grade = Define.grade.Unique;
            else
                grade = Define.grade.Legendary;

            List<Artefact> artefacts = Manager.Artefact._artefacts.Values.ToList().FindAll(x => x.grade == grade);

            for (int i = artefacts.Count - 1; i >= 0; i--)
            {
                foreach (var ownArtefact in Manager.Artefact.currentArtefacts)
                {
                    if (artefacts[i].name == ownArtefact.name)
                    {
                        artefacts.RemoveAt(i);
                        goto NextFor;
                    }
                }

                foreach (Artefact tempArtefact in temp)
                {
                    if (artefacts[i].name == tempArtefact.name)
                    {
                        artefacts.RemoveAt(i);
                        goto NextFor;
                    }
                }

NextFor:;
            }

            if (artefacts.Count != 0)
            {
                temp.Add(artefacts[UnityEngine.Random.Range(0, artefacts.Count - 1)]);
                num--;
            }
        }

        return temp;
    }

    // 유물 효과 적용 isReset이 true 일시 반대로 유물 효과 제거
    public void TakeEffectArtefact(Artefact artefact, bool isReset = false)
    {
        if (artefact.artefacttype == artefactType.Pemanant)
        {
            List<Stat> players = new List<Stat>();

            switch (artefact.artefacttarget)
            {
                case Define.artefactTarget.AllPlayer:
                    players = Manager.Data.playerstats;
                    break;
                case Define.artefactTarget.AllEnemy:
                    break;
                case Define.artefactTarget.ParticularPlayer:
                    var method = Type.GetType(artefact.classname).GetMethod("FindTarget");
                    players = method.Invoke(this, null) as List<Stat>;
                    break;
                case Define.artefactTarget.ParticularEnemy:
                    break;
            }

            int amount = isReset ? -(int)artefact.rates[0] : (int)artefact.rates[0];
            switch (artefact.affectedstat)
            {
                case Define.affectedStat.hp:
                    foreach (Stat player in players)
                    {
                        player.maxhp += amount;
                        player.currenthp += amount;
                    }
                    break;
                case Define.affectedStat.speed:
                    foreach (Stat player in players)
                    {
                        player.speed += amount;
                    }
                    break;
                case Define.affectedStat.armor:
                    foreach (Stat player in players)
                    {
                        player.armor += amount;
                    }
                    break;
                case Define.affectedStat.attack_power:
                    foreach (Stat player in players)
                    {
                        player.attack_power += amount;
                    }
                    break;
                case Define.affectedStat.pp:
                    break;
                case Define.affectedStat.all_stat:
                    foreach (Stat player in players)
                    {
                        player.maxhp += amount;
                        player.currenthp += amount;
                        player.speed += amount;
                        player.armor += amount;
                        player.attack_power += amount;
                    }
                    break;
                case Define.affectedStat.other:
                    var method = Type.GetType(artefact.classname).GetMethod("Effect");
                    method.Invoke(this, new object[] { players, artefact.rates, isReset });
                    break;
            }
        }
    }

    public void ClearArtefact()
    {
        currentArtefacts.Clear();
    }
}
