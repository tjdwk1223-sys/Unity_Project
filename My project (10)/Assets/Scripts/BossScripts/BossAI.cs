using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // UI 필요시

public class BossAI : MonoBehaviour
{
    [Header("필수 연결")]
    public BossData data;
    public Transform kiki;

    [Header("맨손 판정 설정 (뼈 연결)")]
    public Collider leftHandCollider;
    public Collider rightHandCollider;

    [Header("★ 보스 죽으면 열릴 포탈 (꼭 연결하세요!)")]
    public GameObject portalToMap5;

    private BossPunch leftPunch;
    private BossPunch rightPunch;

    private Animator anim;
    private NavMeshAgent agent;

    private float currentHp;
    private int currentPhase = 1;
    private bool isStunned = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isEvading = false;
    private bool isRunningState = false;

    private float lastHitTime = 0f;
    private float[] skillTimers = new float[5];
    private float strafeLeftTimer = 0f;
    private float strafeRightTimer = 0f;
    private float walkBackTimer = 0f;

    private float phaseDamageMultiplier = 1f;

    IEnumerator Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (leftHandCollider) leftPunch = leftHandCollider.GetComponent<BossPunch>();
        if (rightHandCollider) rightPunch = rightHandCollider.GetComponent<BossPunch>();

        if (data != null)
        {
            currentHp = data.maxHp;
            agent.speed = data.walkSpeed;
            agent.stoppingDistance = data.attackDistance - 0.5f;
        }

        if (leftHandCollider) leftHandCollider.enabled = false;
        if (rightHandCollider) rightHandCollider.enabled = false;
        anim.SetInteger("Phase", 1);

        // 포탈은 처음에 숨기기
        if (portalToMap5 != null) portalToMap5.SetActive(false);

