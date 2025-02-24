using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tutorial : Singleton<Tutorial>
{
    string[] contentsList =
    {
        "����� �༺�� Ű�� ��ŷ�\n�Ǹ��ϴ� �ǸŻ��Դϴ�.",
        "Ȳ���� �༺�� �Ƹ��ٿ�\n�ڿ� ����� ���弼��.",
        "���� ��ű��� ���� �ð��Դϴ�.",

        "���� ����ڰ� �����ϰԲ�\n�༺�� ��ġ�� ���̸� �˴ϴ�.",
        "���ѽð� �ȿ�",
        "�༺�� ���� ��ġ�� \"���� ����\"��",
        "\"��ǥ ����\"���� �����ּ���.",

        "�׷��� ���� ��ġ�� ���� ���ô�.",
        "������ ���۵Ǹ� �ڿ� ī�带\n�ϳ� ������ �� �ֽ��ϴ�.",
        "����/�ϸ����� �ѹ����� �� ������\n�ڿ� ī�� ȹ�� ��ȸ�� ����ϴ�.",

        "���� ī�带 �����ϼ���.",
        "������ ī���\n���� ����ҿ� �߰��˴ϴ�.",
        "����� Ÿ���� ��ġ�غ�����.",
        "�� �ٴڿ� Ÿ���� ��ġ�غ�����.",

        "Ÿ�� ��ġ �� ������ ��� �����\n2���� �Դϴ�.",
        "ù��°��\nŸ�� ��/�㿡 ���� �����Դϴ�.",
        "��� ��ġ�� Ÿ���� ���� �����ϴ� Ÿ���̰�\n���� ���� ��ġ�� ������\n��� �ڿ� ������ ������ �� �ְڱ���.",
        "�ش� �ð��뿡 �󸶳� �־�������\nŸ�̸Ӹ� ���� �� �� �ֽ��ϴ�.",
        "Ÿ�̸Ӱ� ä������ ���� ���� �ȴٸ�\nī��Ʈ�� �ʱ�ȭ �˴ϴ�.",
        "������ ��� 2��° ����Դϴ�.",
        "Ÿ���� ����/�ϸ����� �浹�ϸ�\n���ⷮ��ŭ ������ ȹ���մϴ�.",

        "��ġ�� Ÿ���� <color=red>���</color> ��������.",
        "Ÿ�Ͽ� ���� �ڼ��� ������ �����մϴ�.",
        "�ش� �κ��� Ÿ���� �����ϴ�\n������ ������ ǥ���մϴ�.",
        "�ش� �κ���\n� �ð��뿡 �߰����� ������\n������ Ÿ������ ǥ���ϰ� �ֽ��ϴ�.",
        "Ÿ���� ���� ���ʽ� �Դϴ�.",
        "������ Ÿ���� ������ ����\n���ⷮ�� ��ȭ�˴ϴ�.",

        "�ٵ�...",
        "���� Ÿ���� ���� ��� Ÿ���� ������,\n���ⷮ�� �پ��ϴ�.",
        "���� �� Ÿ�� ���� �ξ��ٸ�,\n���� ���ʽ��� �߻��Ͽ�\n�߰� ���ⷮ�� ����� ���Դϴ�.",

        "�ܼ��� Ÿ���� ��ġ�ϴ� ����\n���� �ƴմϴ�.",
        "Ÿ���� ���� �ð� �� ��ȭ�˴ϴ�.",
        "Ÿ���� 1�� ��ġ�ϼ���.",
        "�׷� ��ȭ�� ������ϴ�.",
        "���� ���� �ð� ��ġ�� ���� �ʴ´ٸ�",
        "Ÿ���� �ı��˴ϴ�.",

        "�ڿ� ��� Ÿ����\n��ġ ������ ���� �ó����� �߻��մϴ�.",
        "�� Ÿ����\n\"��� Ÿ���� ���� ���ʽ� ����\"",
        "���� Ÿ����\n\"�߰� Ÿ�� ȹ��\"",
        "���� Ÿ����\n\"���� Ÿ�ϸ� ���ⷮ�� ũ�� ���\"",
        "���� Ÿ����\n\"��� Ÿ���� �⺻ ���ⷮ ����\"",

        "�� Ÿ����\n\"�ֺ��� �𷡷� ����\"�ϴ�\n�ó����� �߻��մϴ�.",

        "���� ���� �ӵ��� ������ �ϰ� �ʹٸ�",
        "��� ��ư�� Ŭ�����ּ���.",
        "Ʃ�丮���� ��������Դϴ�.",
        "�޴��� ���� ���� ȭ������ ���ư��ϴ�.",
        "�ش� ��ư�� ������\n���� ȭ������ ���ư� �� �ֽ��ϴ�."
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
        CommonUIManager.instance.ShowPopupUI("Ʃ�丮����\n�ߴ��ұ��?", "��", "�ƴϿ�",
            () =>
            {
                ExitTutorial();
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
            },
            () =>
            {
                SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");
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

        //Ʃ�丮�� �̹��� ����
        if (curIndex > 0)
        {
            tutorialImageList[curIndex - 1].SetActive(false);
        }
        tutorialImageList[curIndex].SetActive(true);

        //Ʃ�丮�� �ؽ�Ʈ ����
        TMP_Text contents = tutorialImageList[curIndex].GetComponentInChildren<TMP_Text>();
        contents.text = contentsList[curIndex];
        LayoutRebuilder.ForceRebuildLayoutImmediate(contents.rectTransform);

        //�� �ε����� Ư�� ��Ȳ ó�� 
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

        //��ġ �Ǽ��� �����ϱ� ���� �ð�
        yield return new WaitForSecondsRealtime(1f);

        EndWaitTime();
    }
}
