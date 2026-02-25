using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("필수 연결")]
    public BossData data;
    public Transform kiki;

    [Header("맨손 판정 설정 (뼈 연결)")]
    public Collider leftHandCollider;
    public Collider rightHandCollider;

    private Animator anim;
    private NavMeshAgent agent;

    private float currentHp;
    private int currentPhase = 1;
    private bool isStunned = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isRunningState = false;
    private float[] attackCooldowns = new float[5];

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (data != null)
        {
            currentHp = data.maxHp;
            agent.speed = data.walkSpeed;
            agent.stoppingDistance = data.attackDistance - 0.5f;
        }

        // 시작할 때 혹시라도 켜져 있을 손 판정 끄기 (버그 방지)
        if (leftHandCollider) leftHandCollider.enabled = false;
        if (rightHandCollider) rightHandCollider.enabled = false;

        anim.SetInteger("Phase", 1);
    }

    // --- 애니메이션 이벤트 호출용 함수 ---
    public void StartLeftPunch() { if (leftHandCollider) leftHandCollider.enabled = true; }
    public void StopLeftPunch() { if (leftHandCollider) leftHandCollider.enabled = false; }
    public void StartRightPunch() { if (rightHandCollider) rightHandCollider.enabled = true; }
    public void StopRightPunch() { if (rightHandCollider) rightHandCollider.enabled = false; }
    // ------------------------------------

    void Update()
    {
        if (isDead || isStunned || isAttacking || data == null || kiki == null) return;

        float distance = Vector3.Distance(transform.position, kiki.position);
        anim.SetFloat("Distance", distance);

        bool isMoving = agent.velocity.magnitude > 0.1f;
        anim.SetBool("isWalking", isMoving && !isRunningState);

        UpdateCooldowns();
        ThinkAndAct(distance);
    }

    void ThinkAndAct(float distance)
    {
        bool wasRunning = isRunningState;

        List<int> readySkills = new List<int>();
        for (int i = 1; i <= 4; i++)
        {
            if (attackCooldowns[i] <= 0) readySkills.Add(i);
        }

        bool anySkillReady = readySkills.Count > 0;

        // 1. 공격 (사거리 안)
        if (anySkillReady && distance <= data.attackDistance)
        {
            isRunningState = false;
            ResetEvadeAnim(); // 공격 전 혹시 남은 찌꺼기 확실히 정리
            PerformAttack(readySkills);
        }
        // 2. 맹추격 (사거리 밖)
        else if (anySkillReady && distance > data.attackDistance)
        {
            ResetEvadeAnim();
            if (!isRunningState)
            {
                isRunningState = true;
                anim.ResetTrigger("stopRun");
                anim.SetTrigger("doRun");
                agent.isStopped = false;
                agent.speed = data.runSpeed;
            }
            agent.SetDestination(kiki.position);
        }
        // 3. 쿨타임 대기
        else if (!anySkillReady)
        {
            isRunningState = false;
            StandStillAndWait();
        }

        // 맹추격하다가 멈춰야 할 때 처리
        if (wasRunning && !isRunningState)
        {
            anim.ResetTrigger("doRun");
            anim.SetTrigger("stopRun");
        }
    }

    void PerformAttack(List<int> readySkills)
    {
        int randomPick = Random.Range(0, readySkills.Count);
        int skillIndexToUse = readySkills[randomPick];
        StartCoroutine(AttackRoutine(skillIndexToUse));
    }

    IEnumerator AttackRoutine(int index)
    {
        isAttacking = true;
        agent.isStopped = true;
        anim.SetInteger("attackIndex", index);
        anim.SetTrigger("doAttack");
        attackCooldowns[index] = data.attackMaxCooldown;

        yield return new WaitForSeconds(2.0f); // 애니메이션 길이에 맞춰 대기

        // ★ 핵심: 공격 끝나면 혹시라도 켜져 있을 모든 주먹 판정 강제 종료 (다단히트 방지)
        StopLeftPunch();
        StopRightPunch();

        isAttacking = false;
        agent.isStopped = false;
    }

    void StandStillAndWait()
    {
        agent.isStopped = true;
        ResetEvadeAnim();

        // 미끄러지지 않고 제자리에서 키키만 노려봄
        Vector3 lookTarget = new Vector3(kiki.position.x, transform.position.y, kiki.position.z);
        transform.LookAt(lookTarget);
    }

    void ResetEvadeAnim()
    {
        // 족보 꼬임을 막기 위한 과거 회피 애니메이션 스위치 일괄 OFF
        anim.SetBool("isStrafing", false);
        anim.SetBool("isStrafingRight", false);
        anim.SetBool("isWalkingBack", false);
    }

    void UpdateCooldowns()
    {
        for (int i = 1; i <= 4; i++)
        {
            if (attackCooldowns[i] > 0) attackCooldowns[i] -= Time.deltaTime;
        }
    }

    // 데미지 및 페이즈 처리 로직 (이전과 동일하게 완벽 유지)
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHp -= damage;
        if (currentPhase == 1 && currentHp <= data.phase2Threshold)
        {
            StartCoroutine(ChangePhaseRoutine());
            return;
        }
        if (currentHp <= 0) { Die(); return; }
        if (damage >= 80) StartCoroutine(StunRoutine(3, data.stunTimeLarge));
        else if (damage >= 50) StartCoroutine(StunRoutine(2, data.stunTimeMedium));
        else if (damage >= 10) StartCoroutine(StunRoutine(1, data.stunTimeSmall));
    }

    IEnumerator StunRoutine(int hitType, float stunTime)
    {
        isStunned = true;
        agent.isStopped = true;
        anim.SetInteger("HitType", hitType);
        anim.SetTrigger("doHit");
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        agent.isStopped = false;
    }

    IEnumerator ChangePhaseRoutine()
    {
        currentPhase = 2;
        anim.SetInteger("Phase", 2);
        isStunned = true;
        agent.isStopped = true;
        anim.SetTrigger("doRage");
        yield return new WaitForSeconds(3.0f);
        isStunned = false;
        agent.isStopped = false;
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        anim.SetBool("isDead", true);
    }
}