using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum SheetType
{
    RewardData,
    MessageData,
}

public class GoogleSheetReader : MonoBehaviour
{
    public static GoogleSheetReader instance;

    //SheetType, 각 라운드 Value List
    public Dictionary<SheetType, List<string>> sheetDataList = new Dictionary<SheetType, List<string>>();
    private int loadCount = 0;

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
        Init();
    }

    public void Init()
    {
        //"(내 sheet 경로 edit 전까지)/export?format=tsv&gid=(탭 gid ID - 링크 공유 누르면 맨 끝에 나오는 숫자))";
        //여기 링크 참고 : https://no-name-stroy.tistory.com/5

        const string url1 = "https://docs.google.com/spreadsheets/d/135Y_YeDmasxegX4ci7aSCNOs_3Qy5ZXkpJJXk7NK0q4/export?format=tsv&gid=290571534";
        sheetDataList.Add(SheetType.RewardData, new List<string>());
        sheetDataList[SheetType.RewardData].Add("-1"); // 0라운드는 사용 안하므로, 미리 다른 값으로 채워놓음

        StartCoroutine(LoadData(SheetType.RewardData, url1, 2));

        //-----------------------------------------------------
        const string url2 = "https://docs.google.com/spreadsheets/d/135Y_YeDmasxegX4ci7aSCNOs_3Qy5ZXkpJJXk7NK0q4/export?format=tsv&gid=476820087";
        sheetDataList.Add(SheetType.MessageData, new List<string>());

        StartCoroutine(LoadData(SheetType.MessageData, url2, 1));

        //-----------------------------------------------------
        StartCoroutine(CheckLoadEnd());
    }

    IEnumerator LoadData(SheetType sheetType, string url, int targetIndex)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                string data = www.downloadHandler.text;
                string[] rows = data.Split('\n');

                //0번째는 타이틀이므로 1부터 시작
                for (int i = 1; i < rows.Length; i++)
                {
                    string[] columns = rows[i].Split('\t');
                    string value = columns[targetIndex];
                    value = value.Replace("\r", "");

                    sheetDataList[sheetType].Add(value);
                }
            }
        }

        /*
        for (int i = 0; i < sheetDataList[sheetType].Count; i++)
        {
            Debug.Log($"ID {i} / 값 {sheetDataList[sheetType][i]}");
        }
        */

        loadCount++;
    }

    IEnumerator CheckLoadEnd()
    {
        while (loadCount < 2)
        {
            yield return null;
        }

        AllManager.instance.CheckEndLoading(LoadingData.GoogleSheet);
    }

    /*
    IEnumerator LoadData()
    {
        foreach (KeyValuePair<SheetType, Data> item in dataList)
        {
            //string url = item.Value;
            string url = item.Value.url;
            List<int> loadIndexes = item.Value.loadIndexes;
            SheetType sheetType = item.Key;

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.isDone)
                {
                    string data = www.downloadHandler.text;
                    string[] rows = data.Split('\n');

                    //0번째는 타이틀이므로 1부터 시작
                    for (int i = 1; i < rows.Length; i++)
                    {
                        string[] columns = rows[i].Split('\t');
                        string value = columns[2];

                        sheetDataList[sheetType].Add(value);
                    }
                }
            }

            for (int i = 0; i < sheetDataList[sheetType].Count; i++)
            {
                Debug.Log($"ID {i} / 값 {sheetDataList[sheetType][i]}");
            }
        }

        AllManager.instance.CheckEndLoading(LoadingData.GoogleSheet);
    }
    */
}
