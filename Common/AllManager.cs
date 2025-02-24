using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerID;
    public float bgmVolume;
    public float sfxVolume;
}
public enum LoadingData
{
    GoogleSheet,
    Leaderboard,
}

public class AllManager : MonoBehaviour
{
    public static AllManager instance;

    private bool isFinishAppChecking = false;
    public PlayerData playerData;

    public bool isTutorial;
    public List<GameObject> tutorialImage;

    [SerializeField] private ParticleSystem touchFX;


    private bool isGoogleSheetDataLoaded = false;
    private bool isLeaderboardLoaded = false;

    void Awake()
    {
        if(instance != null)
        {            
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        playerData = new PlayerData();
    }

    //----------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            FinishApp();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlayTouchFX();
        }
    }

    //----------------------------------------------------------------------------------
    private void PlayTouchFX()
    {           
        Vector2 touchPos = Input.mousePosition;
        touchFX.transform.position = touchPos;

        //HACK: Play 만 하면 재생이 안되어서, Stop 후 Play 하게 함
        touchFX.Stop();
        touchFX.Play();
    }

    public void LoadTutorial()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");

        LoadSceneManager.LoadScene("Tutorial");
        //SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    public void LoadGameScene()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");
        //SoundManager.instance.StopTargetAudio(SoundType.BGM, "로비", true);

        LoadSceneManager.LoadScene("Game");
    }
        
    /// <summary>
    /// 모바일 백버튼 처리
    /// </summary>
    private void FinishApp()
    {
        if (isFinishAppChecking)
        {
            return;
        }

        isFinishAppChecking = true;
        float preTimeScale = Time.timeScale;
        Time.timeScale = 0;

        PopupCanvas.instance.ShowPopupUI("게임을 종료할까요?", "예", "아니오",
            () =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");

                #if UNITY_EDITOR
                    //에디터 종료
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
			    	//어플리케이션 종료
                    Application.Quit();                                                
                #endif

                isFinishAppChecking = false;
            },

            () =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");
                Time.timeScale = preTimeScale;
                isFinishAppChecking = false;
            });
    }

    /// <summary>
    /// 로딩 끝내도 되는지 체크 
    /// </summary>
    public void CheckEndLoading(LoadingData dataType)
    {
        if (dataType == LoadingData.GoogleSheet)
            isGoogleSheetDataLoaded = true;
        
        else if (dataType == LoadingData.Leaderboard)
            isLeaderboardLoaded = true;

        if (!isGoogleSheetDataLoaded || !isLeaderboardLoaded)
            return;

        CommonUIManager.instance.ToggleLoadingFrame(false);
    }
}
