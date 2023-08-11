using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapSS : MonoBehaviour
{
    public RawImage img;

    void Start()
    {
        if (BaseScene.Scene == Define.Scenes.MapScene)
        {
            img.texture = Manager.Resource.Load<Texture>("Art/Textures/Minimap Render Texture");
        }

        if (BaseScene.Scene == Define.Scenes.BattleScene)
        {
            img.texture = MapManager.Instance.savedMapTexture;
        }
    }
}