        isStunned = true;
        yield return new WaitForSeconds(0.5f);
        isStunned = false;
    }

    public void StartLeftPunch() { if (leftHandCollider) leftHandCollider.enabled = true; }
    public void StopLeftPunch() { if (leftHandCollider) leftHandCollider.enabled = false; }
    public void StartRightPunch() { if (rightHandCollider) rightHandCollider.enabled = true; }
    public void StopRightPunch() { if (rightHandCollider) rightHandCollider.enabled = false; }

    void Update()
    {
        if (isDead || isStunned || data == null || kiki == null) return;
        UpdateCooldowns();

        if (isAttacking || isEvading)
        {
            Vector3 lookTarget = new Vector3(kiki.position.x, transform.position.y, kiki.position.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), Time.deltaTime * 5f);
            return;
        }

        float distance = Vector3.Distance(transform.position, kiki.position);
        anim.SetFloat("Distance", distance);
        bool isMoving = agent.velocity.magnitude > 0.1f;
        anim.SetBool("isWalking", isMoving && !isRunningState);
        ThinkAndAct(distance);
    }

    void UpdateCooldowns()
    {
        for (int i = 1; i <= 4; i++) { if (skillTimers[i] > 0) skillTimers[i] -= Time.deltaTime; }
        if (strafeLeftTimer > 0) strafeLeftTimer -= Time.deltaTime;
        if (strafeRightTimer > 0) strafeRightTimer -= Time.deltaTime;
        if (walkBackTimer > 0) walkBackTimer -= Time.deltaTime;
    }

    void ThinkAndAct(float distance)
    {
        bool wasRunning = isRunningState;
        List<int> readySkills = new List<int>();
        for (int i = 1; i <= 4; i++) { if (skillTimers[i] <= 0) readySkills.Add(i); }

        if (distance <= agent.stoppingDistance + 0.5f)
        {
            isRunningState = false;
            if (wasRunning) { anim.ResetTrigger("doRun"); anim.SetTrigger("stopRun"); }

            if (readySkills.Count > 0)
            {
                int randomPick = Random.Range(0, readySkills.Count);
                StartCoroutine(AttackRoutine(readySkills[randomPick]));
            }
            else
            {
                List<string> readyEvasions = new List<string>();
                if (strafeLeftTimer <= 0) readyEvasions.Add("Left");
                if (strafeRightTimer <= 0) readyEvasions.Add("Right");
                if (walkBackTimer <= 0) readyEvasions.Add("Back");

                if (readyEvasions.Count > 0)
                {
                    int randEvade = Random.Range(0, readyEvasions.Count);
                    StartCoroutine(EvadeRoutine(readyEvasions[randEvade]));
                }
                else StandStillAndWait();
            }
        }
        else
        {
            ResetEvadeAnim();
            if (!isRunningState)
            {
                isRunningState = true;
                anim.ResetTrigger("stopRun"); anim.SetTrigger("doRun");
                agent.isStopped = false; agent.speed = data.runSpeed * (currentPhase == 2 ? data.phase2SpeedMultiplier : 1f);
            }
            agent.SetDestination(kiki.position);
        }
    }

    IEnumerator AttackRoutine(int index)
    {
        isAttacking = true;
        agent.isStopped = true;

        float attackDmg = 0f;
        if (index == 1) attackDmg = data.skill1Damage;
        else if (index == 2) attackDmg = data.skill2Damage;
        else if (index == 3) attackDmg = data.skill3Damage;
        else if (index == 4) attackDmg = data.skill4Damage;

        float finalDmg = attackDmg * phaseDamageMultiplier;
        if (leftPunch) leftPunch.damage = finalDmg;
        if (rightPunch) rightPunch.damage = finalDmg;

        anim.SetInteger("attackIndex", index);
        anim.SetTrigger("doAttack");

        if (index == 1) skillTimers[1] = data.skill1Cooldown;
        else if (index == 2) skillTimers[2] = data.skill2Cooldown;
        else if (index == 3) skillTimers[3] = data.skill3Cooldown;
        else if (index == 4) skillTimers[4] = data.skill4Cooldown;

        yield return new WaitForSeconds(2.0f);

        StopLeftPunch(); StopRightPunch();
        isAttacking = false;
        agent.isStopped = false;
    }

    IEnumerator EvadeRoutine(string evadeType)
    {
        isEvading = true; agent.isStopped = true; ResetEvadeAnim();
        if (evadeType == "Left") { anim.SetBool("isStrafing", true); strafeLeftTimer = data.strafeLeftCooldown; }
        else if (evadeType == "Right") { anim.SetBool("isStrafingRight", true); strafeRightTimer = data.strafeRightCooldown; }
        else if (evadeType == "Back") { anim.SetBool("isWalkingBack", true); walkBackTimer = data.walkBackCooldown; }
        yield return new WaitForSeconds(2.0f);
        ResetEvadeAnim(); isEvading = false; agent.isStopped = false;
    }

    void StandStillAndWait() { agent.isStopped = true; ResetEvadeAnim(); transform.LookAt(new Vector3(kiki.position.x, transform.position.y, kiki.position.z)); }
    void ResetEvadeAnim() { anim.SetBool("isStrafing", false); anim.SetBool("isStrafingRight", false); anim.SetBool("isWalkingBack", false); }

    public void TakeDamage(float damage)
    {
        if (Time.time < lastHitTime + 0.1f) return;
        lastHitTime = Time.time;

        if (isDead) return;
        currentHp -= damage;
        Debug.Log("보스 남은 체력: " + currentHp);

        if (currentPhase == 1 && currentHp <= data.phase2Threshold) { StartCoroutine(ChangePhaseRoutine()); return; }
        if (currentHp <= 0) { Die(); return; }
        if (isStunned) return;

        if (damage >= data.stunThresholdLarge) StartCoroutine(StunRoutine(3, data.stunTimeLarge));
        else if (damage >= data.stunThresholdMedium) StartCoroutine(StunRoutine(2, data.stunTimeMedium));
        else if (damage >= data.stunThresholdSmall) StartCoroutine(StunRoutine(1, data.stunTimeSmall));
    }

    IEnumerator StunRoutine(int hitType, float stunTime)
    {
        isStunned = true;
        agent.isStopped = true;
        isRunningState = false;
        anim.ResetTrigger("doRun");
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
        isRunningState = false;
        anim.ResetTrigger("doRun");
        anim.SetTrigger("doRage");

        currentHp = data.maxHp;
        phaseDamageMultiplier = data.phase2DamageMultiplier;

        float sizeMult = data.phase2SizeMultiplier;
        transform.localScale *= sizeMult;
        agent.stoppingDistance *= sizeMult;
        agent.speed *= data.phase2SpeedMultiplier;

        if (leftPunch) leftPunch.hitRadius *= sizeMult;
        if (rightPunch) rightPunch.hitRadius *= sizeMult;

        yield return new WaitForSeconds(3.0f);
        isStunned = false;
        agent.isStopped = false;
        Debug.Log($"🔥 마왕 페이즈 2 진입! 체력 {currentHp}으로 완전 회복 및 스탯 강화 완료!");
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        anim.SetBool("isDead", true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(50);
        }

        Debug.Log("보스 사망! 3초 뒤 포탈 생성...");
        Invoke("CleanUpAndSpawnPortal", 3.0f);
    }

    void CleanUpAndSpawnPortal()
    {
        if (portalToMap5 != null)
        {
            portalToMap5.SetActive(true);
            Debug.Log("Map5로 가는 포탈 생성됨!");
        }
        gameObject.SetActive(false);
    }
}