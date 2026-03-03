using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingDirector : MonoBehaviour
{
    [Header("--- UI 연결 ---")]
    public Text talkText;   // 대사
    public Text nameText;   // 제작자 이름
    public Text scoreText;  // 점수

    [Header("--- 버튼 연결 ---")]
    public GameObject restartButton; // 다시하기 버튼
    public GameObject quitButton;    // 종료하기 버튼

    void Start()
    {
        // 시작할 때 깔끔하게 비우기
        talkText.text = "";
        nameText.text = "";
        scoreText.text = "";

        if (restartButton != null) restartButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);

        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        // 1. 엔딩 연출 (기존과 동일)
        yield return StartCoroutine(TypeWriter("꿈이었었구나 ..."));
        yield return new WaitForSeconds(2.0f);
        
        talkText.text = ""; 
        yield return StartCoroutine(TypeWriter("너무 생생해...."));
        yield return new WaitForSeconds(2.0f);

        talkText.text = "";
        yield return new WaitForSeconds(1.0f);
        nameText.text = "제작   주성훈"; 
        yield return new WaitForSeconds(3.0f);

        // 2. 점수 공개
        int finalScore = 0;
        if (GameManager.Instance != null) finalScore = GameManager.Instance.totalScore;
        
        scoreText.text = "최종 점수 : " + finalScore + " 점";
        yield return new WaitForSeconds(1.0f);
        
        if (finalScore >= 50) scoreText.text += "\n(멋진 모험이었습니다!)";
        else scoreText.text += "\n(다음엔 더 잘할 수 있어요!)";

        // 3. 버튼 등장 (여기가 중요!)
        yield return new WaitForSeconds(1.0f);
        if (restartButton != null) restartButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);

        // ★ 마우스 커서 보이게 풀기 (이거 없으면 클릭 못함!)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator TypeWriter(string sentence)
    {
        talkText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            talkText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // =========================================================
    // ▼▼▼ [여기가 고장 났던 부분! 완벽하게 고쳤습니다] ▼▼▼
    // =========================================================

    public void OnClickRestart()
    {
        Debug.Log("다시하기 버튼 눌림!");

        // 1. 멈춘 시간을 다시 흐르게 함 (필수!)
        Time.timeScale = 1.0f; 

        // 2. 옛날 기록(게임매니저) 삭제하고 새로 시작
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        // 3. 0번 씬(오프닝)으로 이동
        SceneManager.LoadScene(0); 
    }

    public void OnClickQuit()
    {
        Debug.Log("종료 버튼 눌림!");

#if UNITY_EDITOR
        // 유니티 에디터에서는 이걸로 꺼야 꺼집니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 게임에서는 이걸로 꺼집니다.
        Application.Quit();
#endif
    }
}