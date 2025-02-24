using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.BGM, "�κ�");
        CommonUIManager.instance.ToggleToMainButton(false);
        CommonUIManager.instance.ToggleMenuButton(true);
    }

    //HACK: DontDestroyOnLoad ó���� ��ũ��Ʈ�� �Լ��� ��ư �̺�Ʈ�� ������,
    //�� ��ȯ���� �� �̺�Ʈ�� �����Ǿ ������ ���� ó��
    public void OnToggleLeaderboardUI(bool state)
    {
        Leaderboard.instance.OnToggleLeaderboardUI(state);
    }

    public void OnStartTutorial()
    {
        AllManager.instance.isTutorial = true;
        AllManager.instance.LoadTutorial();
    }

    public void OnStartGame()
    {
        AllManager.instance.isTutorial = false;
        AllManager.instance.LoadGameScene();
    }
}
