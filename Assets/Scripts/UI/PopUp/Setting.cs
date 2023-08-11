using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : UI_PopUp
{
    enum GameObjects
    {
        SettingPanel
    }

    protected override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject Setting = Get<GameObject>((int)GameObjects.SettingPanel);
        GameObject sound = Util.FindChild(Setting, "Sound");
        GameObject total = Util.FindChild(sound, "TotalSoundIcon");
        GameObject bgm = Util.FindChild(sound, "BGMSoundIcon");
        GameObject effect = Util.FindChild(sound, "EffectSoundIcon");

        Slider totalsound = Util.FindChild<Slider>(total, "SoundSlider");
        Slider bgmsound = Util.FindChild<Slider>(bgm, "SoundSlider");
        Slider effectsound = Util.FindChild<Slider>(effect, "SoundSlider");
        totalsound.value = Manager.Sound.totalvolume;
        bgmsound.value = Manager.Sound.bgmvolume;
        effectsound.value = Manager.Sound.effectvolume;
        AddUIEvent(bgmsound.gameObject, (PointerEventData => 
        { 
            Manager.Sound.bgmvolume = bgmsound.value;
            Manager.Sound.SetBgmVolume();
            //Manager.Data.SaveVolume();
        }), Define.UIEvent.Click);

        AddUIEvent(bgmsound.gameObject, (PointerEventData => 
        { 
            Manager.Sound.bgmvolume = bgmsound.value;
            Manager.Sound.SetBgmVolume();
            //Manager.Data.SaveVolume();
        }), Define.UIEvent.Drag);

        AddUIEvent(totalsound.gameObject, (PointerEventData => 
        { 
            Manager.Sound.totalvolume = totalsound.value;
            Manager.Sound.SetBgmVolume();
            //Manager.Data.SaveVolume();
        }), Define.UIEvent.Click);

        AddUIEvent(totalsound.gameObject, (PointerEventData => 
        { 
            Manager.Sound.totalvolume = totalsound.value;
            Manager.Sound.SetBgmVolume();
            //Manager.Data.SaveVolume();
        }), Define.UIEvent.Drag);

        AddUIEvent(effectsound.gameObject, (PointerEventData => { Manager.Sound.effectvolume = effectsound.value; }), Define.UIEvent.Click);
        AddUIEvent(effectsound.gameObject, (PointerEventData => { Manager.Sound.effectvolume = effectsound.value; }), Define.UIEvent.Drag);
        GameObject graphic = Util.FindChild<GameObject>(Setting, "Graphic");
    }

    private void Start()
    {
        Init();
    }
}
