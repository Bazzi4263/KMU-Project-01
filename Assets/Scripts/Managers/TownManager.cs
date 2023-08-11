using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class TownManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private GameObject upgradeBoard;
    [SerializeField] private TextMeshProUGUI[] statTexts;
    [SerializeField] private Image playerImage;
    [SerializeField] private Button[] playersButtons;
    [SerializeField] private int upgradeGold;

    [SerializeField] private GameObject storeBoard;
    [SerializeField] private GameObject storeItems;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemStatText;
    [SerializeField] private TextMeshProUGUI purchaseGoldText;
    List<Button> itemButtons = new List<Button>();

    [SerializeField] private GameObject artefactBoard;
    [SerializeField] private GameObject artefactStatBoard;
    [SerializeField] private GameObject artefactItems;
    [SerializeField] private Image artefactImage;
    [SerializeField] private TextMeshProUGUI artefactNameText;
    [SerializeField] private TextMeshProUGUI artefactDescriptionText;
    [SerializeField] private TextMeshProUGUI artefactGoldText;
    Dictionary<string, Button> artefactButtons = new Dictionary<string, Button>();
    [SerializeField] private int artefactCount;
    [SerializeField] private int reloadGold;

    private int playerNum;
    private Item selectedItem;
    private Artefact selectedArtefact;
    private bool isRandomedMercenary = false;

    private void Start()
    {
        goldText.text = Manager.Item.gold.ToString();
    }

    #region UpgradeWeapn
    public void ShowPlayers()
    {
        Clear();
        upgradeBoard.SetActive(true);

        for (int i = 0; i < Manager.Data.playerstats.Count; i++)
        {
            playersButtons[i].gameObject.SetActive(true);
            playersButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = Manager.Data.playerstats[i].name;
        }
    }

    public void SetPlayer(int num)
    {
        playerNum = num;
    }

    public void ShowStatAndUpgradeButton()
    {
        playerImage.sprite = Manager.Data.playerstats[playerNum].sprites[1];
        //Manager.Data.playerstats[playerNum].sprites에 sprite List를 넣어두었습니다, sprites[1]이 Idle Sprite구요 연결 부탁드려요 
        statTexts[0].text = Manager.Data.playerstats[playerNum].attack_power.ToString();
        statTexts[1].text = Manager.Data.playerstats[playerNum].UpgradeCount.ToString();
        statTexts[2].text = (upgradeGold + Manager.Data.playerstats[playerNum].UpgradeCount * 5).ToString();
    }

    public void UpgradeWeapon()
    {
        if (Manager.Item.gold >= upgradeGold + Manager.Data.playerstats[playerNum].UpgradeCount * 5)
        {
            Manager.Item.gold -= upgradeGold + Manager.Data.playerstats[playerNum].UpgradeCount * 5;
            Manager.Data.playerstats[playerNum].UpgradeCount += 1;
            goldText.text = Manager.Item.gold.ToString();
            ShowStatAndUpgradeButton();
        }
    }
    #endregion

    #region Store
    public void EnableStore()
    {
        Clear();

        storeBoard.SetActive(true);
        storeItems.SetActive(true);

        GameObject itemButton = storeItems.transform.GetChild(0).gameObject;
        itemButton.SetActive(true);

        foreach (Item item in Manager.Item.items.Values)
        {
            GameObject button = Instantiate(itemButton, Vector3.zero, Quaternion.identity);
            button.transform.parent = storeItems.transform;
            button.GetComponent<Image>().sprite = Manager.Item.GetSprite(item.spritenum);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            button.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
            button.GetComponent<Button>().onClick.AddListener( () => SelectItem(item));

            itemButtons.Add(button.GetComponent<Button>());
        }

        itemButton.SetActive(false);
    }

    private void SelectItem(Item item)
    {
        itemImage.sprite = Manager.Item.GetSprite(item.spritenum);
        itemStatText.text = $"Name : {item.name}\n\nDescription :\n{item.description}";

        purchaseGoldText.gameObject.SetActive(true);
        purchaseGoldText.text = item.gold.ToString();
        selectedItem = item;
    }
    
    public void PurchaseItem()
    {
        if (Manager.Item.gold >= selectedItem.gold)
        {
            selectedItem.pp++;
            Manager.Item.gold -= selectedItem.gold;
            goldText.text = Manager.Item.gold.ToString();
        }
    }
    #endregion

    #region Artefact

    public void EnableMercenary()
    {
        Clear();

        artefactBoard.SetActive(true);
        artefactItems.SetActive(true);

        GameObject mercenaryButton = artefactItems.transform.GetChild(0).gameObject;
        mercenaryButton.SetActive(true);

        if (isRandomedMercenary)
        {
            mercenaryButton.SetActive(false);
            return;
        }

        List<Artefact> artefacts = Manager.Artefact.GetRandomArtefacts(artefactCount);

        for (int i = 0; i < artefacts.Count; i++)
        {
            GameObject button = Instantiate(mercenaryButton, Vector3.zero, Quaternion.identity);
            button.transform.parent = artefactItems.transform;
            button.GetComponent<Image>().sprite = Manager.Artefact.Sprites[artefacts[i].spritenum];
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            button.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
            Artefact artefact = artefacts[i];
            button.GetComponent<Button>().onClick.AddListener(() => SelectMercenary(artefact));

            artefactButtons.Add(artefacts[i].name, button.GetComponent<Button>());
        }

        isRandomedMercenary = true;

        mercenaryButton.SetActive(false);
    }

    public void ReloadArtefact()
    {
        if (reloadGold <= Manager.Item.gold)
        {
            Manager.Item.gold -= reloadGold;
            goldText.text = Manager.Item.gold.ToString();

            isRandomedMercenary = false;
            foreach (var item in artefactButtons.Values)
            {
                Destroy(item.gameObject);
            }
            artefactButtons.Clear();
            artefactStatBoard.SetActive(false);
            EnableMercenary();
        }
    }

    private void SelectMercenary(Artefact artefact)
    {
        artefactImage.sprite = Manager.Artefact.Sprites[artefact.spritenum];
        artefactNameText.text = artefact.name_KR;
        switch (artefact.grade)
        {
            case Define.grade.Normal:
                artefactNameText.color = Color.grey;
                break;
            case Define.grade.Rare:
                artefactNameText.color = Color.blue;
                break;
            case Define.grade.Epic:
                artefactNameText.color = Color.red + Color.blue;
                break;
            case Define.grade.Unique:
                artefactNameText.color = Color.yellow;
                break;
            case Define.grade.Legendary:
                artefactNameText.color = Color.green;
                break;
        }
        artefactDescriptionText.text = artefact.description;

        artefactGoldText.gameObject.SetActive(true);
        artefactGoldText.text = artefact.gold.ToString();
        selectedArtefact = artefact;
    }

    public void PurchaseMercenary()
    {
        if (Manager.Item.gold >= selectedArtefact.gold)
        {
            Manager.Item.gold -= selectedArtefact.gold;
            Manager.Artefact.GetArtefact(selectedArtefact.name);
            goldText.text = Manager.Item.gold.ToString();
            Destroy(artefactButtons[selectedArtefact.name].gameObject);
            artefactButtons.Remove(selectedArtefact.name);
        }
    }
    #endregion

    private void Clear()
    {
        upgradeBoard.SetActive(false);
        storeBoard.SetActive(false);
        artefactBoard.SetActive(false);

        storeItems.SetActive(false);
        for (int i = 0; i < itemButtons.Count; i++)
        {
            Destroy(itemButtons[i].gameObject);
        }
        itemButtons.Clear();

        //mercenaryItems.SetActive(false);
        //for (int i = 0; i < mercenaryButtons.Count; i++)
        //{
        //    Destroy(mercenaryButtons[i].gameObject);
        //}
        //mercenaryButtons.Clear();
    }

    public void Exit()
    {
        Manager.Scene.LoadScene(Define.Scenes.MapScene);
    }
}
