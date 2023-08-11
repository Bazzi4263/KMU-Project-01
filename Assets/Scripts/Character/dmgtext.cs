using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class dmgtext : MonoBehaviour
{
    [SerializeField] float destroyTime;

    public TextMeshProUGUI text;
    public string dmg;
    public Color color;
    

    private void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1.5f);
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(init());
    }

    public IEnumerator init()
    {
        destroyTime = 2f;
        text.text = dmg;
        text.color = color;
        transform.DOMove(transform.position + new Vector3(0, 2), destroyTime).SetEase(Ease.OutQuart);
        text.DOFade(0, destroyTime);
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}