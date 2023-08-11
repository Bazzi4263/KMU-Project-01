using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Reward : UI_PopUp
{
    int _level;
    WaitUntil wu;

    enum GameObjects
    {
        RewardPanel
    }

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Random random = new Random();
        int portionrand = random.Next(0, 100);
        Bind<GameObject>(typeof(GameObjects));
        GameObject reward = Get<GameObject>((int)GameObjects.RewardPanel);
        Image gold = Util.FindChild<Image>(reward, "GoldIcon");
        Image portion = Util.FindChild<Image>(reward, "PortionIcon");
        Image exp = Util.FindChild<Image>(reward, "ExpIcon");
        _cancel = Util.FindChild<Button>(reward, "CancelButton");

        AddUIEvent(_cancel.gameObject, (PointerEventData) =>
        {
            if (Manager.Battle.battletype == Define.battleType.town)
            {
                if (MapManager.Instance.currentStage == Define.STAGE.SNOWLAND)
                    Manager.Battle.EndGame();
                else
                    Manager.Scene.LoadScene(Define.Scenes.TownScene); 
            }
            else Manager.Scene.LoadScene(Define.Scenes.MapScene);
        }, Define.UIEvent.Click);

        if (portionrand < 66)
            portion.gameObject.SetActive(false);
        else
            Util.GetOrAddComponent(portion.gameObject, Manager.Item.RandomItemGain());

        Util.GetOrAddComponent(gold.gameObject, Type.GetType("RewardGold"));
        Util.GetOrAddComponent(exp.gameObject, Type.GetType("RewardExp"));
        AddUIEvent(exp.gameObject, (PointerEventData) => {
            CoroutineHelper.StartCoroutine(LevelCheck());
        }, Define.UIEvent.Click);
        wu = new WaitUntil(() => Manager.Reward.choosestat);
    }

    IEnumerator LevelCheck()
    {
        _cancel.gameObject.SetActive(false);
        int i;
        foreach (Stat stat in Manager.Data.playerstats)
        {
            if (!stat.isdead)
            {
                Manager.Reward.nowstat = stat;
                stat.exp += Manager.Battle.exp;
                Manager.UI.SetDialog($"{stat.name}가 {Manager.Battle.exp}의 경험치 획득!");
                yield return Manager.Data.wfs10;

                Manager.UI.SetDialog($"{stat.name}의 경험치는 총 {stat.exp}이 되었다!");
                yield return Manager.Data.wfs10;

                _level = stat.level;
                if (Manager.Reward.LevelUpdate())
                {
                    Manager.UI.SetDialog($"{stat.name}의 레벨이 {stat.level - _level}만큼 올라 {stat.level}이 되었다!");
                    yield return Manager.Data.wfs10;
                    foreach (Skill skill in stat.skills.Values)
                        if (skill.required_level > _level && skill.required_level <= stat.level)
                        {
                            Manager.UI.SetDialog($"{stat.name}은 이제 {skill.name}을 사용할 수 있게 되었다!");
                            yield return Manager.Data.wfs10;
                        }


                    for (i = _level + 1; i <= stat.level; i++)
                    {
                        if (i % 2 == 0)
                        {
                            Manager.UI.SetDialog($"{i}레벨의 보상을 선택해주세요.");
                            yield return Manager.Data.wfs10;
                            Manager.UI.ShowPopUpUI<StatUp>();
                            yield return wu;
                            yield return Manager.Data.wfs10;
                            Manager.Reward.choosestat = false;
                        }
                        
                    }
                }
                yield return Manager.Data.wfs10;
            }
        }
        Manager.Battle.exp = 0;
        _cancel.gameObject.SetActive(true);
    }
}
