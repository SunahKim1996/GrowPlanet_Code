using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;  
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    public Canvas mainCanvas;

    private TileDataManager tileDataManagerInst;
    private SheetData sheetData;
    private SheetTextData sheetTextData;

    [Header("CurGameInfo")]
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject roundUI;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text curRewardText;
    [SerializeField] private TMP_Text goalRewardText;

    /*
    [Header("CardUI")]
    [SerializeField] private GameObject cardUI;
    [SerializeField] private TMP_Text cardTitleText;
    [SerializeField] private TMP_Text cardEffectText;
    [SerializeField] private Image tilePropertyImage;
    [SerializeField] private Image timePropertyImage;
    [SerializeField] private TMP_Text rewardText;
    */


    [Header("SynergyEffectUI")]
    [SerializeField] private GameObject synergyEffectListRoot;
    [SerializeField] private GameObject synergyEffectSlot;
    [SerializeField] private GameObject synergyEffectInfoPopup;
    //[SerializeField] private TMP_Text synergyEffectInfoText;
    [SerializeField] private TMP_Text[] synergyEffectInfoTexts;


    [SerializeField] private GameObject closeButton;

    [Header("GameUI")]
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text endResultText;
    [SerializeField] private GameObject noticeUI;
    [SerializeField] private TMP_Text noticeText;


    public enum ContentsType
    {
        Terms,
        PrivacyPolicy,
    }

    /*
    [SerializeField] private GameObject equipTileCardUI;
    [SerializeField] private TMP_Text equipTileCardText;
    */
    void Start()
    {
        //tileManagerInst = TileManager.Instance;
        tileDataManagerInst = TileDataManager.Instance;
        sheetData = tileDataManagerInst.sheetData;
        sheetTextData = tileDataManagerInst.sheetTextData;

        curRewardText.text = GameManager.Instance.TotalReward.ToString();        

        //synergyEffectSlot.gameObject.SetActive(false);
        synergyEffectSlot.SetActive(false);
        synergyEffectInfoPopup.SetActive(false);
        ToggleCloseButton(false);
        //equipTileCardUI.SetActive(false);

        //GenerateSynergyEffectUI();
        ToggleEndUI(false);
        RefreshRoundUI(1);
    }

    //----------------------------------------------------------------------------------
    public void ToggleEndUI(bool state)
    {
        endScreen.SetActive(state);
    }

    public void ShowEndUI(float resultReward)
    {
        endResultText.text = $"당신의 행성은\n<color=orange>{resultReward}</color> 에 낙찰되었습니다.";
        ToggleEndUI(true);
        CommonUIManager.instance.ToggleMenuButton(false);
    }

    public void ToggleNoticeUI(bool state)
    {
        noticeUI.SetActive(state);
    }

    public void ShowNoticeUI(string nameText)
    {
        noticeText.text = $"{nameText} 새로운 목표 가격을 제시했습니다!";
        ToggleNoticeUI(true);

        noticeUI.transform.DOMoveX(0f, 1f)
            .SetUpdate(true)
            .OnComplete(() => {
                //Invoke("InitNoticeUI", 1f); //TimeScale 이 0 일 때 발동 안함 

                noticeUI.transform.DOMoveX(-300f, 0.5f)
                .SetUpdate(true)
                .SetDelay(1f)
                .SetEase(Ease.InExpo)
                .OnComplete(() => { 
                    ToggleNoticeUI(false);
                });
            });
    }

    /*
    private void InitNoticeUI()
    {
        ToggleNoticeUI(false);
        noticeUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(-300f, 0f, 0f);
    }
    */

    public void RefreshRoundUI(int round)
    {
        roundText.text = $"{round} 턴";
    }

    public void Toggle_SynergyEffectInfoPopup(bool state)
    {
        synergyEffectInfoPopup.SetActive(state);
    }

    /// <summary>
    /// 참고 : TileEffectManager 의 Start 에서 불리는 함수여서, MangerInst 변수 사용 X
    /// </summary>
    public GameObject GenerateSynergyEffectUI(int typeID, int count, int curSynergyNumID)
    {          
        GameObject newSlot = Instantiate(synergyEffectSlot, synergyEffectListRoot.transform);

        Transform icon = newSlot.transform.GetChild(1);
        icon.GetComponent<Image>().sprite = TileDataManager.Instance.tileSprites[typeID][0];

        Transform frame = newSlot.transform.GetChild(0);

        TMP_Text curTileCountText = frame.GetChild(0).GetComponent<TMP_Text>();
        curTileCountText.text = $"{count}";

        TMP_Text typeNameText = frame.GetChild(1).GetComponent<TMP_Text>();
        typeNameText.text = TileDataManager.Instance.sheetTextData.DataList[typeID].TypeText;

        TMP_Text synergyInfo = frame.GetChild(2).GetComponent<TMP_Text>();

        List<int> synergyEffects = new List<int>();
        for (int tileID = 1; tileID < TileDataManager.Instance.sheetData.DataList.Count; tileID++)
        {
            if (TileDataManager.Instance.sheetData.DataList[tileID].TypeID == typeID)
            {
                synergyEffects = TileDataManager.Instance.sheetData.DataList[tileID].SynergyNums;
                break;
            }
        }

        string text = "";
        for (int i = 0; i < synergyEffects.Count; i++)
        {
            text += $"{synergyEffects[i]}";

            if (i < synergyEffects.Count - 1)
            {
                text += " > ";
            }
        }
        synergyInfo.text = text;

        int a = typeID;
        newSlot.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");
                OnShowSynergyEffectInfoPopup(a);
            }
        ); 

        newSlot.SetActive(true);

        return newSlot;
    }

    public void RefreshSynergyEffectUI_Count(GameObject targetSlot, int count)
    {
        Transform frame = targetSlot.transform.GetChild(0);
        TMP_Text curTileCountText = frame.GetChild(0).GetComponent<TMP_Text>();

        curTileCountText.text = $"{count}";
    }

    public void RefreshSynergyEffectUI_CurSynergyInfo(int tileID, int nearByTileCount)
    {
        int typeID = sheetData.DataList[tileID].TypeID;
                
        List<int> effectNums = sheetData.DataList[tileID].SynergyNums;

        string text = "";
        bool isOnEffect = false;

        for (int i = 0; i < effectNums.Count; i++)
        {
            string colorText = "";

            if (nearByTileCount >= effectNums[i])
            {
                TileEffectManager.Instance.RefreshCurSynergyNumID(typeID, i);
                isOnEffect = true;

                colorText = "#FF3100";
            }
            else
            {
                colorText = "white";
            }

            text += $"<color={colorText}>{effectNums[i]}</color>";

            if (i < effectNums.Count - 1)
            {
                text += " > ";
            }
        }

        if (isOnEffect == false)
        {
            TileEffectManager.Instance.RefreshCurSynergyNumID(typeID, -1);
        }        

        GameObject targetSlot = TileEffectManager.Instance.curSynergyEffectData_ByType[typeID].slot;

        Transform frame = targetSlot.transform.GetChild(0);
        TMP_Text synergyInfo = frame.GetChild(2).GetComponent<TMP_Text>();
        synergyInfo.text = text;
    }

    public void OnShowSynergyEffectInfoPopup(int typeID)
    {
        Vector2 touchPos = Input.mousePosition;
        touchPos.x = 300f;
        synergyEffectInfoPopup.transform.position = touchPos;

        RefreshSynergyEffectUI_InfoPopup(typeID);
        synergyEffectInfoPopup.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(synergyEffectInfoPopup.GetComponent<RectTransform>());

        if (!AllManager.instance.isTutorial)
        {
            ToggleCloseButton(true);
        }
    }
    
    public void OnClosePopupUI()
    {
        synergyEffectInfoPopup.SetActive(false);
        CardUIManager.Instance.Toggle_CardUI(false);
        //ToggleEquipTileCardUI(false);

        ToggleCloseButton(false);
    }

    /// <summary>
    /// 음수 & 0 & 양수 여부에 따른 텍스트 색상 정보 string으로 return
    /// </summary>
    string Get_TextColor(int value)
    {
        string colorText = "";

        if (value < 0)
        {
            colorText = "#FF3E49";
        }
        else if(value == 0)
        {
            colorText = "white";
        }
        else if (value > 0)
        {
            colorText = "#00B8FF";
        }

        return colorText;
    }

    private void RefreshSynergyEffectUI_InfoPopup(int typeID)
    {
        int numID = TileEffectManager.Instance.curSynergyEffectData_ByType[typeID].curSynergyNumID;

        /*
        if (numID == -1)
        {
            synergyEffectInfoText.text = "현재 적용된 효과가 없습니다.";
        }
        else
        {
            List<int> synergyEffects = new List<int>();

            for (int tileID = 1; tileID < sheetData.DataList.Count; tileID++)
            {                
                if (sheetData.DataList[tileID].TypeID == typeID)
                {
                    synergyEffects = sheetData.DataList[tileID].SynergyEffects;
                    break;
                }
            }
                
            int effectID = synergyEffects[numID]; 
            string effectText = sheetTextData.DataList[effectID].SynergyEffectText;
            List<string> effectValue = sheetTextData.DataList[effectID].SynergyEffectValue;

            string text = "";
            string targetString = effectText;

            for (int i = 0; i < effectValue.Count; i++) 
            {
                string[] temps = targetString.Split(effectValue[i]);
                text += temps[0];

                string colorText = Get_TextColor(int.Parse(Regex.Replace(effectValue[i], @"[^0-9-]", "")));
                text += $"<color={colorText}>{effectValue[i]}</color>";

                if (i == effectValue.Count - 1)
                {
                    text += temps[1];
                }
                else
                {
                    targetString = temps[1];
                }
            }

            synergyEffectInfoText.text = text;
        }
        */

        List<int> synergyEffects = new List<int>();

        for (int tileID = 1; tileID < sheetData.DataList.Count; tileID++)
        {
            if (sheetData.DataList[tileID].TypeID == typeID)
            {
                synergyEffects = sheetData.DataList[tileID].SynergyEffects;
                break;
            }
        }

        for (int i = 0; i < synergyEffectInfoTexts.Length; i++) 
        {
            if (i < synergyEffects.Count)
            {
                int effectID = synergyEffects[i];
                string effectText = sheetTextData.DataList[effectID].SynergyEffectText;

                if (i == numID)
                {
                    List<string> effectValue = sheetTextData.DataList[effectID].SynergyEffectValue;

                    string text = "";
                    string targetString = effectText;

                    for (int j = 0; j < effectValue.Count; j++)
                    {
                        string[] temps = targetString.Split(effectValue[j]);
                        text += temps[0];

                        string colorText = Get_TextColor(int.Parse(Regex.Replace(effectValue[j], @"[^0-9-]", "")));
                        text += $"<color={colorText}>{effectValue[j]}</color>";

                        if (j == effectValue.Count - 1)
                        {
                            text += temps[1];
                        }
                        else
                        {
                            targetString = temps[1];
                        }
                    }

                    synergyEffectInfoTexts[i].text = text;
                    synergyEffectInfoTexts[i].color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    synergyEffectInfoTexts[i].text = effectText;
                    synergyEffectInfoTexts[i].color = new Color32(255, 255, 255, 100);
                }
                                
                synergyEffectInfoTexts[i].gameObject.SetActive(true);
            }
            else
            {
                synergyEffectInfoTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void ToggleTimeRoundUI(bool state)
    {
        roundUI.SetActive(state);
        timerUI.SetActive(state);
    }

    /*
    public void ToggleRoundUI(bool state)
    {
        roundUI.SetActive(state);
    }

    public void ToggleTimerUI(bool state)
    {
        timerUI.SetActive(state);
    }
    */

    public void RefreshTimerUI(float curTime, float warningTime)
    {
        //curTime++;

        if (curTime <= warningTime) 
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }

        TimeSpan timeSpan = TimeSpan.FromSeconds(curTime);
        string timeText = timeSpan.ToString(@"mm\:ss\:ff");

        timerText.text = timeText;
    }

    public void RefreshGoalRewardUI(float curValue, float preValue)
    {
        //goalRewardText.text = Math.Floor(value).ToString();

        if (preValue > 0)
            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "목표 점수 상승");

        StartCoroutine(CountEffect());
        IEnumerator CountEffect()
        {
            float duration = 0.6f; // 카운팅에 걸리는 시간
            float offset = (curValue - preValue) / duration;

            while (preValue < curValue)
            {
                preValue += offset * Time.unscaledDeltaTime;
                goalRewardText.text = ((int)preValue).ToString();
                goalRewardText.color = new Color32(255, 150, 0, 255);

                yield return null;
            }

            preValue = curValue;
            goalRewardText.text = ((int)preValue).ToString();
            goalRewardText.color = Color.black;
        }
    }

    public void RefreshCurRewardUI(float value)
    {
        curRewardText.text = Math.Floor(value).ToString();
    }

    public void ToggleCloseButton(bool state)
    {
        closeButton.SetActive(state);
    }

    /*
    public void Toggle_startScreenFrame(bool state)
    {
        startScreenFrame.gameObject.SetActive(state);

        if (state)
        {
            Color color = startScreenFrame.color;
            color.a = 0f;
            startScreenFrame.color = color;

            StartCoroutine("FadeOut");
        }
        else
        {
            StopCoroutine("FadeOut");
        }  
    }
    
    IEnumerator FadeOut() 
    {
        while (startScreenFrame.color.a <= 1f)
        {
            Color color = startScreenFrame.color;
            color.a += 0.01f;
            startScreenFrame.color = color;
            
            if (color.a > 1f)
            {
                Toggle_startScreenFrame(false);
                GameManager.Instance.StartGame();

                yield break;
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    */
    /*
    public void ToggleEquipTileCardUI(bool state, DefaultTile targetTile = null, Vector2 touchPos = default)
    {
        closeButton.SetActive(state);
        equipTileCardUI.SetActive(state);

        if (state)
        {
            Debug.Log($"TouchPos {touchPos}");
            equipTileCardUI.transform.position = touchPos;

            Dictionary<string, float> scoreValues = targetTile.GetScoreValueList();

            float totalValue = scoreValues["totalScore"];
            float originReward = scoreValues["originalReward"];

            string totalValueText = $"{totalValue}";
            if (totalValue < originReward)
            {
                totalValueText += $"<color=orange> ( {totalValue - originReward} ▼)</color>";
            }
            else if (totalValue > originReward)
            {
                totalValueText += $"<color=green> ( {totalValue - originReward} ▲)</color>";
            }

            string addColor = "white";
            if (scoreValues["addValueText"] != 0)
            {
                addColor = Get_TextColor(1);
            }

            string reduceText = $"{scoreValues["reduceValueText"]}";
            string reduceColor = "white";
            if (scoreValues["reduceValueText"] != 0)
            {
                reduceText = $"-{scoreValues["reduceValueText"]}";
                reduceColor = Get_TextColor(-1);
            }

            string text = $"기본 산출량 : {originReward}" +
                          $"\n인접 효과 추가량 : <color={addColor}>{scoreValues["addValueText"]} %</color>" +
                          $"\n인접 효과 감소량 : <color={reduceColor}>{reduceText} %</color>" +
                          $"\n그 외 시너지 효과 변화량 : {scoreValues["synergyValue"]} %" +
                          $"\n\n총 산출량 : {totalValueText}";


            equipTileCardText.text = text;
        }
    }
    */
}
