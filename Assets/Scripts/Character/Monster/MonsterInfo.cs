using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;
using Enum = System.Enum;
using System;
using Random = UnityEngine.Random;
using static Define;

[Serializable]
public class MonsterInfo : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] monsterSprites;

    public List<Define.MonsterType> monsterTypes = new List<Define.MonsterType>();

    private void Start()
    {
        if(!MapManager.Instance.isLoadData)
        {
            int monsterCount;

            if (MapManager.Instance.MonsterList.IndexOf(this) < MapManager.Instance.EasyMonsterCount)
            {
                monsterCount = Random.Range(1, 3);
            }
            else
            {
                monsterCount = Random.Range(3, 5);
            }

            for (int i = 0; i < monsterCount; i++)
            {
                switch (MapManager.Instance.currentStage)
                {
                    case Define.STAGE.GRASSLAND:
                        Define.MonsterType.GrassLand monsterType = (Define.MonsterType.GrassLand)(Random.Range(1, Enum.GetNames(typeof(Define.MonsterType.GrassLand)).Length-1));
                        monsterSprites[i].sprite = Manager.Resource.Load<Sprite>($"Sprites/{monsterType.ToString()}(Grassland)");
                        monsterTypes.Add(new Define.MonsterType(monsterType, 0, 0));
                        break;
                    case Define.STAGE.DESERT:
                        Define.MonsterType.Desert monsterType2 = (Define.MonsterType.Desert)(Random.Range(1, Enum.GetNames(typeof(Define.MonsterType.Desert)).Length - 1));
                        monsterSprites[i].sprite = Manager.Resource.Load<Sprite>($"Sprites/{monsterType2.ToString()}(Desert)");
                        if (monsterType2 == MonsterType.Desert.BlackLamia || monsterType2 == MonsterType.Desert.Lamia)
                        { 
                            monsterSprites[i].flipX = true;
                            monsterSprites[i].transform.localScale = new Vector2(0.1f, 0.1f);
                        }
                        monsterTypes.Add(new Define.MonsterType(0, monsterType2, 0));
                        break;
                    case Define.STAGE.SNOWLAND:
                        Define.MonsterType.SnowLand monsterType3 = (Define.MonsterType.SnowLand)(Random.Range(1, Enum.GetNames(typeof(Define.MonsterType.Desert)).Length - 1));
                        monsterSprites[i].sprite = Manager.Resource.Load<Sprite>($"Sprites/{monsterType3.ToString()}(SnowLand)");
                        if (monsterType3 == MonsterType.SnowLand.SkeletonWarrior || monsterType3 == MonsterType.SnowLand.Skeleton) monsterSprites[i].flipX = true;
                        if (monsterType3 == MonsterType.SnowLand.Succubus) monsterSprites[i].transform.localScale = new Vector2(0.07f, 0.07f);
                        monsterTypes.Add(new Define.MonsterType(0,0,monsterType3));
                        break;
                }
            }
        }
    }

    public void RestoreMonster()
    {
        switch (MapManager.Instance.currentStage)
        {
            case Define.STAGE.GRASSLAND:
                for (int i = 0; i < monsterTypes.Count; i++)
                    monsterSprites[i].sprite = Manager.Resource.Load<Sprite>($"Sprites/{monsterTypes[i].grassLand.ToString()}(Grassland)");
                break;
            case Define.STAGE.DESERT:
                break;
            case Define.STAGE.SNOWLAND:
                break;
        }
    }
}
