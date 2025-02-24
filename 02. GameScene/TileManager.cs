using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TileDataManager;

public class TileManager : Singleton<TileManager>
{
    public bool isShowTileCoordinate; //Ÿ�Ͽ� ��ǥ ���� ǥ������ ����

    [SerializeField] private GameObject defaultTileOrigin;
    [SerializeField] private int radiusX, radiusY;
    [SerializeField] private float tileOffset;

    [SerializeField] private ScrollRect invenEquipTileList;
    //[SerializeField] private Scrollbar invenScrollbar;
    [SerializeField] private Transform originTilePos; 

    [SerializeField] private int specailTile_Count; //Ư�� Ÿ�� ��ġ ����

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
        /// �⺻ Ÿ�� ��ġ
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
        *///�����ڵ�
        /*
        // �߽��� �������� ������ ���·� Ÿ�� ��ġ
        for (int q = -Radius; q <= Radius; q++)
        {
            int r1 = Mathf.Max(-Radius, -q - Radius);
            int r2 = Mathf.Min(Radius, -q + Radius);

            for (int r = r1; r <= r2; r++)
            {
                // ��ǥ ��ȯ (ť�� ��ǥ�踦 2D ��ǥ��� ��ȯ)
                float xPos = (q + r / 2.0f) * hexWidth;
                float yPos = r * hexHeight;

                // ��������Ʈ ����
                Vector3 position = new Vector3(xPos, yPos, 0);
                GameObject newTile = Instantiate(defaultTileOrigin, OriginTile);
                newTile.transform.localPosition = position;
                newTile.name = $"{q},{r},{-(q + r)}";

                defaultTiles.Add(newTile);

                yield return new WaitForSeconds(0.5f);
            }
        }
        *///�����ڵ� 2

        //Sprite sprite = defaultTileOrigin.GetComponent<Image>().sprite;
        RectTransform a = defaultTileOrigin.GetComponent<RectTransform>();

        float hexWidth = tileOffset * (a.rect.width / 100); //1.732f; (sqrt(3)) // Ÿ�� ���� ����
        float hexHeight = tileOffset + (a.rect.height / 100); //1.5f;  // Ÿ�� ���� ����

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
                // ��ǥ ��ȯ (ť�� ��ǥ�踦 2D ��ǥ��� ��ȯ)
                float xPos = (q + r / 2.0f) * hexWidth;
                float yPos = r * hexHeight;

                // ��ǥ�� �߾��� ����
                xPos = xPos - (hexWidth / 2.0f) * Math.Abs(radiusX - radiusY);

                // ��������Ʈ ����
                Vector3 position = new Vector3(xPos, yPos, 0);

                GameObject newTile = Instantiate(defaultTileOrigin, originTilePos);
                newTile.transform.localPosition = position;
                newTile.name = $"{q},{r},{-(q + r)}";

