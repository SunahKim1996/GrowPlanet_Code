using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum SoundType
{
    BGM,
    SFX,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] sfxs;

    [SerializeField] private AudioSource curBGM;
    [SerializeField] private AudioSource curSFX;
    [SerializeField] private AudioSource curSFX_Reward; //HACK: 다른 사운드와 겹치면 안돼서 따로 처리 

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private Dictionary<string, AudioClip> bgmList = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxList = new Dictionary<string, AudioClip>();

    //----------------------------------------------------------------------------------
    // 게임 시작과 동시에 싱글톤을 구성
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitSoundList();
        InitVolume();
    }

    //----------------------------------------------------------------------------------    
    private void InitSoundList()
    {
        for (int i = 0; i < bgms.Length; i++) 
        {
            bgmList[bgms[i].name] = bgms[i];
        }

        for (int j = 0; j < sfxs.Length; j++)
        {
            sfxList[sfxs[j].name] = sfxs[j];
        }
    } 

    private void InitVolume()
    {
        //배경음
        if (PlayerPrefs.HasKey("BGMVolume"))            
        {
            AllManager.instance.playerData.bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        }
        else
        {
            AllManager.instance.playerData.bgmVolume = 1f; 
            PlayerPrefs.SetFloat("BGMVolume", 1f);
        }

        bgmSlider.value = AllManager.instance.playerData.bgmVolume;
        OnChangeBGMVolume();

        //효과음
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            AllManager.instance.playerData.sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            AllManager.instance.playerData.sfxVolume = 1f;
            PlayerPrefs.SetFloat("SFXVolume", 1f);
        }
        
        sfxSlider.value = AllManager.instance.playerData.sfxVolume;
        OnChangeSFXVolume();
    }

    public void PlayTargetAudio(SoundType soundType, string soundName)
    {
        if (soundType == SoundType.BGM)
        {
            curBGM.clip = bgmList[soundName];
            curBGM.Play();
        }
        else if (soundType == SoundType.SFX)
        {
            AudioSource targetAudioSource;
            if (soundName == "목표 점수 상승")
                targetAudioSource = curSFX_Reward;
            else
                targetAudioSource = curSFX;

            targetAudioSource.clip = sfxList[soundName];
            targetAudioSource.Play();
        }
    }

    public void StopTargetAudio(SoundType soundType, string soundName, bool isFadeOut = false)
    {
        if (soundType == SoundType.BGM)
        {
            curBGM.clip = bgmList[soundName];            

            if (isFadeOut)
                StartCoroutine("SoundFadeOut");
            else
                curBGM.Stop();
        }
        else if (soundType == SoundType.SFX)
        {
            AudioSource targetAudioSource;
            if (soundName == "목표 점수 상승")
                targetAudioSource = curSFX_Reward;
            else
                targetAudioSource = curSFX;

            targetAudioSource.clip = sfxList[soundName];
            targetAudioSource.Stop();
        }
    }

    IEnumerator SoundFadeOut()
    {
        float preVolume = curBGM.volume;

        while (curBGM.volume >= 0)
        {
            curBGM.volume -= 0.01f;

            if (curBGM.volume <= 0)
            {
                curBGM.volume = preVolume;
                yield break;
            }

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    public void OnChangeBGMVolume()
    {
        float volume = bgmSlider.value;
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void OnChangeSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
