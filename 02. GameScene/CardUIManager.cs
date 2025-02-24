using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardUIManager : Singleton<CardUIManager>
{
    [Header("Data")]
    [SerializeField] private int selectableCardCount; //Ÿ�� ���� �� ���� ����    
    private bool isSelectTime = false;

    private SheetData sheetData;
    private SheetTextData sheetTextData;

    //private List<GameObject> cardList = new List<GameObject>();
    private Dictionary<int, GameObject> cardList = new Dictionary<int, GameObject>();

    private int curSelectedTileID = -1;

    [Header("UI")]
    [SerializeField] protected GameObject cardUI;
    [SerializeField] private Image cardSelectFrame;
    [SerializeField] private TMP_Text cardSelectTitleText;
    [SerializeField] private GameObject selectCardListRoot;
    [SerializeField] private GameObject blockImage;
    [SerializeField] private Button selectButton;

    // Start is called before the first frame update
    void Start()
    {
        sheetData = TileDataManager.Instance.sheetData;
        sheetTextData = TileDataManager.Instance.sheetTextData;

        Toggle_CardUI(false);
        //cardUI.GetComponentInChildren<Button>().enabled = false;

        cardSelectFrame.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(false);
    }

    public void Toggle_CardUI(bool state)
    {
        cardUI.SetActive(state);
    }

    public void Check_CardUI(GameObject targetTile)
    {
        GameObject curTarget = cardUI.GetComponent<CardUI>().curTarget;

        if (curTarget == targetTile)
        {
            Toggle_CardUI(false);
            cardUI.GetComponent<CardUI>().curTarget = null;
        }
    }

    /// <summary>
    /// CardUI ���� ��, �������� �κ� ó��
    /// </summary>
    public void Refresh_CommonCardUI(CardUI targetUI, int tileID)
    {
        targetUI.cardTitleText.text = sheetData.DataList[tileID].TileName;

        int effectDayType = sheetData.DataList[tileID].EffectDayType;
        targetUI.cardEffectText.text = $"{sheetTextData.DataList[effectDayType].EffectDayTypeText}�� ���·� " +
                                       $"{sheetData.DataList[tileID].EffectTime}�ʰ� ������ ���ⷮ�� ȹ���մϴ�.";

        int typeID = sheetData.DataList[tileID].TypeID;
        int rank = sheetData.DataList[tileID].Rank - 1;
        targetUI.cardRankFrame.sprite = TileDataManager.Instance.tileCardRankFrames[rank];
        targetUI.tilePropertyImage.sprite = TileDataManager.Instance.cardBGSprites[typeID][rank];
        targetUI.cardRewardFrame.sprite = TileDataManager.Instance.cardRewardBGSprites[rank];

        int emptyIndex = -1;

        List<int> additionalRewardTypes = sheetData.DataList[tileID].AdditionalRewardTypes;
        if (additionalRewardTypes[0] == -1)
        {
            for (int i = 0; i < targetUI.affinityList.Length; i++)
            {
                targetUI.affinityList[i].SetActive(false);
            }

            emptyIndex = 0;
        }
        else
        {
            for (int i = 0; i < targetUI.affinityList.Length; i++)
            {
                if (i < additionalRewardTypes.Count)
                {
                    targetUI.affinityList[i].transform.GetChild(1).GetComponent<Image>().sprite = TileDataManager.Instance.tileSprites[additionalRewardTypes[i]][0];
                    targetUI.affinityList[i].transform.GetChild(0).GetComponent<TMP_Text>().text = $"��";
                    targetUI.affinityList[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = $"<color=#41AF39>��</color>";
                    targetUI.affinityList[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -3, 0);
                    targetUI.affinityList[i].SetActive(true);
                }
                else
                {
                    emptyIndex = i;
                    break;
                }
            }
        }

        int reducedRewardType = sheetData.DataList[tileID].ReducedRewardType;
        if (reducedRewardType != -1)
        {
            targetUI.affinityList[emptyIndex].transform.GetChild(1).GetComponent<Image>().sprite = TileDataManager.Instance.tileSprites[reducedRewardType][0];
            targetUI.affinityList[emptyIndex].transform.GetChild(0).GetComponent<TMP_Text>().text = $"��";
            targetUI.affinityList[emptyIndex].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = $"<color=orange>��</color>";
            targetUI.affinityList[emptyIndex].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            targetUI.affinityList[emptyIndex].SetActive(true);

            emptyIndex++;
        }

        if (emptyIndex < targetUI.affinityList.Length)
        {
            for (int i = emptyIndex; i < targetUI.affinityList.Length; i++)
            {
                targetUI.affinityList[i].SetActive(false);
            }
        }
    }

    //InInventory
    public void Refresh_CardUI(int tileID, Sprite tilePropertySprite, Sprite timePropertySprite)
    {
        CardUI targetUI = cardUI.GetComponent<CardUI>();

        Toggle_CardUI(true);
        Refresh_CommonCardUI(targetUI, tileID);

        //���ⷮ
        float originReward = sheetData.DataList[tileID].Reward;
        targetUI.gameObject.transform.position = new Vector2(230, 70);
        targetUI.rewardText.text = $"<color=black>{originReward}</color>";
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetUI.rewardText.rectTransform);

        //������
        targetUI.timePropertyIcon.sprite = timePropertySprite;
        targetUI.tilePropertyIcon.sprite = tilePropertySprite;

        //���� ��ư
        targetUI.selectButton.enabled = false;
    }

    //Equiped    
    public void Refresh_CardUI(DefaultTile targetTile, Vector2 touchPos)
    {
        CardUI targetUI = cardUI.GetComponent<CardUI>();

        Toggle_CardUI(true);

        int tileID = targetTile.tileID;
        Refresh_CommonCardUI(targetUI, tileID);

        targetUI.curTarget = targetTile.gameObject;

        //���ⷮ
        float originReward = sheetData.DataList[tileID].Reward;
        float totalValue = targetTile.GetTotalScore(tileID);

        string totalValueText = $"{totalValue}";

        if (totalValue < originReward)
        {
            totalValueText += $"<color=orange> ( {totalValue - originReward} ��)</color>";
        }
        else if (totalValue > originReward)
        {
            totalValueText += $"<color=#41AF39> ( {totalValue - originReward} ��)</color>";
        }

        targetUI.rewardText.text = totalValueText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetUI.rewardText.rectTransform);

        if (AllManager.instance.isTutorial)
        {
            //Ʃ�丮���� ��� ������ ��ġ�� ī�� ���

            RectTransform rectTrans = targetUI.GetComponent<RectTransform>();
            Vector2 anchor = new Vector2(0.5f, 0.5f);
            rectTrans.anchorMin = anchor;
            rectTrans.anchorMax = anchor;
            rectTrans.pivot = anchor;

            targetUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
        }
        else
        {
            //UI ȭ�� ������ �ȳ����� ó��
            targetUI.transform.position = touchPos;

            ////UI �� ��Ŀ�� ���ϴ��̹Ƿ�, ���� ��ġ�� üũ
            Canvas canvas = UIManager.Instance.mainCanvas;
            Vector2 targetSize = targetUI.gameObject.GetComponent<RectTransform>().sizeDelta;
            Vector2 rightUpPos = new Vector2(touchPos.x + targetSize.x, touchPos.y + targetSize.y);

            Vector2 anchoredPos;
            anchoredPos.x = touchPos.x;
            anchoredPos.y = Mathf.Clamp(rightUpPos.y, 0, canvas.GetComponent<RectTransform>().rect.height);
            anchoredPos.y -= targetSize.y;

            //targetUI.gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

            targetUI.transform.position = anchoredPos;
        }        

        //������
        int typeID = sheetData.DataList[tileID].TypeID;
        int rank = sheetData.DataList[tileID].Rank - 1;
        int effectDayID = sheetData.DataList[tileID].EffectDayType;

        targetUI.tilePropertyIcon.sprite = TileDataManager.Instance.tileSprites[typeID][rank];
        targetUI.timePropertyIcon.sprite = TileDataManager.Instance.timeIconSprites[effectDayID];          

        //���� ��ư
        targetUI.selectButton.enabled = false;
        UIManager.Instance.ToggleCloseButton(true);
    }

    //Selecting
    public delegate void ButtonCallback();
    public void Refresh_CardUI(GameObject targetCard, int tileID, Sprite tilePropertySprite, Sprite timePropertySprite, ButtonCallback callback)
    {
        CardUI targetUI = targetCard.GetComponent<CardUI>();

        targetCard.SetActive(true);
        Refresh_CommonCardUI(targetUI, tileID);

        //���ⷮ
        float originReward = sheetData.DataList[tileID].Reward;
        targetUI.rewardText.text = $"<color=black>{originReward}</color>";
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetUI.rewardText.rectTransform);

        //������
        targetUI.timePropertyIcon.sprite = timePropertySprite;
        targetUI.tilePropertyIcon.sprite = tilePropertySprite;
        
        //���� ��ư
        targetUI.selectButton.enabled = true;
        targetUI.selectButton.onClick.AddListener(() => 
            callback()
        );
    }

    /// <summary>
    /// ���� ī�� ����
    /// </summary>
    private GameObject CreateCardUI(int tileID)
    {
        GameObject newCard = Instantiate(cardUI, selectCardListRoot.transform);

        int typeID = sheetData.DataList[tileID].TypeID;
        int rank = sheetData.DataList[tileID].Rank - 1;
        Sprite tilePropertySprite = TileDataManager.Instance.tileSprites[typeID][rank];

        int effectDayTypeID = sheetData.DataList[tileID].EffectDayType;
        Sprite timePropertySprite = TileDataManager.Instance.timeIconSprites[effectDayTypeID];

        RectTransform rectTrans = newCard.GetComponent<RectTransform>();
        Vector2 anchor = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMin = anchor;
        rectTrans.anchorMax = anchor;
        rectTrans.pivot = anchor;

        Refresh_CardUI(newCard, tileID, tilePropertySprite, timePropertySprite, () =>
        {
            if (curSelectedTileID == tileID)
                return;

            curSelectedTileID = tileID;
            selectButton.interactable = true;

            RefreshCardUIScale();
            //OnEndSelectCard(tileID, typeID, effectDayTypeID); 
        });

        /*
        CardUI targetCard = newCard.GetComponent<CardUI>();
  
        //ī�� �Ӽ�
        int typeID = sheetData.DataList[tileID].TypeID;
        Sprite tilePropertySprite = TileDataManager.Instance.tileCardBGs[typeID];
        newCard.transform.GetChild(0).GetComponent<Image>().sprite = tilePropertySprite;

        //ī�� �̸�
        string tileName = sheetData.DataList[tileID].TileName;
        newCard.transform.GetChild(1).GetComponent<TMP_Text>().text = tileName;

        //���� �Ӽ�
        int effectDayTypeID = sheetData.DataList[tileID].EffectDayType;
        Sprite timePropertySprite = TileDataManager.Instance.timeIconSprites[effectDayTypeID];
        newCard.transform.GetChild(2).GetComponent<Image>().sprite = timePropertySprite;

        //ȿ�� 
        string effectDayTypeText = sheetTextData.DataList[effectDayTypeID].EffectDayTypeText;
        string effectTimeText = sheetData.DataList[tileID].EffectTime.ToString();

        string effectText = $"{effectDayTypeText}�� ���·� {effectTimeText}�ʰ� ������ ���ⷮ�� ȹ���մϴ�.";
        newCard.transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = effectText;

        //�⺻ ���ⷮ
        float originReward = sheetData.DataList[tileID].Reward;
        rewardText.text = $"{originReward}";
        Debug.Log($"originReward {originReward}");

        //���� ��ư
        Button selectButton = newCard.GetComponentInChildren<Button>();
        selectButton.enabled = true;

        selectButton.onClick.AddListener(
            delegate { OnEndSelectCard(tileID, typeID, effectDayTypeID); }
        );

        

        newCard.SetActive(true);
        */
        return newCard;
    }

    /// <summary>
    /// ī�� ���� ��, ������ ī�� �ܿ��� ũ�� �پ��� ����
    /// </summary>
    private void RefreshCardUIScale()
    {
        if (curSelectedTileID == -1)
            return;

        foreach (KeyValuePair<int,GameObject> item in cardList)
        {
            int tileID = item.Key;
            GameObject cardUI = item.Value;
            
            if (tileID != curSelectedTileID)
            {
                cardUI.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            }
            else
            {
                cardUI.transform.localScale = Vector3.one;
            }
        }
    }

    //----------------------------------------------------------------------------------
    /// <summary>
    /// Ÿ�� ī�� ���� ����
    /// </summary>
    public void StartSelectCard(bool isBonusTime)
    {
        if (IsSelectTime)
            return;

        IsSelectTime = true;

        curSelectedTileID = -1;
        selectButton.interactable = false;

        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "ī�� �̱�");
        Time.timeScale = 0;
        UIManager.Instance.ToggleTimeRoundUI(false);
        TileManager.Instance.InitSelectEquipTile();

        List<int> tempList = new List<int>();
        for (int tileID = 1; tileID <= sheetData.DataList.Count; tileID++)
        {
            tempList.Add(tileID);
        }

        int curTotalWeight = TileDataManager.Instance.totalWeight;

        for (int i = 0; i < selectableCardCount; i++)
        {
            //int randomTileID = tempList [Random.Range(0, tempList.Count - 1)];

            int randomValue = Random.Range(0, curTotalWeight);
            int selectedTileID = -1;

            for (int j = 0; j < tempList.Count; j++) 
            {
                int tileID = tempList[j];
                int targetWeight = sheetData.DataList[tileID].Rate;

                if (randomValue < targetWeight)
                {
                    selectedTileID = tileID;
                    curTotalWeight -= targetWeight;
                    tempList.Remove(tileID);

                    break;
                }

                randomValue -= targetWeight;
            }

            //GameObject newCard = CreateCardUI(randomTileID);

            GameObject newCard;

            if (AllManager.instance.isTutorial && i == 1)
                newCard = CreateCardUI(4);
            else
                newCard = CreateCardUI(selectedTileID);

            cardList.Add(selectedTileID, newCard);
        }

        TogglecardSelectFrame(true, isBonusTime);
    }

    /// <summary> 
    /// Ÿ�� ī�� ���� ����
    /// </summary>
    public void OnEndSelectCard()
    {
        bool isTutorial = AllManager.instance.isTutorial;

        if (!isTutorial && curSelectedTileID == -1)
            return;        

        int tileID = isTutorial ? 4 :curSelectedTileID;
        int typeID = sheetData.DataList[tileID].TypeID;
        int effectDayTypeID = sheetData.DataList[tileID].EffectDayType;

        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");

        TileManager.Instance.AddGetEquipTileList(tileID, typeID, effectDayTypeID);
        UIManager.Instance.ToggleTimeRoundUI(true);

        Time.timeScale = 1;

        foreach (KeyValuePair<int, GameObject> item in cardList)
        {
            Destroy(item.Value);
        }

        /*
        for (int i = cardList.Count - 1; i >= 0; i--)
        {
            Destroy(cardList[i]);
        }
        */

        cardList.Clear();

        IsSelectTime = false;
        TogglecardSelectFrame(false);

        TileEffectManager tileEffectManagerInst = TileEffectManager.Instance;

        if (tileEffectManagerInst.AddTilePeriod != 0
            && tileEffectManagerInst.CurTilePeriod != 0
            && tileEffectManagerInst.CurTilePeriod == tileEffectManagerInst.AddTilePeriod)
        {
            StartSelectCard(true);
            tileEffectManagerInst.CurTilePeriod = 0;
        }

        //ī�� ���� ���� �ڿ� �Ҹ� ���
        if (GameManager.Instance.isWarningTime)
        {
            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "���� ����");
        }
    }

    private void TogglecardSelectFrame(bool state, bool isBonusTime = false)
    {
        if (state)
        {
            if (isBonusTime)
            {
                cardSelectTitleText.text = "���ʽ� ī�� ����";
            }
            else
            {
                cardSelectTitleText.text = "ī�� ����";
            }

            //Toggle_TileSelectButton(false);

            selectCardListRoot.transform.DOLocalMoveY(-10, 0.7f) //510
                .SetUpdate(true)
                .SetEase(Ease.OutCubic)
                .OnComplete(()=> { 
                    selectButton.gameObject.SetActive(true);
                    blockImage.SetActive(false);
                });
        }
        else
        {
            selectCardListRoot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -320, 0);
            selectButton.gameObject.SetActive(false);
            blockImage.SetActive(true);
        }

        cardSelectFrame.gameObject.SetActive(state);
    }

    //----------------------------------------------------------------------------------
    public bool IsSelectTime
    {
        get
        {
            return isSelectTime;
        }
        set
        {
            isSelectTime = value;
        }
    }
}
