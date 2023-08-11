using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
/*
* 플레이어가 가진 아이템(포션)과 개수를 사전으로 들고 있는다
* 스킬도 포함
* maxpp 10 pp 15 hp 25 maxhp 20 poison 30
*/
public class ItemManager
{
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    List<Sprite> _sprites = new List<Sprite>();
    Random _rand;
    public int gold = 100;

    public Sprite GetSprite(int i) { return _sprites[i]; }

    public void SetSprite(Sprite sprite) { _sprites.Add(sprite); }

    public Type RandomItemGain()
    {
        _rand = new Random();
        int random = _rand.Next(100);
        if (random < 10)
            return Type.GetType("RewardPPMaxPortion");
        else if (random < 25)
            return Type.GetType("RewardPPPortion");
        else if (random < 45)
            return Type.GetType("RewardHPMaxPortion");
        else if (random < 70)
            return Type.GetType("RewardHPPortion");
        else
            return Type.GetType("RewardPoisonPortion");
    }

    public List<string> CountExistItem()
    {
        List<string> item = new List<string>();
        foreach (string key in items.Keys)
            if (items[key].pp > 0)
                item.Add(key);

        return item;
    }
}
