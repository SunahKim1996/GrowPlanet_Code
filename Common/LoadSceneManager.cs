using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadSceneManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] private Image loadingBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.1f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, op.progress, timer);
                if (loadingBar.fillAmount >= op.progress)
                {
                    timer = 0.1f;
                }
            }
            else
            {
                //timer 0으로 하면, 여기서 fillAmount 가 안채워지는 버그 생겨서 최소 0.1 로 설정함    
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timer);
                
                if (loadingBar.fillAmount >= 0.99f)
                {
                    op.allowSceneActivation = true;

                    if (nextScene == "Tutorial")
                    {
                        AsyncOperation op2 = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

                        while (!op2.isDone)
                        {
                            yield return null;
                        }

                        yield break;
                    }

                    yield break;
                }
            }
        }
    }
}
