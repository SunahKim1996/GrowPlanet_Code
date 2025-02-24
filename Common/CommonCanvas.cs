using UnityEngine;

public class CommonCanvas : MonoBehaviour
{
    //시작 Scene 으로 돌아오면 Canvas 가 중복 생성되어서 싱글톤으로 처리
    public static CommonCanvas instance;

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
}
