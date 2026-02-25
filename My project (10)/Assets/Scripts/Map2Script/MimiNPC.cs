using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class MimiNPC : MonoBehaviour, IInteractable
{
    [Header("★ 능력치 데이터 연결")]
    public CharacterStats stats; // MimiStats 연결 확인!

    [Header("UI 연결 (Inspector에서 드래그)")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject choiceGroup1, choiceGroup2, choiceGroup3;
    public TextMeshProUGUI killCountText;

    [Header("미미 상태 데이터")]
    public int currentHp;
    public bool isAttacking = false; // 전투 모드 (비웃음 선택 시)
    public bool isQuestActive = false; // 좀비 20마리 퀘스트
    public int zombieKillCount = 0;

    private int dialogueStep = 0; // ★ 대화 단계 기억 (도망갔다 와도 유지)
    private float lastAttackTime = 0f;
    private bool isPlayerNear = false;
    private bool isFollowing = false;
    private Transform player;
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // 스탯 초기화
        if (stats != null)
        {
            currentHp = stats.maxHp;
            agent.speed = stats.moveSpeed;
            agent.stoppingDistance = stats.attackRange;
        }

        GameObject pObj = GameObject.FindWithTag("Player");
        if (pObj != null) player = pObj.transform;

        if (dialoguePanel) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // 1. 대화 중 멀어지면 창 자동 닫기
        if (dialoguePanel.activeSelf && player != null && Vector3.Distance(transform.position, player.position) > 4.0f)
        {
            CloseDialogue();
        }

        // 2. E키 상호작용 (대화 기억 로직 포함)
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isAttacking && currentHp > 0)
        {
            if (!dialoguePanel.activeSelf) TalkToMimi();
        }

        // 3. 애니메이션 Speed 실시간 반영 (추격 시 필수)
        if (agent != null && anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        // 4. 퀘스트 UI 업데이트
        if (isQuestActive && killCountText != null)
        {
            killCountText.text = "좀비 처치: " + zombieKillCount + " / 20";
            if (zombieKillCount >= 20) CompleteQuest();
        }

        if (isFollowing && !isAttacking) FollowPlayer();

        // 5. ★ 흑화 모드: 키키를 쫓아가서 패기
        if (isAttacking) AttackPlayer();
    }

    public void Interact()
    {
        if (currentHp > 0 && !isAttacking && !dialoguePanel.activeSelf) TalkToMimi();
    }

    // 상황별 대화 시작
    void TalkToMimi()
    {
        SetPlayerAttack(false); // 대화 중 키키 공격 금지

        if (isQuestActive)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = $"부탁할게... (현재 처치 수: {zombieKillCount} / 20)";
            Invoke("CloseDialogue", 1.5f);
        }
        else if (isFollowing)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "너만 믿고 갈게!";
            Invoke("CloseDialogue", 1.5f);
        }
        else
        {
            // ★ 끊겼던 대화 단계부터 시작
            if (dialogueStep == 0) OpenDialogue();
            else if (dialogueStep == 1) AskForHelp();
            else if (dialogueStep == 2) ShowKneelDialogue();
        }
    }

    // ★ 흑화 미미의 추적 및 공격
    void AttackPlayer()
    {
        if (player == null || currentHp <= 0) return;

        float dist = Vector3.Distance(transform.position, player.position);
        agent.SetDestination(player.position); // 실시간 추격

        if (dist <= agent.stoppingDistance + 0.5f)
        {
            agent.isStopped = true;
            anim.SetBool("isAttacking", true);

            // 키키 쳐다보기
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);

            // 진짜 데미지 타격 (1초마다 10씩)
            if (Time.time >= lastAttackTime + 1.0f)
            {
                lastAttackTime = Time.time;
                KikiController k = player.GetComponent<KikiController>();
                if (k != null) k.TakeDamage(10);
            }
        }
        else
        {
            agent.isStopped = false;
            anim.SetBool("isAttacking", false);
        }
    }

    // --- 대화 및 선택지 함수들 (Step 기억 포함) ---
    public void OpenDialogue()
    {
        dialoguePanel.SetActive(true);
        choiceGroup1.SetActive(true);
        dialogueText.text = "나는 미미야... 너는 누구야?";
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void FirstChoice(string opt)
    {
        choiceGroup1.SetActive(false);
        dialogueStep = 1; // 1단계 완료 기억
        if (opt == "a") dialogueText.text = "나는 키키야.";
        Invoke("AskForHelp", 1.5f);
    }

    void AskForHelp()
    {
        if (dialoguePanel == null) return;
        dialoguePanel.SetActive(true);
        choiceGroup2.SetActive(true);
        dialogueText.text = "주변 좀비 좀 처치해 줄 수 있어?";
    }

    public void SecondChoice(int num)
    {
        choiceGroup2.SetActive(false);
        if (num == 1) { isFollowing = true; anim.SetInteger("WalkType", 1); CloseDialogue(); }
        else if (num == 2) { isQuestActive = true; CloseDialogue(); }
        else if (num == 3) { dialogueStep = 2; anim.SetTrigger("doKneel"); ShowKneelDialogue(); }
    }

    void ShowKneelDialogue()
    {
        dialoguePanel.SetActive(true);
        choiceGroup3.SetActive(true);
        dialogueText.text = "제발... 이렇게 부탁할게!!";
    }

    public void IgnoreSubChoice(int num)
    {
        choiceGroup3.SetActive(false);
        if (num == 1) { isQuestActive = true; CloseDialogue(); }
        else if (num == 2)
        {
            // ★ 비웃음 선택 시 적으로 변신!
            isAttacking = true;
            gameObject.tag = "Enemy";
            anim.Play("Locomotion", 0, 0f);
            CloseDialogue();
        }
    }

    void CompleteQuest() { isQuestActive = false; anim.SetTrigger("doDance"); Invoke("FollowAfterDance", 3.0f); }
    void FollowAfterDance() { CloseDialogue(); isFollowing = true; anim.SetInteger("WalkType", 0); }

    void CloseDialogue()
    {
        CancelInvoke();
        dialoguePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetPlayerAttack(true); // 대화 끝나면 키키 공격 다시 활성화
    }

    void FollowPlayer() { if (player == null) return; agent.destination = player.position; agent.isStopped = Vector3.Distance(transform.position, player.position) < 2f; }
    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) isPlayerNear = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) isPlayerNear = false; }

    void SetPlayerAttack(bool can) { if (player != null) { PlayerAttack pa = player.GetComponent<PlayerAttack>(); if (pa != null) pa.enabled = can; } }

    public void TakeDamage(int d) { currentHp -= d; anim.SetTrigger("doHit"); if (currentHp <= 0) Die(); }
    void Die() { anim.SetTrigger("Die"); agent.enabled = false; GetComponent<Collider>().enabled = false; this.enabled = false; Destroy(gameObject, 7f); }
}