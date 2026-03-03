using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("UI 패널 연결")]
    public GameObject settingsPanel;
    public GameObject quitConfirmPanel;

    [Header("★ 오디오 믹서 연결")]
    public AudioMixer audioMixer; // MainMixer를 넣으세요

    private bool isPaused = false;

    void Start()
    {
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        settingsPanel.SetActive(true);
        quitConfirmPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ClickQuitButton() { quitConfirmPanel.SetActive(true); }
    public void ClickNoButton() { quitConfirmPanel.SetActive(false); }
    public void ClickYesButton() { Application.Quit(); }

    // ★★★ [배경음 조절 함수] 이름: BgmVol ★★★
    public void SetBgmVolume(float sliderValue)
    {
        audioMixer.SetFloat("BgmVol", Mathf.Log10(sliderValue) * 20);
    }

    // ★★★ [효과음 조절 함수] 이름: SfxVol ★★★
    public void SetSfxVolume(float sliderValue)
    {
        audioMixer.SetFloat("SfxVol", Mathf.Log10(sliderValue) * 20);
    }
    // ★★★ [게임 시작] 버튼용 함수 (새로 추가!) ★★★
    public void ClickStartGame()
    {
        // 1번 씬(Map1)으로 이동합니다.
        // (만약 Map1이 1번이 아니라면 숫자를 1 대신 "Map1"이라고 적으세요)
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
} // <--- 여기가 스크립트 맨 끝입니다.
