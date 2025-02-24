using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TileDataManager;

public class TileManager : Singleton<TileManager>
{
    public bool isShowTileCoordinate; //타일에 좌표 정보 표시할지 여부

    [SerializeField] private GameObject defaultTileOrigin;
    [SerializeField] private int radiusX, radiusY;
    [SerializeField] private float tileOffset;

    [SerializeField] private ScrollRect invenEquipTileList;
    //[SerializeField] private Scrollbar invenScrollbar;
    [SerializeField] private Transform originTilePos; 

    [SerializeField] private int specailTile_Count; //특수 타일 설치 개수

    [SerializeField] private GameObject equipTileOrigin;
    [SerializeField] private GameObject specialTileOrigin;

    [HideInInspector] public List<GameObject> defaultTiles = new List<GameObject>();
    [HideInInspector] public List<GameObject> equipTiles = new List<GameObject>(); 
    [HideInInspector] public List<GameObject> assignedTiles = new List<GameObject>();

    private int curEquipTileID = -1;
    private GameObject curEquipTileObject = null;

    /*
    private List<DefaultTile> nearBySameTiles = new List<DefaultTile>();
    private Dictionary<int, List<int>> preNearByTileData = new Dictionary<int, List<int>>();
    int checkCount = 0;
    */

    //----------------------------------------------------------------------------------
    /// <summary>
        /// 기본 타일 배치
        /// </summary>
    public void GenerateTile()
    {
        /*
        int xTileCount = TileSize;

        for (int y = 1; y < (TileSize * 2); y++)
        {
            int addValue = 0;

            if (y <= TileSize)
            {
                addValue = 1;
            }
            else
            {
                addValue = -1;
            }

            xTileCount += addValue;

            float defaultTileSizeX = DefaultTile.GetComponent<RectTransform>().sizeDelta.x;
            float defaultTileSizeY = DefaultTile.GetComponent<RectTransform>().sizeDelta.y;

            for (int x = 1; x < xTileCount; x++)
            {
                GameObject newTile = Instantiate(DefaultTile, originTile);

                //float newPosX = ((defaultTileSizeX + tileOffset) * x) + ((defaultTileSizeX + tileOffset) / 2 * -xTileCount);
                float newPosX = (defaultTileSizeX + tileOffset) * (x + (-xTileCount * 0.5f));
                float newPosY = (defaultTileSizeX + tileOffset - Math.Abs(defaultTileSizeY - defaultTileSizeX)) * y;
                Vector3 startPos = originTile.transform.Find("StartPos").transform.position;

                newTile.transform.position = new Vector3(newPosX + startPos.x, newPosY + startPos.y, 0);
            }
        }
        *///이전코드
        /*
        // 중심을 기준으로 육각형 형태로 타일 배치
        for (int q = -Radius; q <= Radius; q++)
        {
            int r1 = Mathf.Max(-Radius, -q - Radius);
            int r2 = Mathf.Min(Radius, -q + Radius);

            for (int r = r1; r <= r2; r++)
            {
                // 좌표 변환 (큐브 좌표계를 2D 좌표계로 변환)
                float xPos = (q + r / 2.0f) * hexWidth;
                float yPos = r * hexHeight;

                // 스프라이트 생성
                Vector3 position = new Vector3(xPos, yPos, 0);
                GameObject newTile = Instantiate(defaultTileOrigin, OriginTile);
                newTile.transform.localPosition = position;
                newTile.name = $"{q},{r},{-(q + r)}";

                defaultTiles.Add(newTile);

                yield return new WaitForSeconds(0.5f);
            }
        }
        *///이전코드 2

        //Sprite sprite = defaultTileOrigin.GetComponent<Image>().sprite;
        RectTransform a = defaultTileOrigin.GetComponent<RectTransform>();

        float hexWidth = tileOffset * (a.rect.width / 100); //1.732f; (sqrt(3)) // 타일 가로 간격
        float hexHeight = tileOffset + (a.rect.height / 100); //1.5f;  // 타일 세로 간격

        for (int q = -radiusX; q <= radiusY; q++)
        {
            int newRaidus = radiusX;

            if (q >= 1)
            {
                newRaidus = radiusY;
            }

            int r1 = Mathf.Max(-radiusX, -q - newRaidus);
            int r2 = Mathf.Min(radiusX, -q + newRaidus);

            for (int r = r1; r <= r2; r++)
            {
                // 좌표 변환 (큐브 좌표계를 2D 좌표계로 변환)
                float xPos = (q + r / 2.0f) * hexWidth;
                float yPos = r * hexHeight;

                // 좌표의 중앙을 맞춤
                xPos = xPos - (hexWidth / 2.0f) * Math.Abs(radiusX - radiusY);

                // 스프라이트 생성
                Vector3 position = new Vector3(xPos, yPos, 0);

                GameObject newTile = Instantiate(defaultTileOrigin, originTilePos);
                newTile.transform.localPosition = position;
                newTile.name = $"{q},{r},{-(q + r)}";

                defaultTiles.Add(newTile);
            }
        }
    }

