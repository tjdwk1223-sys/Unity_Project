using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footstepClip;
    public AudioClip swordClip;

    // 새로 추가된 사운드 파일 공간
    public AudioClip kickClip;      // 발차기 소리용
    public AudioClip xSkillClip;    // X 스킬(폭발) 소리용

    public void PlayFootstep()
    {
        if (audioSource != null && footstepClip != null) audioSource.PlayOneShot(footstepClip);
    }

    public void PlaySwordSound()
    {
        if (audioSource != null && swordClip != null) audioSource.PlayOneShot(swordClip);
    }

    // 발차기 소리 재생 함수 (새로 추가!)
    public void PlayKickSound()
    {
        if (audioSource != null && kickClip != null) audioSource.PlayOneShot(kickClip);
    }

    // X 스킬 소리 재생 함수 (새로 추가!)
    public void PlayXSkillSound()
    {
        if (audioSource != null && xSkillClip != null) audioSource.PlayOneShot(xSkillClip);
    }
}