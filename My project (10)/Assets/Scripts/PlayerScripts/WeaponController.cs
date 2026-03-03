using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public KikiController playerController;
    public Collider damageCollider;
    [HideInInspector] public int currentDamage;

    void Start()
    {
        if (playerController == null) playerController = GetComponentInParent<KikiController>();
        if (damageCollider == null) damageCollider = GetComponent<Collider>();
        if (damageCollider != null) damageCollider.enabled = false;
    }

    public void EnableCollider() { if (damageCollider != null) damageCollider.enabled = true; }
    public void DisableCollider() { if (damageCollider != null) damageCollider.enabled = false; }

    void OnTriggerEnter(Collider other)
    {
        // 1. [복구됨] 좀비 피격 코드 (이게 없어서 안 맞았던 겁니다!)
        ZombieController zombie = other.GetComponentInParent<ZombieController>();
        if (zombie != null)
        {
            zombie.OnHit();
            zombie.TakeDamage(currentDamage);
            return; // 좀비를 때렸으면 보스 판정은 패스 (중복 방지)
        }

        // 2. [기존 유지] 미미(NPC) 피격 코드 (혹시 몰라 넣어둠)
        MimiNPC mimi = other.GetComponentInParent<MimiNPC>();
        if (mimi != null && mimi.isAttacking)
        {
            mimi.TakeDamage(currentDamage);
        }

        // 3. [수정됨] 보스 피격 + V스킬 강화 코드
        BossAI boss = other.GetComponent<BossAI>();
        if (boss != null)
        {
            // 기본적으로는 무기의 현재 데미지를 사용
            int finalDamage = currentDamage;

            // V 스킬 강화 상태인지 체크
            if (GameManager.Instance != null && GameManager.Instance.isVSkillUpgraded && playerController != null)
            {
                // 현재 플레이어가 V 스킬(Skill5) 애니메이션 중인지 확인
                Animator pAnim = playerController.GetComponent<Animator>();
                if (pAnim != null && pAnim.GetCurrentAnimatorStateInfo(0).IsName("Skill5"))
                {
                    // ★★★ [핵심 수정] 데미지를 1000으로 덮어씌움! ★★★
                    finalDamage = 1000;

                    GameManager.Instance.AddScore(10);
                    Debug.Log("⚡ [검 타격] 전설의 V 스킬! 보스에게 1000 데미지!");
                }
            }

            // 결정된 데미지(일반 or 1000)를 전달
            boss.TakeDamage(finalDamage);
        }
    }
}