                defaultTiles.Add(newTile);
            }
        }
    }

    /// <summary>
    /// Ư�� Ÿ�� ��ġ 
    /// </summary>
    public void GenerateSpecialTile()
    {
        //Ʃ�丮���� ���, ������ ��ġ�� ����
        if (AllManager.instance.isTutorial)
        {
            //�տ� int : �ڸ�, �ڿ� int : SpecailTile ���� 
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

        //Ʃ�丮���� �ƴϸ� ���� ���� 
        for(int i = 0; i < specailTile_Count; i++)
        {
            //��� ��ġ���� ����
            int randomInt = UnityEngine.Random.Range(0, defaultTiles.Count);
            DefaultTile targetDefaultTile = defaultTiles[randomInt].GetComponent<DefaultTile>();

            //� ���� ��ġ���� ����
            int typeID = UnityEngine.Random.Range(6, 8);

            Change_DefaultToOtherTile(targetDefaultTile.gameObject, TileState.SpecialTile, typeID);
        }
    }


    /// <summary>
    /// typeID �� �ش��ϴ� ��ġ Ÿ���� invenEquipTileList �ڽ����� ����
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

        //ũ�� ����
        tileClass.bgImage.SetNativeSize();
        tileClass.bgImage.GetComponent<RectTransform>().sizeDelta *= 0.9f;

        return newTile;
    }


    /// <summary>
    /// �κ��丮�� ��ġ Ÿ�� �߰�
    /// </summary>
    public void AddGetEquipTileList(int tileID, int typeID, int effectDayTypeID)
    {
        SheetData sheetData = TileDataManager.Instance.sheetData;

        //ġƮ�� ������ ���
        if (tileID == -1)
        {
            //Ÿ�Ե� ����
            if (typeID == -1)
            {
                tileID = UnityEngine.Random.Range(1, sheetData.DataList.Count);
                typeID = sheetData.DataList[tileID].TypeID;
                //effectDayTypeID = sheetData.DataList[tileID].EffectDayType;
                //typeID = UnityEngitileIDne.Random.Range(1, 6);
            }

            //������ Ÿ�� �߿��� ����
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
    /// ��ġ Ÿ���� ���õ� ���·� ����
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

            //�ó��� ȿ�� Value ����
            TileEffectManager.Instance.SetSynergyEffectValue(targetTile);
        }
        else
        {
            targetTile.gameObject.tag = "DefaultTile";
        }
    }

    /// <summary>
    /// �⺻ Ÿ���� ��ġ Ÿ�� or Ư�� Ÿ�Ϸ� ����
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
    /// Ÿ�� ��ġ ��, ��ó Ÿ�� List ����
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

    //TODO: �ڵ� ���� �ʿ�
    /// <summary>
    /// �ֺ� �� ĭ ���� �ִ� Ÿ������ ���� ��ȯ
    /// </summary>
    public bool IsNearByTile(DefaultTile targetTile, int q, int r, int target_q, int target_r)
    {   
        /*
        //�Ӽ��� ������ return
        if (targetTile.tileProperty == TileProperty.None)
        {
            return false;
        }
        */

        //q ������ ���� ������ return
        if (target_q != q && target_q != q + 1 && target_q != q - 1)
        {
            return false;
        }

        /*

        bool isSame = false;

        //��� �� �ߺ� ����
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

        //��ó Ÿ������ Ȯ��
        if (target_q == q && (target_r == r + 1 || target_r == r - 1) //q�� ���� �� �߿� r + 1 , r -1
            || (target_q == q + 1 && (target_r == r || target_r == r - 1)) //q�� + 1 �� �� �߿� r ������, r - 1
            || (target_q == q - 1 && (target_r == r || target_r == r + 1))) //q�� - 1 �� �� �߿� r ������, r + 1 �ΰ�
        {
            return true;
        }

        return false;        
    }

    /*
    /// <summary>
    /// ���� �Ӽ� Ÿ�� ���� ��������
    /// </summary>    
    public void CountNearBySamePropertyTile(int tileID, int q, int r, int typeID)
    {
        checkCount++;

        for (int i = 0; i < assignedTiles.Count; i++)
        {
            DefaultTile targetTile = assignedTiles[i].GetComponent<DefaultTile>();
            int target_q = targetTile.hexaCoordinate["q"];
            int target_r = targetTile.hexaCoordinate["r"];

            //�Ӽ��� ������ ������
            if (targetTile.tileProperty == TileProperty.None) 
            {
                continue;
            }

            //q ������ ���� ������ ������
            else if (target_q != q && target_q != q + 1 && target_q != q - 1)
            {
                continue;
            }

            bool isSame = false;

            //��� �� �ߺ� ����
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

            //��ó�� ���� �Ӽ� Ÿ���� �ִ��� Ȯ��
            if (target_q == q && (target_r == r + 1 || target_r == r - 1) //q�� ���� �� �߿� r + 1 , r -1
                || (target_q == q + 1 && (target_r == r || target_r == r - 1)) //q�� + 1 �� �� �߿� r ������, r - 1
                || (target_q == q - 1 && (target_r == r || target_r == r + 1))) //q�� - 1 �� �� �߿� r ������, r + 1 �ΰ�
            {                
                if (targetTile.tileProperty == (TileProperty)typeID)
                {
                    nearBySameTiles.Add(targetTile);

                    //nearByTileCount += 1;

                    //���� �Ӽ� Ÿ�� �ֺ��� �� �ִ��� Ȯ��
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
    /// ��� Ÿ�� �ʱ�ȭ
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
