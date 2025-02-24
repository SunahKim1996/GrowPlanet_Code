using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public GameObject curTarget = null;

    public TMP_Text cardTitleText;
    public TMP_Text cardEffectText;

    public Image tilePropertyImage;
    public Image tilePropertyIcon;
    public Image timePropertyIcon;
    public GameObject[] affinityList;

    public Image cardRewardFrame;
    public Image cardRankFrame;
    public TMP_Text rewardText;
    public Button selectButton;
}
