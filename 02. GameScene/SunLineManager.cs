using UnityEngine;
using UnityEngine.EventSystems;

public class SunLineManager : Singleton<SunLineManager>
{
    [SerializeField] private GameObject sunLine;
    [SerializeField] private float sunLineSpeed;

    private bool isStartSunLine = false;
    private bool isTimesSpeedUp = false;
       

    //----------------------------------------------------------------------------------
    void Update()
    {
        if (IsStartSunLine == false)
        {
            return;
        }

        sunLine.transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * (360f / 60f) * GameManager.Instance.SpeedChangeValue);
    }

    public void RefreshRotValue(float value)
    {
        sunLine.transform.rotation = Quaternion.Euler(0, 0, value);
    }

    public float GetRotValue()
    {
        return sunLine.transform.eulerAngles.z;
    }

    //----------------------------------------------------------------------------------
    public void OnChangeSunLineSpeed()
    {
        SoundManager.instance.PlayTargetAudio(SoundType.SFX, "일반 터치 사운드");

        if (IsTimesSpeedUp == true)
        {
            GameManager.Instance.SpeedChangeValue = 1f; //SunLineSpeed /= 10f;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            GameManager.Instance.SpeedChangeValue = 10f;  //SunLineSpeed *= 10f;
        }

        IsTimesSpeedUp = !IsTimesSpeedUp;
    }

    public void InitSunLine()
    {
        sunLine.transform.rotation = Quaternion.identity;

        IsTimesSpeedUp = false;
        GameManager.Instance.SpeedChangeValue = 1f; //SunLineSpeed /= 10f;
        EventSystem.current.SetSelectedGameObject(null);
    }

    //----------------------------------------------------------------------------------
    public bool IsStartSunLine
    {
        get
        {
            return isStartSunLine;
        }
        set
        {
            isStartSunLine = value;
        }
    }

    public bool IsTimesSpeedUp
    {
        get
        {
            return isTimesSpeedUp;
        }
        set
        {
            isTimesSpeedUp = value;
        }
    }
}
