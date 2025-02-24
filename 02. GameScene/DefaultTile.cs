using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TileDataManager;
using static TileEffectManager;

public class DefaultTile : MonoBehaviour
{
    private TileManager tileManagerInst;
    private SheetData sheetData;

    //해당 타일 데이터
    //디버깅을 위해 [HideInInspector] 하지 않음
    public Dictionary<string, int> hexaCoordinate = new Dictionary<string, int>();
    
    public TileProperty tileProperty = TileProperty.None;
    public TimeProperty timeProperty = TimeProperty.None;
    public int tileID = -1;

    //UI
    public Image bgImage;
    public Image timeBG;
    public Image timeValue;
    public Image timeIcon;
    public Image stroke;
    [SerializeField] private Image crackImage;
    [SerializeField] private Text[] coordinateTexts;

    //내구도
    private float curCrackTime, curDeleteTime;
    private int curCrackTouchCount = 0;
    private bool isCrackTime = false;

    //인접 효과
    private float curAdditionalRewardCount = 0;
    private float curReducedRewardCount = 0;

    //시너지 효과
    [HideInInspector] public SynergyEffectValue synergyEffectValue;

    //타일 효과
    private float curEffectTime = 0;

    //효과 파티클
    [SerializeField] private GameObject effectParticle;

    private float pressTime = 0.5f;
    private float curPressTime;
    

    //----------------------------------------------------------------------------------
    void Start()
    {
        tileManagerInst = TileManager.Instance;
        sheetData = TileDataManager.Instance.sheetData;

        GenerateTileCoordinate();
        SetCoordinateText();

        timeBG.gameObject.SetActive(false);
        crackImage.gameObject.SetActive(false);
        timeValue.fillAmount = 0;

        synergyEffectValue = new SynergyEffectValue();

        effectParticle.GetComponent<ParticleSystem>().Stop();
        transform.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        stroke.gameObject.SetActive(false);
    }

