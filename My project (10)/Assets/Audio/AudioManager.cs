using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public AudioSource bgmSource; // [새로 추가] 배경음악을 재생하는 스피커(Audio Source)를 연결할 칸
    public GameObject settingsMenu;
    private bool isMenuOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMenuOpen = !isMenuOpen;
            settingsMenu.SetActive(isMenuOpen);

            // 메뉴 열리면 게임 멈춤(0), 닫히면 다시 시작(1)
            Time.timeScale = isMenuOpen ? 0f : 1f;

            // [추가된 부분] 마우스 커서 껐다 켜기!
            if (isMenuOpen)
            {
                Cursor.visible = true; // 마우스 커서 보이게
                Cursor.lockState = CursorLockMode.None; // 마우스 잠금 해제해서 움직이게
            }
            else
            {
                Cursor.visible = false; // 마우스 숨기기
                Cursor.lockState = CursorLockMode.Locked; // 마우스 화면 중앙에 고정 (게임 조작용)
            }
        }
    }

    // [새로 추가된 함수] 버튼을 누르면 배경음악을 바꿔주는 기능
    public void ChangeBGM(AudioClip newClip)
    {
        if (bgmSource != null) // 안전 장치: 스피커가 잘 연결되어 있을 때만 작동
        {
            bgmSource.Stop();        // 현재 나오는 음악 멈춤
            bgmSource.clip = newClip; // 새로운 음악(전투 음악)으로 교체
            bgmSource.Play();        // 짠! 하고 다시 재생
        }
    }

    public void SetBgmVolume(float volume)
    {
        masterMixer.SetFloat("BgmVol", Mathf.Log10(volume) * 20);
    }
}