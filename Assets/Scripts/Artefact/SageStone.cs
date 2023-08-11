using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SageStone
{
    static List<Stat> players = new List<Stat>();

    // 프리스트 지정
    public static List<Stat> FindTarget()
    {
        if (players.Count == 0 || players == null)
        {
            foreach (Stat player in Manager.Data.playerstats)
                if (player.name == "Magician")
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
    //   기운 증가량 배수            
    public static void Effect(List<Stat> players, float[] rates, bool isReset)
    {
        foreach (Stat magician in players)
        {
            magician.skills["Meditation"].rates[0] = isReset ? magician.skills["Meditation"].rates[0] / rates[0] : magician.skills["Meditation"].rates[0] * rates[0];
        }
    }
}
