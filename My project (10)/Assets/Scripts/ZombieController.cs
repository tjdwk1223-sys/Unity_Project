using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [Header("★ 능력치 데이터 연결")]
    public CharacterStats stats;

    [Header("상태 확인용")]
    public int currentHp;

    [Header("기타 설정")]
    public Transform player;
    public float detectRange = 20.0f; // ★ 탐지 범위를 20으로 늘렸습니다.
    public float hitDuration = 0.5f; // ★ 경직 시간을 0.5초로 줄여 더 빠릿하게 만듭니다.
    public float destroyDelay = 5.0f;

    private bool isHit = false;
    private float lastAttackTime;

    private NavMeshAgent agent;
    private Animator anim;
    private Vector3 startPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;

        if (stats != null)
        {
            currentHp = stats.maxHp;
            agent.speed = stats.moveSpeed;
            // ★ 사거리만큼 멈추도록 설정
            agent.stoppingDistance = stats.attackRange;
        }
        else currentHp = 100;

        if (player == null)
        {
            GameObject kiki = GameObject.Find("Kiki2");
            if (kiki != null) player = kiki.transform;
        }
    }

    void Update()
    {
        // 죽었거나 경직 중이면 아무것도 안 함
        if (player == null || currentHp <= 0 || isHit) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float attackRange = (stats != null) ? stats.attackRange : 2.0f;

        if (distance <= attackRange)
        {
            float cooldown = (stats != null) ? stats.AttackCooldown : 1.0f;
            if (Time.time >= lastAttackTime + cooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else if (distance <= detectRange)
        {
            ChasePlayer(); // ★ 추격 모드
        }
        else
        {
            ReturnToIdle(); // ★ 복귀 모드
        }
    }

    void ChasePlayer()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid) return;

        agent.isStopped = false; // ★ 정지 해제 필수!
        agent.speed = (stats != null) ? stats.moveSpeed : 3.5f;
        agent.SetDestination(player.position);

        anim.SetBool("isRunning", true);
        anim.SetBool("isWalking", false);
    }

    void AttackPlayer()
    {
        agent.isStopped = true; // 공격할 땐 멈춤
        anim.SetBool("isRunning", false);
        anim.SetTrigger("attack");

        // 플레이어 쳐다보기
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void ReturnToIdle()
    {
        if (Vector3.Distance(transform.position, startPosition) > 1.5f)
        {
            agent.isStopped = false;
            agent.speed = (stats != null) ? stats.moveSpeed * 0.5f : 2.0f;
            agent.SetDestination(startPosition);
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
    }

    public void OnHit()
    {
        if (currentHp <= 0 || isHit) return;

        isHit = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        anim.SetTrigger("GetHit");
        // 넉백 살짝 추가
        transform.Translate(Vector3.back * 0.3f);
        Invoke("EndHit", hitDuration);
    }

    void EndHit() { if (currentHp > 0) isHit = false; }

    public void TakeDamage(int damageAmount)
    {
        if (currentHp <= 0) return;
        currentHp -= damageAmount;

        // 피격 시 즉시 추격하도록 설정 (반격)
        detectRange = 50f;

        if (currentHp <= 0) Die();
        else OnHit();
    }

    void Die()
    {
        CancelInvoke();
        anim.SetTrigger("Die");
        agent.isStopped = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        // 미미 퀘스트 카운트 올리기
        MimiNPC mimi = FindObjectOfType<MimiNPC>();
        if (mimi != null && mimi.isQuestActive)
        {
            mimi.zombieKillCount++;
        }

        this.enabled = false;
        Destroy(gameObject, destroyDelay);
    }
}