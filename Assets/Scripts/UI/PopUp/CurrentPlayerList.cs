using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurrentPlayerList : UI_PopUp
{
    List<GameObject> local = new List<GameObject>();
    int i, l;
    GameObject temp;
    Vector3 staticonloc;

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
        GameObject Contents = Util.FindChild(Util.FindChild(scroll, "Viewport"), "Content");

        //자동화 코드
        foreach (Stat stat in Manager.Data.playerstats)
        {
            GameObject staticon = Manager.Resource.Instantiate("UI/PopUp/PlayerStatus");
            local.Add(staticon);

            AddUIEvent(staticon, (PointerEventData data) =>
            {
                for (i = 0; i < local.Count; i++)
                    if (local[i] == staticon)
                        l = i;
                staticonloc = local[l].transform.position;
            }
        , Define.UIEvent.BeginDrag);

            AddUIEvent(staticon, (PointerEventData data) =>
            {
                local[l].transform.position = staticonloc;
                for (i = 0; i < local.Count; i++)
                    if (local[i].transform.position.x >= data.position.x)
                        break;

                Swap(i);
            }
        , Define.UIEvent.EndDrag);
            staticon.transform.SetParent(Contents.transform, false);
            Util.GetOrAddComponent<PlayerStatus>(staticon).SetPanel(stat);
        }
    }

    private void Swap(int x)
    {
        GameObject temp = local[l];
        Vector3 tempvec;
        Stat tempstat = Manager.Data.playerstats[l];

        if (x < l)
        {
            tempvec = local[x].transform.position;

            for (int j = l; j > x; j--)
            {
                tempvec = local[j - 1].transform.position;
                local[j - 1].transform.position = local[j].transform.position;
                local[j].transform.position = tempvec;

                temp = local[j];
                local[j] = local[j - 1];
                local[j - 1] = temp;

                Manager.Data.playerstats[j] = Manager.Data.playerstats[j - 1];
            }
            temp.transform.position = tempvec;
            local[x] = temp;
            Manager.Data.playerstats[x] = tempstat;
        }

        else if (x > l)
        {
            x--;
            tempvec = local[x].transform.position;
            for (int j = l; j < x; j++)
            {
                tempvec = local[j + 1].transform.position;
                local[j + 1].transform.position = local[j].transform.position;
                local[j].transform.position = tempvec;

                temp = local[j];
                local[j] = local[j + 1];
                local[j + 1] = temp;

                Manager.Data.playerstats[j] = Manager.Data.playerstats[j + 1];
            }

            Manager.Data.playerstats[x] = tempstat;
        }
    }
}
