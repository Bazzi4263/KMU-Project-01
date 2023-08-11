using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager
{
    public Stat nowstat = new Stat();
    public bool choosestat = false;

    public bool LevelUpdate()
    {
        int currentlevel = nowstat.level;
        Level level = Manager.Data.LevelDict[currentlevel];
        if (level != null)
            if (Manager.Data.LevelDict[currentlevel].exp <= nowstat.exp)
            {
                foreach (Level l in Manager.Data.LevelDict.Values)
                {
                    if (l.exp > nowstat.exp)
                    {
                        nowstat.level = l.level;
                        return true;
                    }
                }
                nowstat.level = Manager.Data.LevelDict.Count + 1;
                return true;
            }
        return false;
    }
}
