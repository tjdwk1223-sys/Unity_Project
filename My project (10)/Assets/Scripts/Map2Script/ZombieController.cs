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
            // ★ [수정된 부분] 사거리 안에 들어오면 일단 멈추고 애니메이션을 끕니다! (대기 상태로 전환)
            agent.isStopped = true;
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);

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

    // ★ [수정된 부분] Wander 함수 전체 교체
    void Wander()
    {
        timer += Time.deltaTime;

        // (안전장치) 에이전트가 내비메시 위에 정상적으로 올라가 있을 때만 실행
        if (!agent.isOnNavMesh) return;

        if (timer >= wanderTimer || agent.remainingDistance <= 0.2f)
        {
            Vector3 newPos;

            // 유효한 목적지를 찾는 데 성공했을 때만 이동 명령을 내립니다.
            if (RandomNavSphere(startPosition, wanderRadius, -1, out newPos))
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

    // ★ [수정된 부분] RandomNavSphere 함수 전체 교체
    // NavMesh 위에서 랜덤한 목적지를 찾아 성공 여부를 반환해 주는 함수
    public static bool RandomNavSphere(Vector3 origin, float dist, int layermask, out Vector3 result)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;

        // SamplePosition이 갈 수 있는 위치를 찾아 성공(true)했을 때만 좌표 반환
        if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask))
        {
            result = navHit.position;
            return true;
        }

        // 실패했을 경우
        result = Vector3.zero;
        return false;
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
        // transform.Translate(Vector3.back * 0.3f);
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
        // 미미가 퀘스트 중(상태가 1)일 때만 카운트를 올린다
        if (mimi != null && mimi.questState == 1)
        {
            mimi.zombieKillCount++;
        }

        this.enabled = false;
        Destroy(gameObject, destroyDelay);
    }

    // =========================================================================
    // ★ [새로 추가된 애니메이션 이벤트 함수] 
    // 좀비가 손을 뻗어 때리는 정확한 타이밍에 이 함수를 실행할 겁니다!
    // =========================================================================
    // =========================================================================
    // ★ [수정됨] 파라미터(Float)를 받는 애니메이션 이벤트 함수
    // multiplier 파라미터에 1을 넣으면 100% 대미지, 1.5를 넣으면 150% 대미지가 들어갑니다.
    // =========================================================================
    public void AnimEvent_DealDamage(float multiplier)
    {
        if (player == null || currentHp <= 0) return;

        // 키키가 공격을 피했을 수도 있으니 거리를 다시 잽니다. (사거리보다 살짝 여유를 줌)
        float distance = Vector3.Distance(transform.position, player.position);
        float hitRange = (stats != null) ? stats.attackRange + 0.5f : 2.5f;

        if (distance <= hitRange)
        {
            // KikiController 스크립트를 찾아서 대미지를 줍니다!
            KikiController kiki = player.GetComponent<KikiController>();
            if (kiki != null)
            {
                float baseDamage = (stats != null) ? stats.damage : 10f; // 스탯이 없으면 기본 10 대미지
                float finalDamage = baseDamage * multiplier; // ★ 파라미터로 받은 배수 곱하기!

                kiki.TakeDamage(finalDamage);
            }
        }
    }
}