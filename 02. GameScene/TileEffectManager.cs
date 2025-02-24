using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static TileDataManager;

public class TileEffectManager : Singleton<TileEffectManager>
{
    private int addTilePeriod = 0; //SynergyEffect 4~6
    [HideInInspector] public int curTilePeriod = 0;
    public Dictionary<int, TypeSynergyEffectInfo> curSynergyEffectData_ByType = new Dictionary<int, TypeSynergyEffectInfo>();
     
    public struct TypeSynergyEffectInfo
    { 
        public int count;
        public GameObject slot;
        public int curSynergyNumID;
    }

    private TileManager tileManagerInst;
    private SheetData sheetData;
    private SheetTextData sheetTextData;

    public struct SynergyEffectValue
    {
        public float addReward_Value; //SynergyEffect 1~3
        public float addReward_Ice_Value; //SynergyEffect 7~9
        public int reward_MultifyValue; //SynergyEffect 10~11
        public float changeSendValue; //SynergyEffect 12~15
    }

    private float synergyEffect_AdditionalRewardType_AddValue = 0; //SynergyEffect 1~3 
    private float synergyEffect_Ice_AdditionalRewardType_AddValue = 0; //SynergyEffect 7~9
    private float synergyEffect_Ice_AdditionalRewardType_DelValue = 0; //SynergyEffect 7~9
    private int synergyEffect_Reward_MultifyValue = 1; //SynergyEffect 10~11
    private float synergyEffect_ChangeSendValue = 0; //SynergyEffect 12~15

    //----------------------------------------------------------------------------------
    void Start()
    { 
        tileManagerInst = TileManager.Instance;
        sheetData = TileDataManager.Instance.sheetData;
        sheetTextData = TileDataManager.Instance.sheetTextData;

        GenerateSynergyEffectData();
    }

    //----------------------------------------------------------------------------------
    private void GenerateSynergyEffectData()
    {
        for (int typeID = 1; typeID <= TileDataManager.Instance.equipTileTypeCount; typeID++)
        {
            TypeSynergyEffectInfo data = new TypeSynergyEffectInfo();
            data.count = 0;
            data.curSynergyNumID = -1;
            data.slot = UIManager.Instance.GenerateSynergyEffectUI(typeID, data.count, data.curSynergyNumID);

            curSynergyEffectData_ByType[typeID] = data;
        }
    }

    private int RefreshSynergyEffectCount(int tileID, int typeID, int value)
    {
        //int typeID = sheetData.DataList[tileID].TypeID;

        TypeSynergyEffectInfo data = curSynergyEffectData_ByType[typeID];
        data.count += value;
        curSynergyEffectData_ByType[typeID] = data;

        UIManager.Instance.RefreshSynergyEffectUI_Count(curSynergyEffectData_ByType[typeID].slot, curSynergyEffectData_ByType[typeID].count);

        return data.count;
    }

    public void RefreshCurSynergyNumID(int typeID, int id)
    {
        TypeSynergyEffectInfo data = curSynergyEffectData_ByType[typeID];
        data.curSynergyNumID = id;
        curSynergyEffectData_ByType[typeID] = data;
    }   


