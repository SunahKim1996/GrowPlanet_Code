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

            //���Ͽ� ������ �ð��� �ο��ϱ� ����, ���� ���� �Ǹ� Line ȸ������ ��Ȯ�� ������ �ʱ�ȭ  
            SunLineManager.Instance.RefreshRotValue(0f);

            if (TileEffectManager.Instance.CurTilePeriod <= 0)
                return;

            //������ �Ͽ� �ָ��ϰ� �ð��� ���Ƽ� ī�� ������ �Ǵ� ������ ���� 
            if (GameManager.Instance.curTime <= 30 && !GameManager.Instance.CanGoNextRound())
            {
                return;
            }

            CardUIManager.Instance. StartSelectCard(false);
        }
    }
    */
}
