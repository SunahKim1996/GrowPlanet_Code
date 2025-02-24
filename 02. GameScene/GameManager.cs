using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Reward")] //산출물 점수
    [SerializeField] private float initValue_GoalReward;
    //[SerializeField] private float addValue_GoalReward;
    private float curGoalReward;

    private float totalReward = 0;
    private int curRound = 1;

    [Header("Limit Timer")] //제한 시간
    [SerializeField] private float limitTime;
    //[SerializeField] private float addValue_LimitTime;
    [SerializeField] private float turnTime;
    [SerializeField] private float warningTime;
    [HideInInspector] public bool isWarningTime = false;
    [HideInInspector] public float curTime;
    private bool isTurnCoolTime = false;

    [Header("Game Speed")] //속도
    private float speedChangeValue = 1f;

    [Header("Tile Crack")] //내구도
    public float tileCrackTime, tileDeletTime;
    public int crackTouchCount;

    [Header("Get Reward FX")] //산출량 획득 FX
    [SerializeField] private Transform rewardFXParent;
    [SerializeField] private GameObject rewardFXOrigin;


    //----------------------------------------------------------------------------------
    void Start()
    {
        //SetRandomUserID();

        //Time.timeScale = 0;

        /*
        InitGoalRewardLimitTime();
        SunLineManager.Instance.IsStartSunLine = true;

        TileManager.Instance.GenerateTile();
        */

        ObjectPoolManager.Instance.Init("DefaultTile", rewardFXParent, rewardFXOrigin);

        SoundManager.instance.PlayTargetAudio(SoundType.BGM, "게임");
        StartGame();
    }


    //----------------------------------------------------------------------------------    
    /*
    /// <summary>
    /// 시작 시, 익명 ID 부여
    /// </summary>
    private void SetRandomUserID()
    {
        if (PlayerPrefs.HasKey("PlayerID"))
        {
            playerData.playerID = PlayerPrefs.GetString("PlayerID");
        }
        else
        {
            var now = DateTime.Now.ToLocalTime();
            var span = (now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            int timestamp = (int)span.TotalSeconds;
            string randomID = $"{timestamp}_{UnityEngine.Random.Range(1000, 9999)}";

            playerData.playerID = randomID;
            PlayerPrefs.SetString("PlayerID", playerData.playerID);
        }
        
        playerIDText.text = playerData.playerID;
    }
    */
    public void StartGame()
    {
        if (AllManager.instance.isTutorial)
        {
            SpeedChangeValue = 0f;
            //Time.timeScale = 0;

            UIManager.Instance.ToggleNoticeUI(false);
        }
        else
        {
            SpeedChangeValue = 1f;
            Time.timeScale = 1;

            ShowNotice();
        }

        CommonUIManager.instance.ToggleToMainButton(true);

        InitGoalRewardLimitTime();
        SunLineManager.Instance.IsStartSunLine = true;
        TileManager.Instance.GenerateTile();
        TileManager.Instance.GenerateSpecialTile();

        StartCoroutine("CheckLimitTimer");

        /*
        UIManager.Instance.ToggleStartUI(false);
        TileManager.Instance.GenerateSpecialTile();
        //Time.timeScale = 1;

        //TileManager.Instance.GenerateSpecialTile();
        StartCoroutine("CheckLimitTimer");
        */
    }

    /*
    public void OnReturnStartScreen()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.BGM, "로비");

        UIManager.Instance.ToggleEndUI(false);
        UIManager.Instance.ToggleStartUI(true);
         
        //초기화 처리
        InitGoalRewardLimitTime();                               
        TileManager.Instance.Init_TileList();
        SunLineManager.Instance.InitSunLine(); 
        TileEffectManager.Instance.CurTilePeriod = 0;
        TotalReward = 0;

        Time.timeScale = 1;
    }
    */

    private void InitGoalRewardLimitTime()
    {
        CurGoalReward = initValue_GoalReward;
        CurTime = limitTime;

        StopCoroutine("CheckLimitTimer");
    }

    private void ShowNotice()
    {
        List<string> dataList = GoogleSheetReader.instance.sheetDataList[SheetType.MessageData];

        int randomIndex = UnityEngine.Random.Range(0, dataList.Count);
        string name = dataList[randomIndex];
        UIManager.Instance.ShowNoticeUI(name);
    }

    IEnumerator TurnCoolTime()
    {
        isTurnCoolTime = true;
        yield return new WaitForSecondsRealtime(1f);

        while (CardUIManager.Instance.IsSelectTime)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);
        isTurnCoolTime = false;
    }

    public void NewTurn()
    {
        StartCoroutine(TurnCoolTime());

        TileEffectManager.Instance.CurTilePeriod++;

        //한턴에 일정한 시간을 부여하기 위해, 다음 턴이 되면 Line 회전값을 정확한 값으로 초기화  
        SunLineManager.Instance.RefreshRotValue(0f);

        if (TileEffectManager.Instance.CurTilePeriod <= 0)
            return;

        //마지막 턴에 애매하게 시간이 남아서 카드 선택이 되는 현상을 막음 
        if (curTime <= 30 && !CanGoNextRound())
        {
            return;
        }

        CardUIManager.Instance.StartSelectCard(false);
    }

    public bool CanGoNextRound()
    {
        if (TotalReward >= CurGoalReward)
            return true;

        return false;
    }

    private void EndRound()
    {
        bool canGoNextRound = CanGoNextRound();

        if (canGoNextRound)
        {
            List<string> dataList = GoogleSheetReader.instance.sheetDataList[SheetType.RewardData];
            int curAddValue = -1;

            if (CurRound >= dataList.Count)
            {
                int maxIndex = dataList.Count - 1;
                curAddValue = int.Parse(dataList[maxIndex] + (CurRound - (dataList.Count - 1)) * 100);
            }
            else
            {
                curAddValue = int.Parse(dataList[CurRound]);
            }

            CurGoalReward = TotalReward + TotalReward / 100 * curAddValue;
            TotalReward = TotalReward / 100 * 10;
            CurTime = limitTime;

            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "다음 라운드");
            CurRound++;

            ShowNotice();

            StartCoroutine("CheckLimitTimer");
            //CardUIManager.Instance.StartSelectCard(false);
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        //CommonUIManager.instance.ToggleToMainButton(false);
        //UIManager.Instance.ToggleEndUI(true);
        UIManager.Instance.ShowEndUI(TotalReward);
        Time.timeScale = 0;

        InitGoalRewardLimitTime();

        Leaderboard.instance.ComparePreScore(AllManager.instance.playerData.playerID, TotalReward);
    }

    IEnumerator CheckLimitTimer()
    {
        while (CurTime > 0)
        {
            CurTime -= Time.deltaTime * SpeedChangeValue;
            yield return null; // 다음 프레임까지 대기
        }

        EndRound();
        yield break;
    }

    /// <summary>
    /// 게임 실패 후 메인 화면으로 이동 버튼 눌렀을 때 처리
    /// </summary>
    public void OnGoToMainScreen()
    {
        LoadSceneManager.LoadScene("Start");
    }
    

    //----------------------------------------------------------------------------------
    public float TotalReward
    {
        get
        {
            return totalReward;
        }
        set
        {
            if (value != 0)
            {
                totalReward += value;
            }
            else 
            {
                totalReward = 0;
            }
                        
            UIManager.Instance.RefreshCurRewardUI(totalReward);
            //CheckReward(totalReward);
        }
    }

    public float CurTime
    {
        get
        {
            return curTime;
        }
        set
        {
            curTime = value;

            float curTimeValue = curTime;
            if (curTime <= 0)
                curTimeValue = -1;

            //UI
            UIManager.Instance.RefreshTimerUI(curTimeValue, warningTime);

            if (curTime <= warningTime)
            {                
                if (!isWarningTime)
                    SoundManager.instance.PlayTargetAudio(SoundType.SFX, "점수 부족");

                isWarningTime = true;
            }
            else
            {
                isWarningTime = false;
            }

            //새로운 턴 
            if (!AllManager.instance.isTutorial && !isTurnCoolTime && Math.Abs(curTime % turnTime) < 0.5f)
            {
                NewTurn();
            }
        }   
    }

    public float CurGoalReward
    {
        get
        {
            return curGoalReward;
        }
        set
        {
            float preValue = curGoalReward;
            curGoalReward = value;

            UIManager.Instance.RefreshGoalRewardUI(curGoalReward, preValue);
        }
    }
    public float SpeedChangeValue
    {
        get
        {
            return speedChangeValue;
        }
        set
        {
            speedChangeValue = value;
        }
    }

    public int CurRound
    {
        get
        {
            return curRound;
        }
        set
        {
            curRound = value;
            UIManager.Instance.RefreshRoundUI(curRound);
        }
    }
}
