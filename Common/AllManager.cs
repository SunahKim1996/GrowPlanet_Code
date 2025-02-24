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

        //HACK: Play �� �ϸ� ����� �ȵǾ, Stop �� Play �ϰ� ��
        touchFX.Stop();
        touchFX.Play();
    }

    public void LoadTutorial()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");

        LoadSceneManager.LoadScene("Tutorial");
        //SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    public void LoadGameScene()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
        //SoundManager.instance.StopTargetAudio(SoundType.BGM, "�κ�", true);

        LoadSceneManager.LoadScene("Game");
    }
        
    /// <summary>
    /// ����� ���ư ó��
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

        PopupCanvas.instance.ShowPopupUI("������ �����ұ��?", "��", "�ƴϿ�",
            () =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");

                #if UNITY_EDITOR
                    //������ ����
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
			    	//���ø����̼� ����
                    Application.Quit();                                                
                #endif

                isFinishAppChecking = false;
            },

            () =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
                Time.timeScale = preTimeScale;
                isFinishAppChecking = false;
            });
    }

    /// <summary>
    /// �ε� ������ �Ǵ��� üũ 
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
