using System;
using System.Collections.Generic;
using UnityEngine;

public class TileDataManager : Singleton<TileDataManager>
{
    public SheetData sheetData;
    public SheetTextData sheetTextData;

    //public Sprite[] tileSprites;
    public Sprite[] timeIconSprites;
    public Sprite[] cardRewardBGSprites;
    //public Sprite[] tileCardBGs;
    public Sprite[] tileCardRankFrames;

    //int : TileProperty / Sprtie : rank 순 그래픽
    public Dictionary<int, Sprite[]> tileSprites = new Dictionary<int, Sprite[]>();
    public Dictionary<int, Sprite[]> cardBGSprites = new Dictionary<int, Sprite[]>();

    public Dictionary<int, List<int>> tileIDs_ByType = new Dictionary<int, List<int>>();

    public int totalWeight = 0;

    /// <summary>
    /// 타일 속성
    /// </summary>
    public enum TileProperty
    {
        None = 0,

        Forest = 1,
        Field = 2,
        Ice = 3,
        Rock = 4,
        Sand = 5,

        Water = 6,
        Lava = 7,
    }

    /// <summary>
    /// 장착 타일 타입 개수
    /// </summary>
    [HideInInspector] public int equipTileTypeCount = 5;

    /// <summary>
    /// 타일 종류
    /// </summary>
    public enum TileState
    {
        DefaultTile,  //기본 타일
        AssignedTile, //기본 타일에 설치 및 특수 타일이 배치된 상태

        EquipTile,    //설치 타일
        SpecialTile,  //특수 타일
    }

    /// <summary>
    /// 시간 속성
    /// </summary>
    public enum TimeProperty
    {
        None = 0,
        Day = 1,
        Night = 2,
        DayAndNight = 3,
    }
    
    //----------------------------------------------------------------------------------
    void Start()
    {
        //tileSprites & cardBGSprites 구성
        foreach (var targetEnum in Enum.GetValues(typeof(TileProperty)))
        {
            if ((int)targetEnum == 0)
            {
                tileSprites[0] = Resources.LoadAll<Sprite>($"TileIcon/0_Default");
                cardBGSprites[0] = Resources.LoadAll<Sprite>($"CardBG/0_Default");
            }
            else
            {
                tileSprites[(int)targetEnum] = Resources.LoadAll<Sprite>($"TileIcon/{(int)targetEnum}_{targetEnum}");
                cardBGSprites[(int)targetEnum] = Resources.LoadAll<Sprite>($"CardBG/{(int)targetEnum}_{targetEnum}");
            }            
        }

        
        for (int tileID = 1; tileID < sheetData.DataList.Count; tileID++) 
        {
            //sheetData 의 DataList 를 typeID 별로 재분류
            int typeID = sheetData.DataList[tileID].TypeID;

            if (!tileIDs_ByType.ContainsKey(typeID)) 
            {
                tileIDs_ByType[typeID] = new List<int>();
            }

            tileIDs_ByType[typeID].Add(tileID);

            //가중치 합산
            int weight = sheetData.DataList[tileID].Rate;
            totalWeight += weight;
        }

        /*
        Debug.Log($"AllCount {tileIDs_ByType.Count}");
        foreach (var item in tileIDs_ByType) 
        { 
            for (int i = 0; i < item.Value.Count; i++)
            {
                Debug.Log($"typeID {item.Key} / tileID {item.Value[i]}");
            }            
        }
        */
    }
}
