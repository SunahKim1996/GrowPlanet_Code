using PlayNANOO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard instance;

    // 전역변수 생성
    Plugin rankingPlugin;

    [SerializeField] private string tableCode;
    [SerializeField] private int rangeStart;
    [SerializeField] private int rangeEnd;

    //UI
    [SerializeField] private GameObject leaderboardFrame;
    [SerializeField] private GameObject playerSlotListOrigin;
    [SerializeField] private LeaderboardSlot myRecordSlot;
    [SerializeField] private GameObject loadingFrame;
    [SerializeField] TMP_Text playerIDText;

    private LeaderboardSlot[] playerSlotList;
    
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // 인스턴스 활성화
        rankingPlugin = Plugin.GetInstance();
        DontDestroyOnLoad(rankingPlugin);
    }

    void Start()
    {
        InitLeaderboardUI();
        GuestLogin();

        OnToggleLeaderboardUI(false);
    }

    /// <summary>
    /// 리더보드 UI 처리
    /// </summary>
    public void OnToggleLeaderboardUI(bool state)
    {
        if (state)
        {
            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");
            GetLeaderboard();
        }

        leaderboardFrame.SetActive(state);
    }

    private void GuestLogin()
    {
        //loadingFrame.SetActive(true);

        rankingPlugin.AccountManagerV20240401.GuestSignIn((status, errorCode, jsonString, values) => {
            if (status.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log($"GuestLogin [<color=blue>Success</color>]");

                AllManager.instance.playerData.playerID = $"{values["uuid"]}";
                playerIDText.text = $"{values["uuid"]}";
                PlayerPrefs.SetString("PlayerID", $"{values["uuid"]}");

                AllManager.instance.CheckEndLoading(LoadingData.Leaderboard);
            }
            else
            {
                if (values != null)
                {
                    if (values["ErrorCode"].ToString() == "30007")
                    {
                        Debug.Log(values["WithdrawalKey"].ToString());
                    }
                    else if (values["ErrorCode"].ToString() == "70002")
                    {
                        Debug.Log(values["BlockKey"].ToString());
                    }
                    else
                    {
                        Debug.Log($"GuestSignIn [<color=red>Fail 1</color>] / ErrorCode : {errorCode} -------------------------");
                    }
                }
                else
                {
                    Debug.Log($"GuestSignIn [<color=red>Fail 2</color>] / ErrorCode : {errorCode} -------------------------");
                }
            }
        });
    }

    private void InitLeaderboardUI()
    {
        playerSlotList = playerSlotListOrigin.GetComponentsInChildren<LeaderboardSlot>();

        for (int i = 0; i < playerSlotList.Length; i++)
        {
            playerSlotList[i].rank.text = "";
            playerSlotList[i].playerID.text = "";
            playerSlotList[i].rewardRecord.text = "";
        }
    }

    private void SetSlotLeaderboardUI(LeaderboardSlot targetSlot, object rank, object playerID, object rewardRecord, bool isMyData)
    {
        string playerIDText = $"{playerID}";

        if (playerIDText == "")
        {
            playerIDText = AllManager.instance.playerData.playerID;
        }

        if (isMyData)
        {
            playerIDText += " (나)";
            targetSlot.BG.color = new Color32(177, 255, 243, 255);
        }
        else
        {
            targetSlot.BG.color = new Color32(255, 177, 177, 255);
        }

        //기록 없음
        if ((int)rank == -1)
        {
            targetSlot.rank.text = $"-등";
            targetSlot.playerID.text = playerIDText;
            targetSlot.rewardRecord.text = $"기록 없음";

            return;
        }       

        targetSlot.rank.text = $"{rank}등";
        targetSlot.playerID.text = playerIDText;

        string rewardRecordText = (double)rewardRecord % 1 == 0 ? string.Format("{0:0}", rewardRecord) : string.Format("{0:0.00}", rewardRecord);
        targetSlot.rewardRecord.text = rewardRecordText;
    }

    public void GetLeaderboard()
    {
        loadingFrame.SetActive(true);

        rankingPlugin.LeaderboardManagerV20240301.Range(tableCode, rangeStart, rangeEnd, (status, errorCode, jsonString, values) => {
            if (status.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log("GetLeaderboard [<color=blue>Success</color>]");

                bool isInMyData = false;

                foreach (Dictionary<string, object> value in (ArrayList)values["Items"])
                {
                    bool isMyData = false;

                    if ((string)value["RecordId"] == AllManager.instance.playerData.playerID)
                    {
                        isInMyData = true;
                        isMyData = true;
                    }
                    else 
                    {
                        isMyData = false;
                    }

                    int index = (int)value["Rank"] - 1;
                    SetSlotLeaderboardUI(playerSlotList[index], value["Rank"], value["RecordId"], value["Score"], isMyData);
                }

                if (!isInMyData)
                    GetMyData();

                myRecordSlot.gameObject.SetActive(!isInMyData);

                loadingFrame.SetActive(false);
            }
            else
            {
                Debug.Log($"GetLeaderboard [<color=red>Fail</color>] / ErrorCode : {errorCode} -------------------------");
            }
        });
    }

    public void ComparePreScore(string playerID, float score)
    {
        if (score <= 0)
        {
            Debug.Log("<color=orange>None Record</color>");
            return;
        }
        else
        {
            rankingPlugin.LeaderboardManagerV20240301.Show(tableCode, playerID, (status, errorCode, jsonString, values) =>
            {
                if (status.Equals(Configure.PN_API_STATE_SUCCESS))
                {
                    Debug.Log("GetMyData in SetLeaderboard [<color=blue>Success</color>]");

                    if (score <= (double)values["Score"])
                    {
                        Debug.Log("<color=orange>None Record</color>");
                    }
                    else
                    {
                        SetLeaderboard(playerID, score);
                    }
                }
                else
                {
                    Debug.Log($"GetMyData in SetLeaderboard [<color=red>Fail</color>] / ErrorCode : {errorCode} -------------------------");
                }
            });
        }
    }

    public void SetLeaderboard(string playerID, float score)
    {
        rankingPlugin.LeaderboardManagerV20240301.Record(tableCode, playerID, score, "", (status, errorCode, jsonString, values) => {
            if (status.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log("SetLeaderboard [<color=blue>Success</color>]");
            }
            else
            {
                Debug.Log($"SetLeaderboard [<color=red>Fail</color>] / ErrorCode : {errorCode} -------------------------");
            }
        });
    }

    private void GetMyData()
    {
        string recordId = AllManager.instance.playerData.playerID;

        rankingPlugin.LeaderboardManagerV20240301.Show(tableCode, recordId, (status, errorCode, jsonString, values) => {
            if (status.Equals(Configure.PN_API_STATE_SUCCESS))
            {
                Debug.Log("GetMyData [<color=blue>Success</color>]");

                SetSlotLeaderboardUI(myRecordSlot, values["Rank"], values["RecordId"], values["Score"], true);
            }
            else
            {
                Debug.Log($"GetMyData [<color=red>Fail</color>] / ErrorCode : {errorCode} -------------------------");
            }
        });
    }
}