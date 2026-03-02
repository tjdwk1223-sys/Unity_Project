using System.Collections;
using UnityEngine;
using TMPro;

public class MagicStone : MonoBehaviour
{
    [Header("대사 설정")]
    public string[] normalLines = { "마법의 힘이 깃들어 있는 돌이다.", "좌측에 길이 있으리라..." };
    public string[] upgradeLines = { "오... 그 파편은 미미의 보물...", "좋다, 너의 검에 숨겨진 힘을 해방해주마!", "[시스템] V 스킬이 강화되었습니다!" };

    [Header("UI 오브젝트 연결 (만든 거 드래그하세요)")]
    public GameObject interactUI;      // "E를 누르세요" 안내문
    public GameObject stoneCanvas;     // MagicStoneCanvas
    public TextMeshProUGUI stoneText;  // StoneText

    private bool isPlayerNearby = false;
    private bool isTalking = false;
    private string[] currentLines;
    private int lineIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        // 처음엔 다 꺼둡니다.
        if (interactUI != null) interactUI.SetActive(false);
        if (stoneCanvas != null) stoneCanvas.SetActive(false);
    }

    void Update()
    {
        // 1. 대화 시작 로직
        if (isPlayerNearby && !isTalking && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
        // 2. 대화 넘기기 로직 (E키나 마우스 왼쪽 클릭)
        else if (isTalking)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                NextSentence();
            }
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        lineIndex = 0;
        stoneCanvas.SetActive(true);
        interactUI.SetActive(false);

        // 퀘스트 상태 체크 (기존 로직 유지)
        if (GameManager.Instance != null && GameManager.Instance.isVSkillUpgraded)
            currentLines = new string[] { "이미 너의 검엔 강력한 마력이 깃들어 있다." };
        else if (GameManager.Instance != null && GameManager.Instance.hasMagicItem)
        {
            currentLines = upgradeLines;
            GameManager.Instance.isVSkillUpgraded = true;
            GameManager.Instance.hasMagicItem = false;
        }
        else
            currentLines = normalLines;

        StartCoroutine(TypeEffect(currentLines[lineIndex]));
    }

    void NextSentence()
    {
        if (isTyping) // 타이핑 중이면 즉시 완성
        {
            StopAllCoroutines();
            stoneText.text = currentLines[lineIndex];
            isTyping = false;
            return;
        }

        lineIndex++;

        if (lineIndex < currentLines.Length)
        {
            StartCoroutine(TypeEffect(currentLines[lineIndex]));
        }
        else
        {
            // 모든 대사 끝! UI 종료
            isTalking = false;
            stoneCanvas.SetActive(false);
        }
    }

    IEnumerator TypeEffect(string sentence)
    {
        isTyping = true;
        stoneText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            stoneText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!isTalking) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            isTalking = false; // 멀어지면 대화 강제 종료
            interactUI.SetActive(false);
            stoneCanvas.SetActive(false);
        }
    }
}