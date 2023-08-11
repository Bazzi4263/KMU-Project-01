using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PlayerSelect : MonoBehaviour
{
    [SerializeField] private Image playerImage;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject[] HPImages;
    [SerializeField] private GameObject[] AtImages;
    [SerializeField] private GameObject[] DefImages;
    [SerializeField] private GameObject[] SpeedImages;

    Define.PlayerType currentPlayer = Define.PlayerType.None + 1;

    public void NextBtn()
    {
        if (currentPlayer + 1 == Define.PlayerType.End)
            currentPlayer = Define.PlayerType.None + 1;
        else
            currentPlayer += 1;

        UpdatePlayer();
    }

    public void PrevBtn()
    {
        if (currentPlayer - 1 == Define.PlayerType.None)
            currentPlayer = Define.PlayerType.End - 1;
        else
            currentPlayer -= 1;

        UpdatePlayer();
    }

    public void UpdatePlayer()
    {
        playerImage.sprite = Manager.Data.PlayerStatDict[currentPlayer.ToString()].sprites[1];

        nameText.text = currentPlayer.ToString();

        for (int i = 0; i < HPImages.Length; i++)
        {
            bool active = true ? Manager.Data.PlayerStatDict[currentPlayer.ToString()].maxhp > i : false;
            HPImages[i].SetActive(active);
        }

        for (int i = 0; i < AtImages.Length; i++)
        {
            bool active = true ? Manager.Data.PlayerStatDict[currentPlayer.ToString()].attack_power > i : false;
            AtImages[i].SetActive(active);
        }

        for (int i = 0; i < DefImages.Length; i++)
        {
            bool active = true ? Manager.Data.PlayerStatDict[currentPlayer.ToString()].armor > i : false;
            DefImages[i].SetActive(active);
        }

        for (int i = 0; i < SpeedImages.Length; i++)
        {
            bool active = true ? Manager.Data.PlayerStatDict[currentPlayer.ToString()].speed > i : false;
            SpeedImages[i].SetActive(active);
        }
    }

    public void SelectPlayer()
    {
        Manager.Data.AddPlayer(currentPlayer.ToString());
        Manager.Scene.LoadScene(Define.Scenes.MapScene);
    }

    public void LoadGame()
    {
       // Manager.Data.Load();
    }
}
