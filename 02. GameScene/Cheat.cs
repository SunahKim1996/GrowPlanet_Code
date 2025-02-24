using UnityEngine;
using TMPro;

public class Cheat : MonoBehaviour
{
    [SerializeField] private GameObject cheatPanel;
    [SerializeField] private TMP_Text cheatButtonTxt;

    private bool isCheatOpen = false;

    private void Start()
    {
        cheatPanel.SetActive(false);
        cheatButtonTxt.text = "ġƮ ����";
    }

    public void OnToggleCheatPanel()
    {
        isCheatOpen = !isCheatOpen;
        cheatPanel.SetActive(isCheatOpen);

        if (isCheatOpen)
        {
            cheatButtonTxt.text = "ġƮ �ݱ�";
        }
        else
        {
            cheatButtonTxt.text = "ġƮ ����";
        }
    }

    public void OnCreateEquipTileCheat(int typeID)
    {
        TileManager.Instance.AddGetEquipTileList(-1, typeID, -1);
    }
}