    /// <summary>
    /// 특수 타일 배치 
    /// </summary>
    public void GenerateSpecialTile()
    {
        //튜토리얼인 경우, 지정된 위치에 생성
        if (AllManager.instance.isTutorial)
        {
            //앞에 int : 자리, 뒤에 int : SpecailTile 종류 
            Dictionary<int, int> asdf = new Dictionary<int, int>();
            asdf.Add(5, 6);
            asdf.Add(13, 7);
            asdf.Add(20, 6);
            asdf.Add(15, 7);
            asdf.Add(29, 6);

            foreach(KeyValuePair<int, int> item in asdf)
            {
                DefaultTile targetDefaultTile = defaultTiles[item.Key].GetComponent<DefaultTile>();
                int typeID = item.Value;

                Change_DefaultToOtherTile(targetDefaultTile.gameObject, TileState.SpecialTile, typeID);
            }

            return;
        }

        //튜토리얼이 아니면 랜덤 지정 
        for(int i = 0; i < specailTile_Count; i++)
        {
            //어디에 설치할지 결정
            int randomInt = UnityEngine.Random.Range(0, defaultTiles.Count);
            DefaultTile targetDefaultTile = defaultTiles[randomInt].GetComponent<DefaultTile>();

            //어떤 종류 설치할지 결정
            int typeID = UnityEngine.Random.Range(6, 8);

            Change_DefaultToOtherTile(targetDefaultTile.gameObject, TileState.SpecialTile, typeID);
        }
    }


    /// <summary>
    /// typeID 에 해당하는 설치 타일을 invenEquipTileList 자식으로 생성
    /// </summary>
    /// <param name="typeID"></param>
    public GameObject CreateEqiupTile(int tileID, int typeID, int effectDayTypeID)
    {       
        GameObject newTile = Instantiate(equipTileOrigin, invenEquipTileList.content);
        //invenScrollbar.value = 0f;

        EquipAndSpecialTile tileClass = newTile.GetComponent<EquipAndSpecialTile>();

        tileClass.tileProperty = (TileProperty)typeID;
        //newTile.GetComponent<Image>().sprite = TileDataManager.Instance.tileSprites[typeID];

        tileClass.timeProperty = (TimeProperty)effectDayTypeID;
        tileClass.timeIcon.sprite = TileDataManager.Instance.timeIconSprites[effectDayTypeID];

        tileClass.tileID = tileID;

        int rank = TileDataManager.Instance.sheetData.DataList[tileID].Rank - 1;
        tileClass.bgImage.sprite = TileDataManager.Instance.tileSprites[typeID][rank];

        //크기 조정
        tileClass.bgImage.SetNativeSize();
        tileClass.bgImage.GetComponent<RectTransform>().sizeDelta *= 0.9f;

        return newTile;
    }


