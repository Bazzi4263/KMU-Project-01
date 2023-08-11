using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnknownEvent_GetArtefact : UnknownEvent
{
    public UnknownEvent_GetArtefact()
    {
        description = "유물 Random 획득!";
    }

    public override void Effect()
    {
        Artefact artefact = Manager.Artefact.GetRandomArtefacts(1)[0];
        Manager.UI.SetDialog($"유물 {artefact.name_KR} 획득!");
        Manager.Artefact.GetArtefact(artefact.name);
    }
}
