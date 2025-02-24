using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonUIManager : MonoBehaviour
{
    public static CommonUIManager instance;

    [Header("Loading")]
    [SerializeField] private GameObject loadingFrame;

    [Header("MenuUI")]
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject toMainScreenButton;
    private float preTimeScale;

    [Header("MenuContentsFrame")]
    [SerializeField] private GameObject contentsFrame;
    [SerializeField] private TMP_Text contentsText;
    [SerializeField] private Scrollbar contentsScrollbar;
    [SerializeField] private string termsText;
    [SerializeField] private string privacyPolicyText;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() 
    {
        contentsFrame.SetActive(false);
        ToggleMenuUI(false);
        ToggleLoadingFrame(true);
    }

    //----------------------------------------------------------------------------------
    
    public void ToggleLoadingFrame(bool state)
    {
        loadingFrame.SetActive(state);
    }

    //----------------------------------------------------------------------------------
    /// <summary>
    /// ���� �޴� UI ó��
    /// </summary>
    public void OnToggleMenuUI(bool state)
    {
        ToggleMenuUI(state);

        if (state)
        {
            preTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
        }
        else
        {
            Time.timeScale = preTimeScale;
        }
    }

    public void ToggleMenuUI(bool state)
    {
        menuUI.SetActive(state);
    }

    public void ToggleToMainButton(bool state)
    {
        toMainScreenButton.SetActive(state);
    }
    public void ToggleMenuButton(bool state)
    {
        menuButton.SetActive(state);
    }

    //----------------------------------------------------------------------------------
    /// <summary>
    /// �޴� ������ UI ó��
    /// </summary>
    private void ShowContentsUI()
    {
        contentsFrame.SetActive(true);
        contentsScrollbar.value = 1f;
    }

    public void OnShowTermsPopup()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
        contentsText.text = termsText;
        ShowContentsUI();
    }

    public void OnShowPrivatePolicyPopup()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
        contentsText.text = privacyPolicyText;
        ShowContentsUI();
    }

    public void OnCloseContentsUI()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
        contentsFrame.SetActive(false);
    }

    //----------------------------------------------------------------------------------
    /// <summary>
    /// �޴� > ����ȭ������ ��ư ������ �� ó��
    /// </summary>
    public void OnGoToMainScreen()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");

        PopupCanvas.instance.ShowPopupUI("���� ȭ������\n���ư����?", "��", "�ƴϿ�",
            () =>
            {
                //OnReturnStartScreen();
                ToggleMenuUI(false);
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
                LoadSceneManager.LoadScene("Start");
            },
            () =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
            }
        );
    }
}
