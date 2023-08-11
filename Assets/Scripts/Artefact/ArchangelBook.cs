using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ArchangelBook
{
    static List<Stat> players = new List<Stat>();
    static int level;

    // 프리스트 지정
    public static List<Stat> FindTarget()
    {
        if (players.Count == 0 || players == null)
        {
            foreach (Stat player in Manager.Data.playerstats)
                if (player.name == "Priest")
                    players.Add(player);

            return players;
        }
        else
        {
            List<Stat> temp = players.ToList();
            players.Clear();

            return temp;
        }
    }
                               //   AP배수, pp증가량, 집중시간               
    public static void Effect(List<Stat> players, float[] rates, bool isReset)
    {
        foreach (Stat priest in players)
        { //ap*2 pp++ duration = 0
            priest.attack_power = isReset ? priest.attack_power / rates[0] : priest.attack_power * rates[0];
            priest.skills["Genesis"].pp = isReset ? priest.skills["Genesis"].pp - (int)rates[1] : priest.skills["Genesis"].pp + (int)rates[1];
            priest.skills["Genesis"].maxpp = isReset ? priest.skills["Genesis"].maxpp - (int)rates[1] : priest.skills["Genesis"].maxpp + (int)rates[1];
            priest.skills["Genesis"].duration[0] = isReset ? priest.skills["Genesis"].duration[0] + (int)rates[2] : priest.skills["Genesis"].duration[0] - (int)rates[2];

            if (!isReset)
            {
                level = priest.skills["Genesis"].required_level;
                priest.skills["Genesis"].required_level -= level;
            }
            else
            {
                priest.skills["Genesis"].required_level += level;
            }
        }
    }
}
