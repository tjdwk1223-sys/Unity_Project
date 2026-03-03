using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class EndingDirector : MonoBehaviour
{
    [Header("--- UI 연결 ---")]
    public Text talkText;   // 대사 (꿈이었구나...)
    public Text nameText;   // 제작자 (주성훈)
    public Text scoreText;  // 점수판

    [Header("--- 버튼 연결 ---")]
    public GameObject restartButton; // 다시하기 버튼
    public GameObject quitButton;    // 종료하기 버튼

    void Start()
    {
        // 시작할 때 글씨들 다 숨기기 (깨끗하게)
        talkText.text = "";
        nameText.text = "";
        scoreText.text = "";

        // 버튼들도 처음엔 숨겨둡니다
        if (restartButton != null) restartButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);

        // 엔딩 영화 시작!
        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        // 1. 첫 번째 대사 ("꿈이었었구나 ...")
        yield return StartCoroutine(TypeWriter("꿈이었었구나 ..."));
        yield return new WaitForSeconds(2.0f); // 2초 쉬고

        // 2. 두 번째 대사 ("너무 생생해....")
        talkText.text = ""; // 지우고
        yield return StartCoroutine(TypeWriter("너무 생생해...."));
        yield return new WaitForSeconds(2.0f);

        // 3. 대사 사라지고 제작자 등장!
        talkText.text = "";
        yield return new WaitForSeconds(1.0f);

        nameText.text = "제작   주성훈"; // 쾅!
        yield return new WaitForSeconds(3.0f); // 3초 동안 자랑스럽게 보여주기

        // 4. 대망의 점수 공개!
        int finalScore = 0;

        // GameManager가 있으면 점수 가져오기 (없으면 0점)
        if (GameManager.Instance != null)
        {
            finalScore = GameManager.Instance.totalScore;
        }

        scoreText.text = "최종 점수 : " + finalScore + " 점";

        // (보너스) 점수에 따른 멘트
        yield return new WaitForSeconds(1.0f);
        if (finalScore >= 50) scoreText.text += "\n(멋진 모험이었습니다!)";
        else scoreText.text += "\n(다음엔 더 잘할 수 있어요!)";

        // 5. ★ 버튼 등장 및 마우스 잠금 해제 ★
        yield return new WaitForSeconds(1.0f);
        if (restartButton != null) restartButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);

        // 버튼을 눌러야 하니까 마우스 커서를 보이게 만듭니다!
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 타자기처럼 한 글자씩 나오는 효과
    IEnumerator TypeWriter(string sentence)
    {
        talkText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            talkText.text += letter;
            yield return new WaitForSeconds(0.1f); // 글자 나오는 속도
        }
    }

    // ▼▼▼ [필수] 이 함수들이 있어야 버튼에서 보입니다! ▼▼▼

    public void OnClickRestart()
    {
        // 점수 초기화를 위해 매니저 삭제
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
        // 첫 맵으로 이동 (씬 이름 확인하세요!)
        SceneManager.LoadScene("Map1");
    }

    public void OnClickQuit()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}