using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [Header("대미지 텍스트 설정")]
    public GameObject damageTextPrefab;

    [Header("★ 능력치 데이터 연결")]
    public CharacterStats stats;

    [Header("상태 확인용")]
    public int currentHp;

    [Header("기타 설정")]
    public Transform player;
    public float detectRange = 20.0f;
    public float hitDuration = 0.5f;
    public float destroyDelay = 5.0f;

    [Header("순찰(Patrol) 설정")]
    public float wanderRadius = 10f; // 순찰 반경 (10미터 내외)
    public float wanderTimer = 3f;   // 몇 초마다 새로운 목적지를 찾을지
    private float timer;

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
        timer = wanderTimer;

        if (anim != null) anim.applyRootMotion = false;

        if (stats != null)
        {
            currentHp = stats.maxHp;
            agent.speed = stats.moveSpeed;
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
            ChasePlayer(); // 추격
        }
        else
        {
            Wander(); // 멈추지 않고 순찰!
        }
    }

    // ★ 새롭게 추가된 순찰 로직
    // Wander 함수 내부의 목적지 설정 부분을 이렇게 바꿔주세요!
    void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer || agent.remainingDistance <= 0.2f)
        {
            Vector3 newPos = RandomNavSphere(startPosition, wanderRadius, -1);

            // ★ [해결 코드] 목적지가 무한대(Infinity)가 아닐 때만 이동 명령을 내립니다.
            if (newPos != Vector3.positiveInfinity && newPos != Vector3.negativeInfinity)
            {
                agent.SetDestination(newPos);
            }

            timer = 0;
        }

        agent.isStopped = false;
        agent.speed = (stats != null) ? stats.moveSpeed * 0.5f : 1.5f;

        anim.SetBool("isWalking", true);
        anim.SetBool("isRunning", false);
    }

    // NavMesh 위에서 랜덤한 목적지를 찾아주는 함수
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void ChasePlayer()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid) return;

        agent.isStopped = false;
        agent.speed = (stats != null) ? stats.moveSpeed : 3.5f;
        agent.SetDestination(player.position);

        anim.SetBool("isRunning", true);
        anim.SetBool("isWalking", false);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        anim.SetBool("isRunning", false);
        anim.SetBool("isWalking", false); // 공격 시 걷기도 확실히 꺼줌
        anim.SetTrigger("attack");

        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    public void TakeDamage(int damageAmount, bool isCrit = false)
    {
        if (currentHp <= 0) return;

        currentHp -= damageAmount;

        if (damageTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 2.0f;
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);
            DamageText dt = textObj.GetComponent<DamageText>();
            if (dt != null) dt.Setup(damageAmount, isCrit);
        }

        detectRange = 50f; // 한 대 맞으면 끝까지 쫓아오게

        if (currentHp <= 0) Die();
        else OnHit();
    }

    public void OnHit()
    {
        if (currentHp <= 0 || isHit) return;

        isHit = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        anim.SetTrigger("GetHit");
        transform.Translate(Vector3.back * 0.3f);
        Invoke("EndHit", hitDuration);
    }

    void EndHit() { if (currentHp > 0) isHit = false; }

    void Die()
    {
        CancelInvoke();
        anim.SetTrigger("Die");
        agent.isStopped = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        MimiNPC mimi = FindObjectOfType<MimiNPC>();
        if (mimi != null && mimi.isQuestActive) mimi.zombieKillCount++;

        this.enabled = false;
        Destroy(gameObject, destroyDelay);
    }
}