    /// <summary>
    /// 인벤토리에 설치 타일 추가
    /// </summary>
    public void AddGetEquipTileList(int tileID, int typeID, int effectDayTypeID)
    {
        SheetData sheetData = TileDataManager.Instance.sheetData;

        //치트로 접근한 경우
        if (tileID == -1)
        {
            //타입도 랜덤
            if (typeID == -1)
            {
                tileID = UnityEngine.Random.Range(1, sheetData.DataList.Count);
                typeID = sheetData.DataList[tileID].TypeID;
                //effectDayTypeID = sheetData.DataList[tileID].EffectDayType;
                //typeID = UnityEngitileIDne.Random.Range(1, 6);
            }

            //정해진 타입 중에서 랜덤
            else 
            {
                int randomIndex = UnityEngine.Random.Range(0, TileDataManager.Instance.tileIDs_ByType[typeID].Count);
                tileID = TileDataManager.Instance.tileIDs_ByType[typeID][randomIndex];
            }

            effectDayTypeID = sheetData.DataList[tileID].EffectDayType;
        }

        GameObject newTile = CreateEqiupTile(tileID, typeID, effectDayTypeID);
        equipTiles.Add(newTile);
    }

    public void DeleteEquipTile()
    {
        if (AllManager.instance.isTutorial)
        {
            GameObject targetTile = equipTiles[0];
            equipTiles.RemoveAt(0);
            Destroy(targetTile);
            return;
        }

        for (int i = 0; i < equipTiles.Count; i++) 
        {
            if(equipTiles[i] == CurEquipTileObject)
            {
                equipTiles.RemoveAt(i);
                break;
            }
        }

        Destroy(CurEquipTileObject);
    }


    private void ToggleEquiptTileColor(GameObject targetTile, bool state)
    {
        EquipAndSpecialTile tileClass = targetTile.GetComponent<EquipAndSpecialTile>();

        float alpha = state ? 1f : 0.1f;

        tileClass.bgImage.color = new Color(tileClass.bgImage.color.r, tileClass.bgImage.color.g, tileClass.bgImage.color.b, alpha);
        tileClass.timeBG.color = new Color(tileClass.timeBG.color.r, tileClass.timeBG.color.g, tileClass.timeBG.color.b, alpha);
        tileClass.timeIcon.color = new Color(tileClass.timeIcon.color.r, tileClass.timeIcon.color.g, tileClass.timeIcon.color.b, alpha);
    }

    /// <summary>
    /// 설치 타일을 선택된 상태로 변경
    /// </summary>
    private void ChangeEqiupTileSelected()
    {
        for (int i = 0; i < equipTiles.Count; i++) 
        {
            if (CurEquipTileObject == null || equipTiles[i] == CurEquipTileObject)
            {
                ToggleEquiptTileColor(equipTiles[i], true);
            }
            else if (equipTiles[i] != CurEquipTileObject)
            {
                ToggleEquiptTileColor(equipTiles[i], false);
            }
        }
    }

    public void ToggleCanSelectTileSign(bool state)
    {
        for(int i = 0; i < defaultTiles.Count; i++)
        {
            DefaultTile targetTile = defaultTiles[i].GetComponent<DefaultTile>();

            if (state)
            {    
                if (targetTile.tileProperty != TileProperty.Lava && targetTile.tileProperty != TileProperty.Water)
                {
                    targetTile.stroke.gameObject.SetActive(true);
                }
            }
            else
            {
                targetTile.stroke.gameObject.SetActive(false);
            }
        }
    }

    public void ChangeTileProperty(DefaultTile targetTile, int typeID, int dayTypeID, int tileID)
    {
        targetTile.tileProperty = (TileProperty)typeID;
        //targetTile.GetComponent<Image>().sprite = TileDataManager.Instance.tileSprites[typeID];

        int rank = TileDataManager.Instance.sheetData.DataList[tileID].Rank - 1;

        if (typeID == 0)
        {
            rank = 0;
        }

        targetTile.GetComponent<DefaultTile>().bgImage.sprite = TileDataManager.Instance.tileSprites[typeID][rank];
        targetTile.GetComponent<DefaultTile>().bgImage.SetNativeSize();
        targetTile.timeProperty = (TimeProperty)dayTypeID;

        if (dayTypeID != 0)
        { 
            targetTile.timeIcon.sprite = TileDataManager.Instance.timeIconSprites[dayTypeID];
            targetTile.timeBG.gameObject.SetActive(true);
        }
        else
        {
            targetTile.timeBG.gameObject.SetActive(false);
        }

        targetTile.tileID = tileID;

        if (tileID != 0)
        {
            targetTile.gameObject.tag = "EquipTile";

            //시너지 효과 Value 적용
            TileEffectManager.Instance.SetSynergyEffectValue(targetTile);
        }
        else
        {
            targetTile.gameObject.tag = "DefaultTile";
        }
    }

