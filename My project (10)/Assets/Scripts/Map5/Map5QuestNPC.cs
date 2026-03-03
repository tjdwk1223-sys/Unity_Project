using UnityEngine;

public class Map5QuestNPC : MonoBehaviour
{
    [Header("--- 연결할 것들 ---")]
    public GameObject interactUI;   // [E] 대화하기
    public DialogueManager dialogueManager;
    public GameObject mushroomObj;  // 맵에 있는 버섯 오브젝트
    public Animator anim;

    [Header("--- 예/아니오 버튼 (Canvas에 있는 버튼 연결) ---")]
    public GameObject yesButton;
    public GameObject noButton;

    [Header("--- 대사 목록 ---")]
    public string[] talk_Intro = { "안녕? 난 모모야.", "배가 너무 고픈데... 저기 울타리 너머 위험한 곳에 버섯 좀 가져다줄 수 있어?" };
    // 여기서 예/아니오 선택

    public string[] talk_Thinking = { "흐음... 생각 좀 해봤어?", "내 부탁 들어줄 거야?" };
    // 여기서 예/아니오 선택

    public string[] talk_Accepted = { "고마워! 기다리고 있을게! 빨리 다녀와~" };
    public string[] talk_Ing = { "아직이야? 배가 등가죽에 붙겠어..." };

    public string[] talk_Success1 = { "와! 진짜 가져왔구나! 냠냠...", "음... 근데 아직 배가 덜 찼어.", "진짜 미안한데 딱 한 번만 더 다녀와 줄 수 있어?" };
    // 여기서 예/아니오 선택

    public string[] talk_Refuse2 = { "장난하냐?", "농담이고~ 알겠어, 이번이 진짜 마지막이야! 부탁해!" }; // 아니오 선택 시

    public string[] talk_Success2 = { "꺼억~ 잘 먹었다!", "덕분에 살았어. 너 정말 착한 아이구나?", "이제 집으로 돌아가도 좋아. 잘 가!" };

    [Header("--- 상태 변수 (건드리지 마세요) ---")]
    public int state = 0;
    // 0:첫만남, 1:생각중, 2:퀘스트1수락, 3:퀘스트1완료(복귀), 4:퀘스트2수락, 5:완료
    public bool hasItem = false;
    private bool isPlayerNearby = false;

    void Start()
    {
        // 시작할 때 버튼은 숨겨두기
        if (yesButton) yesButton.SetActive(false);
        if (noButton) noButton.SetActive(false);
    }

    void Update()
    {
        // 대화 중이 아니고 + 플레이어가 없으면 운동 멈춤(Idle)
        if (!dialogueManager.isDialogueActive || !isPlayerNearby)
        {
            if (anim != null) anim.SetBool("isExercising", false);
        }
        // 퀘스트 수행 중일 때만 운동(푸쉬업)
        else if (state == 2 || state == 4)
        {
            if (anim != null) anim.SetBool("isExercising", true);
        }

        // --- E키 상호작용 ---
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !dialogueManager.isDialogueActive)
        {
            // 대화 시작하면 일단 운동 멈춤
            if (anim != null) anim.SetBool("isExercising", false);

            StartConversation();
        }
    }

    void StartConversation()
    {
        switch (state)
        {
            case 0: // 첫 만남 -> 예/아니오
                dialogueManager.StartDialogue(talk_Intro);
                Invoke("ShowButtons", 1f); // 대사 끝나갈 쯤 버튼 띄우기
                break;

            case 1: // 거절 후 다시 말 걸 때 -> 예/아니오
                dialogueManager.StartDialogue(talk_Thinking);
                Invoke("ShowButtons", 1f);
                break;

            case 2: // 1차 심부름 중
                dialogueManager.StartDialogue(talk_Ing);
                break;

            case 3: // 1차 버섯 가져옴 -> 예/아니오 (한번 더?)
                dialogueManager.StartDialogue(talk_Success1);
                Invoke("ShowButtons", 1f);
                break;

            case 4: // 2차 심부름 중
                dialogueManager.StartDialogue(talk_Ing);
                break;

            case 5: // 2차 버섯 가져옴 (최종 완료)
                dialogueManager.StartDialogue(talk_Success2);
                break;
        }
    }

    // 버섯 스크립트에서 호출하는 함수
    public void GetMushroom()
    {
        hasItem = true;
        if (state == 2)
        {
            state = 3;
            if (GameManager.Instance != null) GameManager.Instance.AddScore(10); // 1차 배달 +10점
        }
        if (state == 4)
        {
            state = 5;
            if (GameManager.Instance != null) GameManager.Instance.AddScore(15); // 2차 배달 +15점
        }
    }

    // --- 버튼 기능 (UI 버튼 OnClick에 연결하세요) ---
    public void OnClickYes()
    {
        HideButtons(); // 버튼 숨김

        if (state == 0 || state == 1) // 1차 수락
        {
            dialogueManager.StartDialogue(talk_Accepted);
            state = 2; // 퀘스트 시작
            if (mushroomObj) mushroomObj.SetActive(true); // 버섯 켜기
        }
        else if (state == 3) // 2차 수락
        {
            dialogueManager.StartDialogue(new string[] { "고마워! 빨리 다녀와!" });
            state = 4; // 2차 퀘스트 시작
            if (mushroomObj) mushroomObj.SetActive(true); // 버섯 리필
        }
    }

    public void OnClickNo()
    {
        HideButtons();

        if (state == 0 || state == 1) // 1차 거절
        {
            dialogueManager.StartDialogue(new string[] { "알겠어... 마음 바뀌면 다시 말 걸어줘." });
            state = 1; // '생각중' 상태로 변경
        }
        else if (state == 3) // 2차 거절 ("장난하냐?")
        {
            dialogueManager.StartDialogue(talk_Refuse2);
            state = 4; // 억지로 2차 퀘스트 시작시킴 (강제 진행)
            if (mushroomObj) mushroomObj.SetActive(true); // 버섯 리필
        }
    }

    void ShowButtons() { if (yesButton) yesButton.SetActive(true); if (noButton) noButton.SetActive(true); }
    void HideButtons() { if (yesButton) yesButton.SetActive(false); if (noButton) noButton.SetActive(false); }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) { isPlayerNearby = true; if (interactUI) interactUI.SetActive(true); } }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) { isPlayerNearby = false; if (interactUI) interactUI.SetActive(false); } }
}