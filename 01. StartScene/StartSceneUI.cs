using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.BGM, "로비");
        CommonUIManager.instance.ToggleToMainButton(false);
        CommonUIManager.instance.ToggleMenuButton(true);
    }

    //HACK: DontDestroyOnLoad 처리한 스크립트의 함수를 버튼 이벤트에 넣으면,
    //씬 전환했을 때 이벤트가 삭제되어서 다음과 같이 처리
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