    //TODO: ���� ����
    /// <summary>
    /// ���� targetTile �� ȿ���� üũ  
    /// </summary>
    public void CheckEffect(int targetTileID, int q, int r, int typeID, DefaultTile targetTile, int value)
    {
        //�ó��� ȿ�� (���� �Ӽ��� � ��ġ�Ǿ� �ִ���)
        //tileManagerInst.InitNearTileData();
        //tileManagerInst.CountNearBySamePropertyTile(targetTileID, q, r, typeID);

        //�ó��� ȿ�� (���� �Ӽ��� � ��ġ�Ǿ� �ִ���)
        int curCount = RefreshSynergyEffectCount(targetTileID, typeID, value);
        CheckSynergyEffect(targetTileID, curCount);

        //���� ȿ�� (Additional, Reduce �� ����)
        CheckAll_NearByEffectValue(targetTileID, q, r, targetTile, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param> Ÿ�� �߰�(1)/����(-1) ����
    public void CheckAllEffect(DefaultTile originTile, int targetTileID, int q, int r, int typeID, int value)
    {
        CheckEffect(targetTileID, q, r, typeID, originTile, value);

        //��ó Ÿ���� ���� ȿ�� (Additional, Reduce �� ����) ���� üũ
        List<DefaultTile> nearByTiles = tileManagerInst.GetNearByTiles(q, r);

        for (int i = 0; i < nearByTiles.Count; i++)
        {
            DefaultTile targetTile = nearByTiles[i];

            if (!targetTile.CompareTag("EquipTile"))
            {
                continue;
            }

            //��ó Ÿ���� AdditionalReward �� ���� Ÿ���� ������ �شٸ�, 
            Set_NearByEffectValue(targetTile, originTile, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="originTile"></param> ���� Ÿ��
    /// <param name="targetTile"></param> originTile ���� ���� ���� Ÿ��
    private void Set_NearByEffectValue(DefaultTile originTile, DefaultTile targetTile, int value)
    {
        int tileID = originTile.tileID;
        List<int> addRewardTypeIDs = sheetData.DataList[tileID].AdditionalRewardTypes;
        int reducedRewardTypeID = sheetData.DataList[tileID].ReducedRewardType;

        //���ⷮ �߰� ó��
        for (int i = 0; i < addRewardTypeIDs.Count; i++)
        {
            int typeID = addRewardTypeIDs[i];

            if (typeID == -1)
            {
                continue;
            }
            else if (targetTile.tileProperty == (TileProperty)typeID)
            {
                originTile.CurAdditionalRewardCount += value;
            }
        }

        //���ⷮ ���� ó��
        if (reducedRewardTypeID == -1)
        {
            return;
        }
        else if (targetTile.tileProperty == (TileProperty)reducedRewardTypeID)
        {
            originTile.CurReducedRewardCount += value;
        }
    }

    private void CheckAll_NearByEffectValue(int tileID, int q, int r, DefaultTile originTile, int value)
    {
        for (int i = 0; i < tileManagerInst.assignedTiles.Count; i++)
        {
            DefaultTile targetTile = tileManagerInst.assignedTiles[i].GetComponent<DefaultTile>();

            int target_q = targetTile.hexaCoordinate["q"];
            int target_r = targetTile.hexaCoordinate["r"];

            bool isNearByTile = tileManagerInst.IsNearByTile(targetTile, q, r, target_q, target_r);

            if (isNearByTile == false)
            {
                continue;
            }

            Set_NearByEffectValue(originTile, targetTile, value);

            /*
            //���ⷮ �߰� ó��
            for (int j = 0; j < additionalRewardTypesIDs.Count; j++)
            {
                int typeID = additionalRewardTypesIDs[j];

                if (typeID == -1)
                {
                    continue;
                }
                else if (targetTile.tileProperty == (TileProperty)typeID)
                {
                    orginTile.CurAdditionalRewardCount += 1;
                }
            }

            //���ⷮ ���� ó��
            if (reducedRewardTypeID == -1)
            {
                continue;
            }
            else if (targetTile.tileProperty == (TileProperty)reducedRewardTypeID)
            {
                orginTile.CurReducedRewardCount += 1;
            }
            */

        }
    }

    public void SetSynergyEffectValue(DefaultTile targetTile)
    {
        //SynergyEffect 1~3
        targetTile.synergyEffectValue.addReward_Value = SynergyEffect_AdditionalRewardType_AddValue;

        //SynergyEffect 7~9
        if (targetTile.tileProperty == TileProperty.Ice)
        {
            float addValue = SynergyEffect_Ice_AdditionalRewardType_AddValue;
            targetTile.synergyEffectValue.addReward_Ice_Value = addValue;
        }
        else
        {
            float delValue = SynergyEffect_Ice_AdditionalRewardType_DelValue;
            targetTile.synergyEffectValue.addReward_Ice_Value = -delValue;
        }

        //SynergyEffect 10~11
        int multifyValue = SynergyEffect_Reward_MultifyValue;
        targetTile.synergyEffectValue.reward_MultifyValue = multifyValue;

        //SynergyEffect 12~15
        if (targetTile.tileProperty == TileProperty.Sand)
        {
            float changeValue = SynergyEffect_ChangeSendValue;
            targetTile.synergyEffectValue.changeSendValue = changeValue;
        }
    }

    /// <summary>
    /// ����� �ó��� ȿ�� Ȯ��
    /// </summary>
    /// <param name="tileID"></param>
    private void CheckSynergyEffect(int tileID, int sameTypeTileCount)
    {
        List<int> SynergyNums = sheetData.DataList[tileID].SynergyNums;
        int typeID = sheetData.DataList[tileID].TypeID;        
        int effectID = -1;

        for (int i = SynergyNums.Count - 1; i >= 0; i--)
        {
            int targetCount = SynergyNums[i];
            
            if (sameTypeTileCount >= targetCount)
            {
                if (curSynergyEffectData_ByType[typeID].curSynergyNumID == i)
                {
                    return;
                }

                List<int> synergyEffects = sheetData.DataList[tileID].SynergyEffects;
                effectID = synergyEffects[i];

                break;
            }
        }
                
        UIManager.Instance.RefreshSynergyEffectUI_CurSynergyInfo(tileID, sameTypeTileCount);
        SetSynergyEffect(effectID);
    }


    private void Add_AllTileRewardPercent(int addValue)
    {
        for (int i = 0; i < tileManagerInst.assignedTiles.Count; i++)
        {
            DefaultTile targetTile = tileManagerInst.assignedTiles[i].GetComponent<DefaultTile>();

            targetTile.synergyEffectValue.addReward_Value = addValue;
        }
    }

    private void Change_IceRewardPercent(int removeValue, int addValue)
    {
        for (int i = 0; i < tileManagerInst.assignedTiles.Count; i++)
        {
            DefaultTile targetTile = tileManagerInst.assignedTiles[i].GetComponent<DefaultTile>();

            if (targetTile.tileProperty == TileProperty.Ice)
            {
                targetTile.synergyEffectValue.addReward_Ice_Value = addValue;
            }
            else
            {
                targetTile.synergyEffectValue.addReward_Ice_Value = -removeValue;
            }
        }
    }

    /// <summary>
    /// �ó��� ȿ�� ���� 
    /// </summary>
    private void SetSynergyEffect(int effectID)
    {
        List<string> effectValue = new List<string>();

        if (effectID == -1)
        {
            effectValue.Add("0");
            effectValue.Add("0");
        }
        else
        {
            SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�ó��� �ߵ�");
            effectValue = sheetTextData.DataList[effectID].SynergyEffectValue;
        }

        Debug.Log($"targetValue {effectValue}");

        switch (effectID)
        {
            case 1:
            case 2:
            case 3:
                {
                    int targetValue = int.Parse(Regex.Replace(effectValue[0], @"\D", ""));

                    //���� ��ġ�� Ÿ�Ͽ� ����
                    SynergyEffect_AdditionalRewardType_AddValue = targetValue;

                    //�̹� ��ġ�� Ÿ�Ͽ� ����
                    Add_AllTileRewardPercent(targetValue);
                    break;
                }
            case 4:
            case 5:
            case 6:
                {
                    int targetValue = int.Parse(Regex.Replace(effectValue[0], @"\D", ""));
                    AddTilePeriod = targetValue;
                    break;
                }
            case 7:
            case 8:
            case 9:
                {
                    //string[] values = effectTextUI.Split('%');

                    int deValue = Math.Abs(int.Parse(Regex.Replace(effectValue[0], @"\D", "")) );
                    int addValue = int.Parse(Regex.Replace(effectValue[1], @"\D", "")); ;

                    Debug.Log($"deValue {deValue} / addValue {addValue}");

                    //���� ��ġ�� Ÿ�Ͽ� ����
                    SynergyEffect_Ice_AdditionalRewardType_AddValue = addValue;
                    SynergyEffect_Ice_AdditionalRewardType_DelValue = deValue;

                    //�̹� ��ġ�� Ÿ�Ͽ� ����
                    Change_IceRewardPercent(deValue, addValue);
                    break;
                }
            case 10:
            case 11:
                {
                    int targetValue = int.Parse(Regex.Replace(effectValue[0], @"\D", ""));

                    //���� ��ġ�� Ÿ�Ͽ� ����
                    SynergyEffect_Reward_MultifyValue = targetValue;

                    //�̹� ��ġ�� Ÿ�Ͽ� ����
                    for (int i = 0; i < tileManagerInst.assignedTiles.Count; i++)
                    {
                        DefaultTile targetTile = tileManagerInst.assignedTiles[i].GetComponent<DefaultTile>();
                        targetTile.synergyEffectValue.reward_MultifyValue  = targetValue;
                    }
                    break;
                }
            case 12:
            case 13:
            case 14:
            case 15:
                {
                    int targetValue = int.Parse(Regex.Replace(effectValue[0], @"\D", ""));

                    //���� ��ġ�� Ÿ�Ͽ� ����
                    synergyEffect_ChangeSendValue = targetValue;

                    //�̹� ��ġ�� Ÿ�Ͽ� ����
                    for (int i = 0; i < TileManager.Instance.assignedTiles.Count; i++)
                    {
                        DefaultTile targetTile = TileManager.Instance.assignedTiles[i].GetComponent<DefaultTile>();

                        if (targetTile.tileProperty == TileProperty.Sand)
                        {
                            targetTile.synergyEffectValue.changeSendValue = targetValue;
                        }
                    }

                    break;
                }
        }
    }

    //----------------------------------------------------------------------------------
    public int AddTilePeriod
    {
        get
        {
            return addTilePeriod;
        }
        set
        {
            addTilePeriod = value;
            CurTilePeriod = 0;
        }
    }

    public int CurTilePeriod
    {
        get
        {
            return curTilePeriod;
        }
        set
        {
            curTilePeriod = value;
        }
    }

    public float SynergyEffect_AdditionalRewardType_AddValue
    {
        get
        {
            return synergyEffect_AdditionalRewardType_AddValue;
        }
        set
        {
            synergyEffect_AdditionalRewardType_AddValue = value;
        }
    }

    public float SynergyEffect_Ice_AdditionalRewardType_AddValue
    {
        get
        {
            return synergyEffect_Ice_AdditionalRewardType_AddValue;
        }
        set
        {
            synergyEffect_Ice_AdditionalRewardType_AddValue = value;
        }
    }

    public float SynergyEffect_Ice_AdditionalRewardType_DelValue
    {
        get
        {
            return synergyEffect_Ice_AdditionalRewardType_DelValue;
        }
        set
        {
            synergyEffect_Ice_AdditionalRewardType_DelValue = value;
        }
    }

    public int SynergyEffect_Reward_MultifyValue
    {
        get
        {
            return synergyEffect_Reward_MultifyValue;
        }
        set
        {
            synergyEffect_Reward_MultifyValue = value;
        }
    }

    public float SynergyEffect_ChangeSendValue
    {
        get
        {
            return synergyEffect_ChangeSendValue;
        }
        set
        {
            synergyEffect_ChangeSendValue = value;
        }
    }
}