    /// <summary>
    /// 기본 타일을 설치 타일 or 특수 타일로 변경
    /// </summary>
    /// <param name="targetTile"></param>
    /// <param name=""></param>
    public void Change_DefaultToOtherTile(GameObject defaultTile, TileState tileState, int typeID = -1)
    {
        if (tileState == TileState.EquipTile)
        {
            typeID = TileDataManager.Instance.sheetData.DataList[CurEquipTileID].TypeID;
            int dayTypeID = TileDataManager.Instance.sheetData.DataList[CurEquipTileID].EffectDayType;

            //defaultTile.GetComponent<Image>().sprite = TileSprites[typeID];
            defaultTile.tag = "EquipTile";

            DefaultTile targetTile = defaultTile.GetComponent<DefaultTile>();
            //targetTile.tileID = CurEquipTileID;
            //targetTile.tileProperty = (TileProperty)typeID;
            //targetTile.timeProperty = (TimeProperty)dayTypeID;

            //targetTile.timeIcon.sprite = TimeIconSprites[dayTypeID];

            ChangeTileProperty(targetTile, typeID, dayTypeID, CurEquipTileID);
        }
        else if (tileState == TileState.SpecialTile)
        {
            //int randomInt = UnityEngine.Random.Range(0, specialTileOrigin.Length);
            //defaultTile.GetComponent<Image>().sprite = TileDataManager.Instance.tileSprites[typeID];
            defaultTile.tag = "SpecialTile";

            DefaultTile targetTile = defaultTile.GetComponent<DefaultTile>();
            targetTile.bgImage.sprite = TileDataManager.Instance.tileSprites[typeID][0];
            targetTile.bgImage.SetNativeSize();

            targetTile.tileID = -1;
            targetTile.tileProperty = (TileProperty)typeID;
            targetTile.timeProperty = TimeProperty.None;
        }
        
        assignedTiles.Add(defaultTile);
    }

    public void InitSelectEquipTile()
    {
        CurEquipTileID = -1;
        CurEquipTileObject = null;

        ToggleCanSelectTileSign(false);
        CardUIManager.Instance.Toggle_CardUI(false);
    }
     
    /*
    public void InitNearTileData()
    {
        nearBySameTiles.Clear();
        preNearByTileData.Clear();
    }
    */

    /// <summary>
    /// 타일 배치 시, 근처 타일 List 세팅
    /// </summary>
    public List<DefaultTile> GetNearByTiles(int q, int r)
    {
        List<DefaultTile> nearByTiles = new List<DefaultTile>();

        for (int i = 0; i < defaultTiles.Count; i++)
        {
            DefaultTile targetTile = defaultTiles[i].GetComponent<DefaultTile>();

            int target_q = targetTile.hexaCoordinate["q"];
            int target_r = targetTile.hexaCoordinate["r"];

            if (targetTile.tileProperty == TileProperty.Sand)
            {
                continue;
            }

            bool isNearByTile = IsNearByTile(targetTile, q, r, target_q, target_r);

            if (isNearByTile)
            {
                nearByTiles.Add(targetTile);
            }
        }

        return nearByTiles;
    }

    //TODO: 코드 정리 필요
    /// <summary>
    /// 주변 한 칸 내에 있는 타일인지 여부 반환
    /// </summary>
    public bool IsNearByTile(DefaultTile targetTile, int q, int r, int target_q, int target_r)
    {   
        /*
        //속성이 없으면 return
        if (targetTile.tileProperty == TileProperty.None)
        {
            return false;
        }
        */

        //q 조건이 맞지 않으면 return
        if (target_q != q && target_q != q + 1 && target_q != q - 1)
        {
            return false;
        }

        /*

        bool isSame = false;

        //재귀 시 중복 제거
        if (preNearByTileData.ContainsKey(target_q))
        {
            for (int j = 0; j < preNearByTileData[target_q].Count; j++)
            {
                int pre_r = preNearByTileData[target_q][j];

                if (pre_r == target_r)
                {
                    isSame = true;
                    break;
                }
            }

            if (isSame)
            {
                return false;
            }
        }
        */

        //근처 타일인지 확인
        if (target_q == q && (target_r == r + 1 || target_r == r - 1) //q가 같은 거 중에 r + 1 , r -1
            || (target_q == q + 1 && (target_r == r || target_r == r - 1)) //q가 + 1 인 것 중에 r 같은거, r - 1
            || (target_q == q - 1 && (target_r == r || target_r == r + 1))) //q가 - 1 인 것 중에 r 같은거, r + 1 인거
        {
            return true;
        }

        return false;        
    }

