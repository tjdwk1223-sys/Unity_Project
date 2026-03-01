using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // 사운드 조절을 위해 필요!

public class SettingsManager : MonoBehaviour
{
    [Header("UI 패널 연결")]
    public GameObject settingsPanel;     // 환경설정 창
    public GameObject quitConfirmPanel;  // 종료 확인 팝업창

    private bool isPaused = false;       // 현재 일시정지 상태인지 기억하는 스위치

    void Start()
    {
        // 게임 시작 시 설정창과 팝업창은 무조건 숨겨둡니다.
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    void Update()
    {
        // ESC 키를 누르면
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // 켜져 있으면 끕니다
            }
            else
            {
                PauseGame();  // 꺼져 있으면 켭니다
            }
        }
    }

    // 1. 게임 일시정지 및 설정창 켜기
    public void PauseGame()
    {
        settingsPanel.SetActive(true);
        quitConfirmPanel.SetActive(false); // 혹시 켜져있을 팝업창 닫기
        Time.timeScale = 0f; // 게임 시간 정지! (좀비도 키키도 멈춤)
        isPaused = true;

        // 마우스 커서 보이게 하기
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 2. 게임 재개 및 설정창 끄기
    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 시간 다시 정상 속도로!
        isPaused = false;

        // 마우스 커서 다시 숨기고 화면 중앙에 가두기 (게임 플레이 모드)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 3. '게임 종료' 버튼 눌렀을 때 팝업창 띄우기
    public void ClickQuitButton()
    {
        quitConfirmPanel.SetActive(true);
    }

    // 4. 팝업창에서 'No' 눌렀을 때
    public void ClickNoButton()
    {
        quitConfirmPanel.SetActive(false);
    }

    // 5. 팝업창에서 'Yes' 눌렀을 때 진짜 종료!
    public void ClickYesButton()
    {
        Debug.Log("게임을 종료합니다!"); // 유니티 에디터 확인용
        Application.Quit(); // 실제 빌드된 게임에서 작동하는 종료 코드
    }
}