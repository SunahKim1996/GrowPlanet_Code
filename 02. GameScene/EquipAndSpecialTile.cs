using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TileDataManager;

public class EquipAndSpecialTile : MonoBehaviour
{
    private TileManager tileManagerInst;

    //�ش� Ÿ�� ������
    //������� ���� [HideInInspector] ���� ����
    public TileProperty tileProperty;
    public TimeProperty timeProperty;
    public int tileID;

    //UI
    public Image bgImage;
    public Image timeBG;
    public Image timeIcon;

    void Start()
    {
        tileManagerInst = TileManager.Instance;
    }

    public void OnSelectEquipTile()
    {        
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "�Ϲ� ��ġ ����");

        bool isTutorial = AllManager.instance.isTutorial;
        GameObject curEquipTile = TileManager.Instance.CurEquipTileObject;
        GameObject selectEquipTile = EventSystem.current.currentSelectedGameObject;

        if (curEquipTile == selectEquipTile && !isTutorial)
        {
            tileManagerInst.InitSelectEquipTile();
        }
        else
        {
            tileManagerInst.CurEquipTileObject = selectEquipTile;
            tileManagerInst.CurEquipTileID = tileID;

            Sprite tilePropertySprite = bgImage.sprite;
            Sprite timePropertySprite = timeIcon.sprite;

            CardUIManager.Instance.Refresh_CardUI(tileID, tilePropertySprite, timePropertySprite);
            TileManager.Instance.ToggleCanSelectTileSign(true);
        }
    }
}