    //----------------------------------------------------------------------------------
    private void GenerateTileCoordinate()
    {
        string[] coordinateArray = gameObject.name.Split(',');

        hexaCoordinate.Add("q", int.Parse(coordinateArray[0]));
        hexaCoordinate.Add("r", int.Parse(coordinateArray[1]));
        hexaCoordinate.Add("s", int.Parse(coordinateArray[2]));
    }
    private void SetCoordinateText()
    {
        if (tileManagerInst.isShowTileCoordinate == true)
        {
            string[] coordinateArray = gameObject.name.Split(',');

            for (int i = 0; i < coordinateTexts.Length; i++)
            {
                coordinateTexts[i].text = coordinateArray[i];
            }
        }
        else
        {
            for (int i = 0; i < coordinateTexts.Length; i++)
            {
                coordinateTexts[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 타일 초기화
    /// </summary>
    public void InitDefaultTile()
    {
        if (tag.Equals("EquipTile"))
        {
            int preTileID = tileID;
            int preTypeID = (int)tileProperty;

            CurEffectTime = 0;

            //tileManagerInst.ChangeTileProperty(this, 0, 0, 0);
            crackImage.gameObject.SetActive(false);

            TileEffectManager.Instance.CheckAllEffect(this, preTileID, hexaCoordinate["q"], hexaCoordinate["r"], preTypeID, -1);
            CurAdditionalRewardCount = 0;
            CurReducedRewardCount = 0;

            //tileManagerInst.assignedTiles.Remove(gameObject);

            CardUIManager.Instance.Check_CardUI(gameObject);

            StopCoroutine("CheckCrackTime");
        }

        tileManagerInst.assignedTiles.Remove(gameObject);
        tileManagerInst.ChangeTileProperty(this, 0, 0, 0);
        InitCheckCrakTime();
    }

    /// <summary>
    /// 장착 상태로 변경
    /// </summary>
    public void OnChangeDefaultTile()
    {
        bool isTutorial = AllManager.instance.isTutorial;
        int curEquipTileID = tileManagerInst.CurEquipTileID;

        if (isTutorial)
        {
            curEquipTileID = 4;
        }
        else if (curEquipTileID == -1 || tileProperty != TileProperty.None)
        {
            return;
        }

        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "타일 설치");

        tileManagerInst.Change_DefaultToOtherTile(gameObject, TileState.EquipTile);

        tileManagerInst.DeleteEquipTile();
        tileManagerInst.InitSelectEquipTile();

        TileEffectManager.Instance.CheckAllEffect(this, tileID, hexaCoordinate["q"], hexaCoordinate["r"], (int)tileProperty, 1);

        //타일 내구도 체크 시작
        if (!isTutorial)
        {
            StartCoroutine("CheckCrackTime");
        }
    }

    public float GetTotalScore(int tileID)
    {
        float totalReward = 0;
        int originalReward = sheetData.DataList[tileID].Reward;

        float curAdditionalReward = CurAdditionalRewardCount * (sheetData.DataList[tileID].AdditionalRewardPercent + synergyEffectValue.addReward_Value);
        float curReducedReward = CurReducedRewardCount * (sheetData.DataList[tileID].ReducedRewardPercent);

        totalReward = (originalReward * synergyEffectValue.reward_MultifyValue) 
                    + (originalReward * (curAdditionalReward - curReducedReward + synergyEffectValue.addReward_Ice_Value) / 100);

        return totalReward;
    }

    /*
    public Dictionary<string, float> GetScoreValueList()
    {
        Dictionary<string, float> scoreValues = new Dictionary<string, float>();

        float originalReward = sheetData.DataList[tileID].Reward;
        float addValueText = CurAdditionalRewardCount * (sheetData.DataList[tileID].AdditionalRewardPercent + synergyEffectValue.addReward_Value);
        float reduceValueText = CurReducedRewardCount * sheetData.DataList[tileID].ReducedRewardPercent;

        scoreValues["originalReward"] = originalReward;
        scoreValues["addValueText"] = addValueText;
        scoreValues["reduceValueText"] = reduceValueText;
        scoreValues["totalScore"] = GetTotalScore(tileID);
        scoreValues["synergyValue"] = synergyEffectValue.addReward_Ice_Value;

        return scoreValues;
    }
    */

    /*
    public void PlayRewardFX()
    {
        Debug.Log("========================");
        StopCoroutine("StopRewardFX");

        rewardFX.SetActive(true);
        rewardFX.GetComponent<Animator>().SetTrigger("GetReward");

        float animationLength = rewardFX.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine("StopRewardFX", animationLength);
    }

    IEnumerator StopRewardFX(float animationLength)
    {
        yield return new WaitForSecondsRealtime(animationLength);
        rewardFX.SetActive(false);
        StopCoroutine("StopRewardFX");
    }
    */

    public void PlayRewardFX()
    {
        GameObject pool = ObjectPoolManager.Instance.ShowObjectPool("DefaultTile", transform);

        float totalReward = GetTotalScore(tileID);
        string text = totalReward % 1 == 0 ? string.Format("{0:0}", totalReward) : string.Format("{0:0.00}", totalReward);

        pool.GetComponentInChildren<TMP_Text>().text = text;
    }
    
    /// <summary>
    /// 타일 효과 (낮밤에 의한)
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("TimeTrigger"))
        {
            return;
        }

        TimeTrigger targetTrigger = collision.gameObject.GetComponent<TimeTrigger>();

        if (timeProperty != TimeProperty.DayAndNight && timeProperty != targetTrigger.timeProperty)
        {
            return;
        }

        float effectTime = sheetData.DataList[tileID].EffectTime;

        if (CurEffectTime >= effectTime)
        {
            CurEffectTime = 0;
            GameManager.Instance.TotalReward = GetTotalScore(tileID);
            //SoundManager.instance.PlayTargetAudio(SoundType.SFX, "점수 획득");
            PlayRewardFX();

            return; 
        }

        CurEffectTime += Time.deltaTime * GameManager.Instance.SpeedChangeValue;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("TimeTrigger"))
        {
            return;
        }

        TimeTrigger targetTrigger = collision.gameObject.GetComponent<TimeTrigger>();

        //낮밤 속성인 경우에는 Exit 해도 시간 계산 이어지게 설정
        if (timeProperty != targetTrigger.timeProperty)
        {
            return;
        }

        CurEffectTime = 0;
    }

    //TODO: 코드 정리
    /// <summary>
    /// 산출량 발생 시, 주변 타일을 랜덤 확률로 모래로 변경
    /// </summary> 
    public void ChangeRandomSendTile()
    {
        int randomInt = Random.Range(1, 100);

        if (randomInt > synergyEffectValue.changeSendValue)
        {            
            return;
        }

        List<DefaultTile> nearByTiles = tileManagerInst.GetNearByTiles(hexaCoordinate["q"], hexaCoordinate["r"]);

        if (nearByTiles.Count <= 0)
        {            
            return;
        }

        int randomIndex = Random.Range(0, nearByTiles.Count);

        //Debug.Log($"randomIndex {randomIndex} / nearByTiles.Count {nearByTiles.Count}");

        DefaultTile tileClass = nearByTiles[randomIndex];

        if (tileClass.tileProperty == TileProperty.None) 
        {
            tileManagerInst.assignedTiles.Add(tileClass.gameObject);
        }

        //tileClass.synergyEffectValue.changeSendValue = this.CurChangeSendValue;

        //
        int typeID = (int)TileProperty.Sand;
        int dayTypeID = (int)timeProperty;
        int targetTileID = -1;

        for (int i = 0; i < TileDataManager.Instance.tileIDs_ByType[typeID].Count; i++)
        {
            int tileID = TileDataManager.Instance.tileIDs_ByType[typeID][i];

            if (sheetData.DataList[tileID].EffectDayType == dayTypeID)
            {
                targetTileID = tileID;
                break;
            }
        }
        
        int q = tileClass.hexaCoordinate["q"];
        int r = tileClass.hexaCoordinate["r"];

        tileManagerInst.ChangeTileProperty(tileClass, typeID, dayTypeID, targetTileID);
        tileClass.gameObject.tag = "EquipTile";
        TileEffectManager.Instance.CheckEffect(targetTileID, q, r, typeID, tileClass, 1);

        nearByTiles.Clear();
    }

