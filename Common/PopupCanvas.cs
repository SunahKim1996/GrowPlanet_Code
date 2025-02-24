using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCanvas : MonoBehaviour
{
    public static PopupCanvas instance;

    [Header("Popup")]
    [SerializeField] private GameObject popupUI;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;

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
        popupUI.SetActive(false);
    }

    //----------------------------------------------------------------------------------
    /// <summary>
    /// 공용 팝업 UI 처리
    /// </summary>
    public delegate void Button1Callback();
    public delegate void Button2Callback();

    public void ShowPopupUI(string contentsText, string button1Text, string button2Text, Button1Callback button1Callback, Button2Callback button2Callback)
    {
        popupText.text = contentsText;

        button1.GetComponentInChildren<TMP_Text>().text = button1Text;
        button1.onClick.AddListener(() =>
        {
            if (button1Callback != null)
            {
                button1Callback();
            }
            ClosePopupUI();
        }
        );

        button2.GetComponentInChildren<TMP_Text>().text = button2Text;
        button2.onClick.AddListener(() =>
        {
            if (button2Callback != null)
            {
                button2Callback();
            }

            ClosePopupUI();
        }
        );

        popupUI.SetActive(true);
    }

    private void ClosePopupUI()
    {
        popupUI.SetActive(false);

        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
    }
}
