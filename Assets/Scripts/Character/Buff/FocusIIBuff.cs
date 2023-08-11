using UnityEngine;

public class FocusIIBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        for (int i = affectedchara.bufflist.Count - 1; i >= 0; i--)
        {
            if (affectedchara.bufflist[i].bufftype == Define.buffType.Debuff)
            {
                Basebuff temp = affectedchara.bufflist[i];
                affectedchara.gui.DelBuffGUI(affectedchara.bufflist[i]);
                affectedchara.bufflist.RemoveAt(i);
                Destroy(temp);
            }
        }
        affectedchara._status[Define.characterStatus.DebuffImmune] = true;
        //UI_Panel.Dialog($"{chara._stats.name}(이)가 자신에게 걸린 모든 디버프를 제거했다!");
    }

    protected override void OnDestroy()
    {
        affectedchara._status[Define.characterStatus.DebuffImmune] = false;
        base.OnDestroy();
    }
}
