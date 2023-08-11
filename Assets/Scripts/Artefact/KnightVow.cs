using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;

public static class KnightVow
{
    static List<Stat> players = new List<Stat>();

    // 가장 체력이 낮은 아군을 지정
    public static List<Stat> FindTarget()
    {
        if (players.Count == 0 || players == null)
        {
            players = new List<Stat>(new Stat[] { Manager.Data.playerstats.OrderBy(x => x.maxhp).ToList()[0] });
            return players;
        }
        else
        {
            List<Stat> temp = players.ToList();
            players.Clear();
            return temp;
        }
    }
}
