using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnknownEventManager
{
    private UnknownEvent[] events = {
    new UnknownEvent_DamageHP(),
    new UnknownEvent_DamagePP(),
    new UnknownEvent_GetArtefact(),
    new UnknownEvent_RecoverHP(),
    new UnknownEvent_RecoverPP(),
    new UnknownEvent_UpgradeCharacter()
    };

    private Queue<UnknownEvent> currentEvents;

    public UnknownEventManager()
    {
        ClearEvent();
    }

    public void ClearEvent()
    {
        currentEvents = new Queue<UnknownEvent>(events);
        currentEvents = new Queue<UnknownEvent>(currentEvents.OrderBy(g => Guid.NewGuid()).ToList());
    }

    public void Event()
    {
        Manager.UI.ShowPopUpUI<UI_MapDialog>();
        currentEvents.Dequeue().Effect();       
    }
}
