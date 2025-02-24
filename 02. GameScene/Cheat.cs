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
        cheatButtonTxt.text = "치트 열기";
    }

    public void OnToggleCheatPanel()
    {
        isCheatOpen = !isCheatOpen;
        cheatPanel.SetActive(isCheatOpen);

        if (isCheatOpen)
        {
            cheatButtonTxt.text = "치트 닫기";
        }
        else
        {
            cheatButtonTxt.text = "치트 열기";
        }
    }

    public void OnCreateEquipTileCheat(int typeID)
    {
        TileManager.Instance.AddGetEquipTileList(-1, typeID, -1);
    }
}
