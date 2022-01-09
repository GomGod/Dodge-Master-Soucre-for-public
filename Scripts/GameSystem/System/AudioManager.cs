using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixer mixer;

    private void Awake()
    {
        Instance = this;
    }

    public void BgmOn() => mixer.SetFloat("bgmVolume", 0f);
    public void BgmOff() => mixer.SetFloat("bgmVolume", -80f);
    public void SfxOn()=> mixer.SetFloat("sfxVolume", 0f);
    public void SfxOff()=> mixer.SetFloat("sfxVolume", -80f);
}
