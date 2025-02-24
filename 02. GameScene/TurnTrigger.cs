using UnityEngine;

public class TurnTrigger : MonoBehaviour
{
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (AllManager.instance.isTutorial)
            return;
      
        if (collision.CompareTag("TurnTrigger"))
        {
            TileEffectManager.Instance.CurTilePeriod++;

            //한턴에 일정한 시간을 부여하기 위해, 다음 턴이 되면 Line 회전값을 정확한 값으로 초기화  
            SunLineManager.Instance.RefreshRotValue(0f);

            if (TileEffectManager.Instance.CurTilePeriod <= 0)
                return;

            //마지막 턴에 애매하게 시간이 남아서 카드 선택이 되는 현상을 막음 
            if (GameManager.Instance.curTime <= 30 && !GameManager.Instance.CanGoNextRound())
            {
                return;
            }

            CardUIManager.Instance. StartSelectCard(false);
        }
    }
    */
}
