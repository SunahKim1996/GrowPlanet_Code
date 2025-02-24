using UnityEngine;

public class CommonCanvas : MonoBehaviour
{
    //���� Scene ���� ���ƿ��� Canvas �� �ߺ� �����Ǿ �̱������� ó��
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
