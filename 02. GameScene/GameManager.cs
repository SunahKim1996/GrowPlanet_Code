using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Reward")] //���⹰ ����
    [SerializeField] private float initValue_GoalReward;
    //[SerializeField] private float addValue_GoalReward;
    private float curGoalReward;

    private float totalReward = 0;
    private int curRound = 1;

    [Header("Limit Timer")] //���� �ð�
    [SerializeField] private float limitTime;
    //[SerializeField] private float addValue_LimitTime;
    [SerializeField] private float turnTime;
    [SerializeField] private float warningTime;
    [HideInInspector] public bool isWarningTime = false;
    [HideInInspector] public float curTime;
    private bool isTurnCoolTime = false;

    [Header("Game Speed")] //�ӵ�
    private float speedChangeValue = 1f;

    [Header("Tile Crack")] //������
    public float tileCrackTime, tileDeletTime;
    public int crackTouchCount;

    [Header("Get Reward FX")] //���ⷮ ȹ�� FX
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

        SoundManager.instance.PlayTargetAudio(SoundType.BGM, "����");
        StartGame();
    }


    //----------------------------------------------------------------------------------    
    /*
    /// <summary>
    /// ���� ��, �͸� ID �ο�
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
        SoundManager.instance.PlayTargetAudio(SoundType.BGM, "�κ�");

        UIManager.Instance.ToggleEndUI(false);
        UIManager.Instance.ToggleStartUI(true);
         
        //�ʱ�ȭ ó��
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

        //���Ͽ� ������ �ð��� �ο��ϱ� ����, ���� ���� �Ǹ� Line ȸ������ ��Ȯ�� ������ �ʱ�ȭ  
        SunLineManager.Instance.RefreshRotValue(0f);

        if (TileEffectManager.Instance.CurTilePeriod <= 0)
            return;

        //������ �Ͽ� �ָ��ϰ� �ð��� ���Ƽ� ī�� ������ �Ǵ� ������ ���� 
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

            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "���� ����");
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
            yield return null; // ���� �����ӱ��� ���
        }

        EndRound();
        yield break;
    }

    /// <summary>
    /// ���� ���� �� ���� ȭ������ �̵� ��ư ������ �� ó��
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
                    SoundManager.instance.PlayTargetAudio(SoundType.SFX, "���� ����");

                isWarningTime = true;
            }
            else
            {
                isWarningTime = false;
            }

            //���ο� �� 
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