    public void OnStartButtonPress()
    {
        if (!this.CompareTag("EquipTile") || curPressTime > 0)
        {
            return;
        }

        transform.localScale = new Vector3(0.8f, 0.8f, 1);
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "타일 프레스");

        curPressTime = 0;
        StartCoroutine("ButtonPress");
    }

    public void OnEndButtonPress() 
    {
        transform.localScale = new Vector3(0.85f, 0.85f, 1);
        SoundManager.instance.StopTargetAudio(SoundType.SFX, "타일 프레스");

        StopCoroutine("ButtonPress");
        curPressTime = 0;
    }

    IEnumerator ButtonPress()
    {
        while (curPressTime < pressTime) 
        {
            curPressTime += Time.deltaTime;
            yield return null;
        }

        Vector2 touchPos = Input.mousePosition;
        //UIManager.Instance.ToggleEquipTileCardUI(true, this, touchPos); //LEGACY
        CardUIManager.Instance.Refresh_CardUI(this, touchPos);

        if (AllManager.instance.isTutorial)
        {
            Tutorial.Instance.EndWaitTime(true);
        }

        OnEndButtonPress();
        yield break;
    }

    IEnumerator ResetTileScale()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale = new Vector3(0.85f, 0.85f, 1);
    }

    public void OnResetCrack()
    {
        if (isCrackTime == false)
        {
            return;
        }

        curCrackTouchCount++;
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "타일 노화");

        if (AllManager.instance.isTutorial)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 1);
            StartCoroutine(ResetTileScale());
        }

        if (curCrackTouchCount >= GameManager.Instance.crackTouchCount)
        {
            crackImage.gameObject.SetActive(false);
            InitCheckCrakTime();

            curCrackTouchCount = 0;

            if (AllManager.instance.isTutorial)
            {
                Tutorial.Instance.EndWaitTime(true);
            }
        }        
    }

    private void InitCheckCrakTime()
    {
        curDeleteTime = 0;
        curCrackTime = 0;
        curCrackTouchCount = 0;
        isCrackTime = false;
    }

    public void SetTileCrack()
    {
        if (!isCrackTime)
        {
            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "타일 노화");
        }

        isCrackTime = true;
        crackImage.gameObject.SetActive(true);
    }

    public void DestroyTile()
    {
        InitDefaultTile();
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "타일 파괴");
    }

    IEnumerator CheckCrackTime()
    {
        while (true)
        {
            if (curCrackTime >= GameManager.Instance.tileCrackTime)
            {
                SetTileCrack();

                curDeleteTime += Time.deltaTime * GameManager.Instance.SpeedChangeValue;

                if (curDeleteTime >= GameManager.Instance.tileDeletTime)
                {
                    DestroyTile();
                    yield break;
                }
            }
            else
            {
                curCrackTime += Time.deltaTime * GameManager.Instance.SpeedChangeValue;
            }

            //yield return new WaitForSeconds(Time.deltaTime);
            yield return null;
        }
    }

    private void PlayParticle(Color targetColor)
    {
        var particle = Instantiate(effectParticle, transform.position, Quaternion.identity) as GameObject;
        particle.transform.SetParent(transform);

        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;

        main.startColor = targetColor;

        Destroy(particle, main.duration + main.startLifetime.constantMax);
    }


    //----------------------------------------------------------------------------------
    public float CurAdditionalRewardCount
    {
        get
        {
            return curAdditionalRewardCount;
        }
        set
        {
            curAdditionalRewardCount = value;

            if (curAdditionalRewardCount != 0)
            {
                PlayParticle(Color.white);
                Debug.Log($"tileID {tileID} : curAdditionalReward {curAdditionalRewardCount * (sheetData.DataList[tileID].AdditionalRewardPercent + synergyEffectValue.addReward_Value)}");
            }
        }
    }

    public float CurReducedRewardCount
    {
        get
        {
            return curReducedRewardCount;
        }
        set
        {
            curReducedRewardCount = value;

            if (curReducedRewardCount != 0)
            {
                PlayParticle(Color.red);
                Debug.Log($"tileID {tileID} : curReducedReward {curReducedRewardCount * sheetData.DataList[tileID].ReducedRewardPercent}");
            }
        }
    }

    public float CurEffectTime
    {
        get
        {
            return curEffectTime;
        }
        set
        {
            curEffectTime = value;

            float effectTime = sheetData.DataList[tileID].EffectTime;
            timeValue.fillAmount = CurEffectTime / effectTime;
        }
    }
}