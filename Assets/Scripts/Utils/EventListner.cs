using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class EventListner : MonoBehaviour
{
    LightEventListener lightEventListener;
    [SerializeField] SpriteRenderer[] SpriteRenderers;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material fowMeterial;

    private void Awake()
    {
        lightEventListener = GetComponent<LightEventListener>();
    }

    public void UpdateListner()
    {
        if (lightEventListener.visability >= 0.66f)
        {
            foreach (SpriteRenderer sprite in SpriteRenderers)
            {
                if (transform.parent.tag == "BattleEnemy")
                {
                    if (GetComponentInParent<MonsterController>().isChasing)
                    {
                        sprite.material = fowMeterial;
                    }
                    else
                    {
                        sprite.material = defaultMaterial;
                    }
                }
                else
                {
                    sprite.material = defaultMaterial;
                }
            }
        }
        else
        {
            foreach (SpriteRenderer sprite in SpriteRenderers)
            {
                if (transform.parent.tag == "BattleEnemy")
                {
                    if (GetComponentInParent<MonsterController>().isChasing)
                    {
                        sprite.material = fowMeterial;
                    }
                }
            }
        }
    }

    public void ClearMateial()
    {
        foreach (SpriteRenderer sprite in SpriteRenderers)
        {
            sprite.material = fowMeterial;
        }
    }
}
