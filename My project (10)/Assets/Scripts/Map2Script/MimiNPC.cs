using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class MimiNPC : MonoBehaviour, IInteractable
{
    [Header("★ 능력치 데이터 연결")]
    public CharacterStats stats;

    [Header("UI 연결 (Inspector에서 드래그)")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject choiceGroup1, choiceGroup2, choiceGroup3;
    public TextMeshProUGUI killCountText;

    [Header("미미 상태 데이터")]
    public int currentHp;
    public bool isAttacking = false;

    public int questState = 0;
    public int zombieKillCount = 0;

    private int dialogueStep = 0;
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

        // ★ [안전장치 추가] 스탯 데이터가 없어도 알아서 기본값으로 작동하게 만듭니다!
        currentHp = (stats != null) ? stats.maxHp : 100;
        agent.speed = (stats != null) ? stats.moveSpeed : 3.5f; // 속도가 0이 되는 걸 방지!
        agent.stoppingDistance = 2.0f; // 사거리 무조건 2.0 강제 고정!

        GameObject pObj = GameObject.FindWithTag("Player");
        if (pObj != null) player = pObj.transform;

        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (killCountText) killCountText.text = "";
    }

    void Update()
    {
        if (dialoguePanel.activeSelf && player != null && Vector3.Distance(transform.position, player.position) > 4.0f)
        {
            CloseDialogue();
        }

        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isAttacking && currentHp > 0)
        {
            if (!dialoguePanel.activeSelf) TalkToMimi();
        }

        if (agent != null && anim != null)
        {
            // 속도에 따라 걷기/뛰기 애니메이션 자연스럽게 연결
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (questState == 1 && killCountText != null)
        {
            killCountText.text = "좀비 처치: " + zombieKillCount + " / 10";

            if (zombieKillCount >= 10)
            {
                questState = 2;
                killCountText.text = "미미에게 돌아가서 대화(E)하세요!";
            }
        }
        else if (questState == 3 && killCountText != null)
        {
            killCountText.text = "";
        }

        if (isFollowing && !isAttacking) FollowPlayer();

        // 전투 모드일 때만 공격!
        if (isAttacking) AttackPlayer();
    }

    public void Interact()
    {
        if (currentHp > 0 && !isAttacking && !dialoguePanel.activeSelf) TalkToMimi();
    }

    void TalkToMimi()
    {
        SetPlayerAttack(false);

        if (questState == 3)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "빨리 저기 있는 마법의 돌에게 가봐! 나도 따라갈게!";
            Invoke("CloseDialogue", 2.0f);
        }
        else if (questState == 2)
        {
            GiveReward();
        }
        else if (questState == 1)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = $"아직 좀비가 남았어... 빨리 부탁할게! (현재 처치 수: {zombieKillCount} / 10)";
            Invoke("CloseDialogue", 1.5f);
        }
        else if (isFollowing && questState == 0)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "너만 믿고 갈게!";
            Invoke("CloseDialogue", 1.5f);
        }
        else
        {
            if (dialogueStep == 0) OpenDialogue();
            else if (dialogueStep == 1) AskForHelp();
            else if (dialogueStep == 2) ShowKneelDialogue();
        }
    }

    void GiveReward()
    {
        questState = 3;
        if (GameManager.Instance != null) GameManager.Instance.hasMagicItem = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = "대단해! 그걸 다 해치우다니... 이건 내 고마움의 표시야. [마법의 파편]을 획득했다!";

        anim.SetTrigger("doDance");
        Invoke("FollowAfterDance", 3.0f);
    }

    void AttackPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 거리가 2.0f 보다 멀면 무조건 미친 듯이 쫓아갑니다!
        if (distance > 2.0f)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        // 거리가 2.0f 안으로 들어오면(코앞이면) 멈춰서 때립니다!
        else
        {
            agent.isStopped = true;

            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);

            if (Time.time >= lastAttackTime + 2.0f)
            {
                anim.Play("Punching", 0, 0f);
                lastAttackTime = Time.time;
            }
        }
    }

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
        dialogueStep = 1;
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
        if (num == 1)
        {
            isFollowing = true;
            anim.SetInteger("WalkType", 1);
            // ▼▼▼ [이 부분을 꼭 추가해 주세요!] ▼▼▼
            questState = 1;
            CloseDialogue();
        }
        else if (num == 2) { questState = 1; CloseDialogue(); }
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
        if (num == 1) { questState = 1; CloseDialogue(); }
        else if (num == 2)
        {
            isAttacking = true;
            gameObject.tag = "Enemy";
            // ★ [오류 원인 제거] 무한루프를 유발하던 anim.SetBool("isAttacking", true); 를 삭제했습니다!
            anim.Play("Punching", 0, 0f);
            CloseDialogue();
        }
    }

    void FollowAfterDance() { CloseDialogue(); isFollowing = true; anim.SetInteger("WalkType", 0); }

    void CloseDialogue()
    {
        CancelInvoke();
        dialoguePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetPlayerAttack(true);
    }

    void FollowPlayer() { if (player == null) return; agent.destination = player.position; agent.isStopped = Vector3.Distance(transform.position, player.position) < 2f; }
    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) isPlayerNear = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) isPlayerNear = false; }
    void SetPlayerAttack(bool can) { if (player != null) { PlayerAttack pa = player.GetComponent<PlayerAttack>(); if (pa != null) pa.enabled = can; } }
    public void TakeDamage(int d) { currentHp -= d; anim.SetTrigger("doHit"); if (currentHp <= 0) Die(); }
    void Die() { anim.SetTrigger("Die"); agent.enabled = false; GetComponent<Collider>().enabled = false; this.enabled = false; Destroy(gameObject, 7f); }

    // =========================================================================
    // ★ [새로 추가된 애니메이션 이벤트 함수] 
    // 미미가 주먹을 뻗는 정확한 타이밍에 이 함수를 실행할 겁니다!
    // =========================================================================
    // =========================================================================
    // ★ [수정됨] 파라미터(Float)를 받는 애니메이션 이벤트 함수
    // =========================================================================
    public void AnimEvent_MimiPunch(float multiplier)
    {
        if (player == null || currentHp <= 0 || !isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float hitRange = 2.5f; // 미미 사거리는 2.0으로 고정했으니 타격 판정은 2.5 정도로 여유 있게

        if (distance <= hitRange)
        {
            KikiController kiki = player.GetComponent<KikiController>();
            if (kiki != null)
            {
                float baseDamage = (stats != null) ? stats.damage : 15f; // 스탯이 없으면 기본 15 대미지
                float finalDamage = baseDamage * multiplier; // ★ 파라미터로 받은 배수 곱하기!

                kiki.TakeDamage(finalDamage);
            }
        }
    }
}