    /*
    /// <summary>
    /// 같은 속성 타일 개수 가져오기
    /// </summary>    
    public void CountNearBySamePropertyTile(int tileID, int q, int r, int typeID)
    {
        checkCount++;

        for (int i = 0; i < assignedTiles.Count; i++)
        {
            DefaultTile targetTile = assignedTiles[i].GetComponent<DefaultTile>();
            int target_q = targetTile.hexaCoordinate["q"];
            int target_r = targetTile.hexaCoordinate["r"];

            //속성이 없으면 지나감
            if (targetTile.tileProperty == TileProperty.None) 
            {
                continue;
            }

            //q 조건이 맞지 않으면 지나감
            else if (target_q != q && target_q != q + 1 && target_q != q - 1)
            {
                continue;
            }

            bool isSame = false;

            //재귀 시 중복 제거
            if (preNearByTileData.ContainsKey(target_q))
            {
                for (int j = 0; j < preNearByTileData[target_q].Count; j++)
                {
                    int pre_r = preNearByTileData[target_q][j];

                    if (pre_r == target_r)
                    {
                        isSame = true;                        
                        break;
                    }
                }

                if (isSame)
                {                    
                    continue;
                }
            }

            //근처에 같은 속성 타일이 있는지 확인
            if (target_q == q && (target_r == r + 1 || target_r == r - 1) //q가 같은 거 중에 r + 1 , r -1
                || (target_q == q + 1 && (target_r == r || target_r == r - 1)) //q가 + 1 인 것 중에 r 같은거, r - 1
                || (target_q == q - 1 && (target_r == r || target_r == r + 1))) //q가 - 1 인 것 중에 r 같은거, r + 1 인거
            {                
                if (targetTile.tileProperty == (TileProperty)typeID)
                {
                    nearBySameTiles.Add(targetTile);

                    //nearByTileCount += 1;

                    //같은 속성 타일 주변에 또 있는지 확인
                    if (preNearByTileData.ContainsKey(target_q) == false)
                    {
                        preNearByTileData[target_q] = new List<int>();
                    }

                    preNearByTileData[target_q].Add(target_r);

                    CountNearBySamePropertyTile(-1, target_q, target_r, (int)targetTile.tileProperty);
                }
            }
        }

        checkCount--;

        if (checkCount == 0 && tileID != -1)
        {
            Debug.Log($"nearByTileCount {nearBySameTiles.Count}");
            TileEffectManager.Instance.CheckSynergyEffect(tileID, nearBySameTiles);
        }
    }
    */

    /// <summary>
    /// 모든 타일 초기화
    /// </summary>
    public void Init_TileList()
    {
        for (int i = assignedTiles.Count - 1; i >= 0; i--)
        {
            assignedTiles[i].GetComponent<DefaultTile>().InitDefaultTile();
        }

        EquipAndSpecialTile[] invenTiles = invenEquipTileList.GetComponentsInChildren<EquipAndSpecialTile>();
        for (int j = invenTiles.Length - 1; j >= 0; j--)
        {
            Destroy(invenTiles[j].gameObject);
        }

        equipTiles.Clear();
        assignedTiles.Clear();
    }

    //----------------------------------------------------------------------------------
    public int CurEquipTileID
    {
        get
        {
            return curEquipTileID;
        }
        set
        {
            curEquipTileID = value;
        }
    }

    public GameObject CurEquipTileObject
    {
        get
        {
            return curEquipTileObject;
        }
        set
        {
            curEquipTileObject = value;
            ChangeEqiupTileSelected();
        }
    }
}
