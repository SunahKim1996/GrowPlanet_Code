using UnityEngine;

public class TimeLineTrigger : MonoBehaviour
{    
    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.CompareTag("EquipTile"))
        {
            //SoundManager.instance.PlayTargetAudio(SoundType.SFX, "Á¡¼ö È¹µæ");

            DefaultTile targetTile = collision.gameObject.GetComponent<DefaultTile>();
            targetTile.PlayRewardFX();

            int tileID = targetTile.tileID;
            GameManager.Instance.TotalReward = targetTile.GetTotalScore(tileID);

            if (targetTile.tileProperty == TileDataManager.TileProperty.Sand)
            {
                targetTile.ChangeRandomSendTile();
            }
        }
    }
}
