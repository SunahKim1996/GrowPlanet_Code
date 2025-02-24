using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tutorial : Singleton<Tutorial>
{
    string[] contentsList =
    {
        "당신은 행성을 키워 경매로\n판매하는 판매상입니다.",
        "황폐한 행성에 아름다운\n자연 경관을 만드세요.",
        "다음 경매까지 남은 시간입니다.",

        "다음 경매자가 구매하게끔\n행성의 가치를 높이면 됩니다.",
        "제한시간 안에",
        "행성의 현재 가치인 \"현재 가격\"을",
        "\"목표 가격\"까지 높여주세요.",

        "그러면 이제 가치를 높여 봅시다.",
        "게임이 시작되면 자연 카드를\n하나 선택할 수 있습니다.",
        "일출/일몰선이 한바퀴를 돌 때마다\n자연 카드 획득 기회를 얻습니다.",

        "들판 카드를 선택하세요.",
        "선택한 카드는\n우측 저장소에 추가됩니다.",
        "저장된 타일을 터치해보세요.",
        "빈 바닥에 타일을 설치해보세요.",

        "타일 설치 후 점수를 얻는 방법은\n2가지 입니다.",
        "첫번째는\n타일 낮/밤에 의한 산출입니다.",
        "방금 설치한 타일은 낮에 산출하는 타일이고\n지금 낮에 위치해 있으니\n잠시 뒤에 점수를 산출할 수 있겠군요.",
        "해당 시간대에 얼마나 있었는지는\n타이머를 통해 알 수 있습니다.",
        "타이머가 채워지기 전에 밤이 된다면\n카운트는 초기화 됩니다.",
        "점수를 얻는 2번째 방법입니다.",
        "타일이 일출/일몰선과 충돌하면\n산출량만큼 점수를 획득합니다.",

        "설치된 타일을 <color=red>길게</color> 누르세요.",
        "타일에 대해 자세한 설명이 등장합니다.",
        "해당 부분은 타일이 산출하는\n점수의 수량을 표시합니다.",
        "해당 부분은\n어떤 시간대에 추가적인 산출이\n가능한 타일인지 표시하고 있습니다.",
        "타일의 인접 보너스 입니다.",
        "인접한 타일의 종류에 따라\n산출량이 변화됩니다.",

        "근데...",
        "들판 타일은 옆에 용암 타일이 있으면,\n산출량이 줄어듭니다.",
        "만약 물 타일 옆에 두었다면,\n인접 보너스가 발생하여\n추가 산출량을 얻었을 것입니다.",

        "단순히 타일을 설치하는 것이\n끝은 아닙니다.",
        "타일은 일정 시간 뒤 노화됩니다.",
        "타일을 1번 터치하세요.",
        "그럼 노화는 사라집니다.",
        "만약 일정 시간 터치를 하지 않는다면",
        "타일은 파괴됩니다.",

        "자연 경관 타일은\n설치 개수에 따라 시너지가 발생합니다.",
        "숲 타일은\n\"모든 타일의 인접 보너스 증가\"",
        "들판 타일은\n\"추가 타일 획득\"",
        "얼음 타일은\n\"얼음 타일만 산출량을 크게 상승\"",
        "바위 타일은\n\"모든 타일의 기본 산출량 증가\"",

        "모래 타일은\n\"주변을 모래로 변경\"하는\n시너지가 발생합니다.",

        "만약 게임 속도를 빠르게 하고 싶다면",
        "배속 버튼을 클릭해주세요.",
        "튜토리얼은 여기까지입니다.",
        "메뉴를 통해 메인 화면으로 돌아갑니다.",
        "해당 버튼을 눌러서\n메인 화면으로 돌아갈 수 있습니다."
    };

    [SerializeField] GameObject tutorialImageOriginList;
    //[SerializeField] TMP_Text contents;
    [SerializeField] Image fadeOutFrame;
    [SerializeField] Image firstDim;

    int curIndex = 0;
    public bool IsFadeTime { get; set; }
    public bool IsExplaining { get; set; }

    List<GameObject> tutorialImageList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Init_tutorialImageList();
        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsFadeTime && !IsExplaining) 
        {
            RefreshNextContents();
        }
    }
    /*
    public void ExitTutorialPopup()
    {
        CommonUIManager.instance.ShowPopupUI("튜토리얼을\n중단할까요?", "예", "아니오",
            () =>
            {
                ExitTutorial();
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");
            },
            () =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");
            }
        );
    }
    */

    public void ExitTutorial()
    {
        CommonUIManager.instance.ToggleMenuUI(false);
        LoadSceneManager.LoadScene("Start");
    }

    void RefreshNextContents()
    {
        if (curIndex >= contentsList.Length) 
        {
            ExitTutorial();
            return;
        }

        //튜토리얼 이미지 변경
        if (curIndex > 0)
        {
            tutorialImageList[curIndex - 1].SetActive(false);
        }
        tutorialImageList[curIndex].SetActive(true);

        //튜토리얼 텍스트 변경
        TMP_Text contents = tutorialImageList[curIndex].GetComponentInChildren<TMP_Text>();
        contents.text = contentsList[curIndex];
        LayoutRebuilder.ForceRebuildLayoutImmediate(contents.rectTransform);

        //각 인덱스별 특수 상황 처리 
        CheckIndex();

        curIndex++;
    }

    void Init_tutorialImageList()
    {
        for (int i = 0; i < tutorialImageOriginList.transform.childCount; i++)
        {
            tutorialImageList.Add(tutorialImageOriginList.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < tutorialImageList.Count; i++)
        {
            tutorialImageList[i].SetActive(false);
        }
    }
    IEnumerator FadeOut()
    {
        IsFadeTime = true;
        fadeOutFrame.gameObject.SetActive(true);

        while (fadeOutFrame.color.a > 0)
        {
            Color color = fadeOutFrame.color;
            color.a -= 0.1f;

            fadeOutFrame.color = color;

            if (fadeOutFrame.color.a <= 0)
            {
                IsFadeTime = false;
                fadeOutFrame.gameObject.SetActive(false);
                firstDim.gameObject.SetActive(false);

                RefreshNextContents();

                yield break;
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    void CheckIndex()
    {
        EquipAndSpecialTile targetEquipTile;
        DefaultTile targetDefaultTile = TileManager.Instance.defaultTiles[16].GetComponent<DefaultTile>();

        switch (curIndex)
        {
            case 10:
                CardUIManager.Instance.StartSelectCard(false);
                break;

            case 11:
                CardUIManager.Instance.OnEndSelectCard();
                break;

            case 13:
                targetEquipTile = TileManager.Instance.equipTiles[0].GetComponent<EquipAndSpecialTile>();
                targetEquipTile.OnSelectEquipTile();
                break;

            case 14:
                targetDefaultTile.OnChangeDefaultTile();
                break;

            case 15:
                SunLineManager.Instance.RefreshRotValue(162f);
                break;

            case 18:
                targetDefaultTile.CurEffectTime = 5f;
                SunLineManager.Instance.RefreshRotValue(140f);
                StartWaitTime(130f);
                break;

            case 20:
                SunLineManager.Instance.RefreshRotValue(-20f);
                StartWaitTime(330f);
                break;

            case 21:
                IsExplaining = true;
                break;

            case 27:
                CardUIManager.Instance.Toggle_CardUI(false);
                break;

            case 31:
                IsExplaining = true;
                StartCoroutine(WaitForTileCrack(targetDefaultTile));
                break;

            case 32:
                IsExplaining = true;

                Button button = tutorialImageList[curIndex].GetComponentInChildren<Button>();
                button.onClick.AddListener(targetDefaultTile.OnResetCrack);
                break;

            case 33:
                IsExplaining = true;
                StartCoroutine(WaitForSecond());

                IEnumerator WaitForSecond()
                {
                    yield return new WaitForSecondsRealtime(1f);
                    IsExplaining = false;
                }

                break;

            case 34:
                targetDefaultTile.SetTileCrack();
                break;

            case 35:
                targetDefaultTile.DestroyTile();
                break;
            
            case 37:
                UIManager.Instance.OnShowSynergyEffectInfoPopup(1);
                break;
            case 38:
                UIManager.Instance.OnShowSynergyEffectInfoPopup(2);
                break;
            case 39:
                UIManager.Instance.OnShowSynergyEffectInfoPopup(3);
                break;
            case 40:
                UIManager.Instance.OnShowSynergyEffectInfoPopup(4);
                break;
            case 41:
                UIManager.Instance.OnShowSynergyEffectInfoPopup(5);
                break;

            case 42:
                UIManager.Instance.Toggle_SynergyEffectInfoPopup(false);
                break;

            case 46:
                CommonUIManager.instance.ToggleMenuUI(true);
                break;
        }
    }

    IEnumerator WaitForTileCrack(DefaultTile targetTile)
    {
        yield return new WaitForSecondsRealtime(1f);
        targetTile.SetTileCrack();
        yield return new WaitForSecondsRealtime(1f);
        IsExplaining = false;
    }

    void StartWaitTime(float value)
    {
        IsExplaining = true;
        GameManager.Instance.SpeedChangeValue = 1f; //Time.timeScale = 1;

        StartCoroutine(WaitForTargetRotValue(value));
    }

    public void EndWaitTime(bool isNextContents = false)
    {
        GameManager.Instance.SpeedChangeValue = 0f; //Time.timeScale = 0;
        IsExplaining = false;

        if (isNextContents)
        {
            RefreshNextContents();
        }
    }

    IEnumerator WaitForTargetRotValue(float value)
    {      
        while (SunLineManager.Instance.GetRotValue() > value)
            yield return null;

        //터치 실수를 방지하기 위한 시간
        yield return new WaitForSecondsRealtime(1f);

        EndWaitTime();
    }
}
