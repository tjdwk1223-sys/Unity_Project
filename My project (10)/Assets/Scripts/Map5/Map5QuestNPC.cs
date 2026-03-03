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
    public string[] talk_Thinking = { "흐음... 생각 좀 해봤어?", "내 부탁 들어줄 거야?" };
    public string[] talk_Accepted = { "고마워! 기다리고 있을게! 빨리 다녀와~" };
    public string[] talk_Ing = { "아직이야? 배가 등가죽에 붙겠어..." };
    public string[] talk_Success1 = { "와! 진짜 가져왔구나! 냠냠...", "음... 근데 아직 배가 덜 찼어.", "진짜 미안한데 딱 한 번만 더 다녀와 줄 수 있어?" };
    public string[] talk_Refuse2 = { "장난하냐?", "농담이고~ 알겠어, 이번이 진짜 마지막이야! 부탁해!" };
    public string[] talk_Success2 = { "꺼억~ 잘 먹었다!", "덕분에 살았어. 너 정말 착한 아이구나?", "이제 집으로 돌아가도 좋아. 잘 가!" };

    [Header("--- 상태 변수 ---")]
    public int state = 0;
    public bool hasItem = false;
    private bool isPlayerNearby = false;
    private int clickCount = 0; // 대사 넘김 카운트

    void Start()
    {
        // 1. 대화 매니저 연결
        if (GameManager.Instance != null)
        {
            dialogueManager = GameManager.Instance.GetComponent<DialogueManager>();
        }

        // 2. [수정] 시작할 때 대화창 및 버튼 강제 초기화
        if (dialogueManager != null && dialogueManager.dialoguePanel != null)
        {
            dialogueManager.dialoguePanel.SetActive(false);
            dialogueManager.isDialogueActive = false;
        }

        HideButtons();
    }

    void Update()
    {
        if (dialogueManager == null) return;

        // 1. 운동 애니메이션 관리
        if (!dialogueManager.isDialogueActive || !isPlayerNearby)
        {
            if (anim != null) anim.SetBool("isExercising", false);
        }
        else if (state == 2 || state == 4)
        {
            if (anim != null) anim.SetBool("isExercising", true);
        }

        // 2. E키 상호작용
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !dialogueManager.isDialogueActive)
        {
            if (anim != null) anim.SetBool("isExercising", false);
            StartConversation();
        }

        // 3. [기능] 대사를 다 읽고 클릭해야 버튼 등장
        if (dialogueManager.isDialogueActive && (state == 0 || state == 3))
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickCount++;
                int targetClicks = (state == 0) ? talk_Intro.Length - 1 : talk_Success1.Length - 1;

                if (clickCount >= targetClicks)
                {
                    ShowButtons();
                }
            }
        }
    }

    // ★ 플레이어 체력 풀 리커버리 함수
    void HealPlayer()
    {
        KikiController player = FindObjectOfType<KikiController>();
        if (player != null && player.stats != null)
        {
            player.currentHp = player.stats.maxHp;
            Debug.Log("💖 키키의 체력이 완전히 회복되었습니다!");
        }
    }

    void StartConversation()
    {
        clickCount = 0;

        switch (state)
        {
            case 0:
                dialogueManager.StartDialogue(talk_Intro);
                break;
            case 1:
                dialogueManager.StartDialogue(talk_Thinking);
                ShowButtons();
                break;
            case 2:
                dialogueManager.StartDialogue(talk_Ing);
                break;
            case 3: // 1차 버섯 배달 시
                HealPlayer(); // 배달 올 때 피 회복!
                dialogueManager.StartDialogue(talk_Success1);
                break;
            case 4:
                dialogueManager.StartDialogue(talk_Ing);
                break;
            case 5: // 최종 버섯 배달 시
                HealPlayer(); // 배달 올 때 피 회복!
                dialogueManager.StartDialogue(talk_Success2);
                break;
        }
    }

    // 버섯 채취 시 호출되는 함수 (Map5Flower에서 호출)
    public void GetMushroom()
    {
        hasItem = true;
        HealPlayer(); // ★ 채취하는 순간 피 회복!

        if (state == 2) { state = 3; if (GameManager.Instance) GameManager.Instance.AddScore(10); }
        if (state == 4) { state = 5; if (GameManager.Instance) GameManager.Instance.AddScore(15); }
    }

    public void OnClickYes()
    {
        HideButtons();
        if (state == 0 || state == 1)
        {
            dialogueManager.StartDialogue(talk_Accepted);
            state = 2;
            if (mushroomObj) mushroomObj.SetActive(true);
        }
        else if (state == 3)
        {
            dialogueManager.StartDialogue(new string[] { "고마워! 빨리 다녀와!" });
            state = 4;
            if (mushroomObj) mushroomObj.SetActive(true);
        }
    }

    public void OnClickNo()
    {
        HideButtons();
        if (state == 0 || state == 1)
        {
            dialogueManager.StartDialogue(new string[] { "알겠어... 마음 바뀌면 다시 말 걸어줘." });
            state = 1;
        }
        else if (state == 3)
        {
            dialogueManager.StartDialogue(talk_Refuse2);
            state = 4;
            if (mushroomObj) mushroomObj.SetActive(true);
        }
    }

    void ShowButtons()
    {
        if (yesButton && !yesButton.activeSelf) yesButton.SetActive(true);
        if (noButton && !noButton.activeSelf) noButton.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideButtons()
    {
        if (yesButton) yesButton.SetActive(false);
        if (noButton) noButton.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) { isPlayerNearby = true; if (interactUI) interactUI.SetActive(true); } }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) { isPlayerNearby = false; if (interactUI) interactUI.SetActive(false); } }
}