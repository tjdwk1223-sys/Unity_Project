using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public AudioSource bgmSource;

    // ★ [수정됨] ESC 눌러서 메뉴 켜고 끄는 코드는 SettingsManager가 전담하도록 싹 지웠습니다!

    public void ChangeBGM(AudioClip newClip)
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
            bgmSource.clip = newClip;
            bgmSource.Play();
        }
    }

    // [기존 유지] BGM 볼륨 슬라이더용 함수
    public void SetBgmVolume(float volume)
    {
        masterMixer.SetFloat("BgmVol", Mathf.Log10(volume) * 20);
    }

    // ★ [새로 추가] 효과음(SFX) 볼륨 슬라이더용 함수
    public void SetSfxVolume(float volume)
    {
        // 주의: 오디오 믹서에서 효과음 파라미터 이름을 "SfxVol"로 노출(Expose)해두셨다고 가정합니다!
        masterMixer.SetFloat("SfxVol", Mathf.Log10(volume) * 20);
    }
}