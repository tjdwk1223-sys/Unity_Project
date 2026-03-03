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

        // 3. [신규 기능] 보스 피격 + V스킬 강화 코드
        BossAI boss = other.GetComponent<BossAI>();
        if (boss != null)
        {
            // V 스킬 강화 상태인지 체크
            if (GameManager.Instance != null && GameManager.Instance.isVSkillUpgraded && playerController != null)
            {
                // 현재 플레이어가 V 스킬(Skill5) 애니메이션 중인지 확인
                Animator pAnim = playerController.GetComponent<Animator>();
                if (pAnim != null && pAnim.GetCurrentAnimatorStateInfo(0).IsName("Skill5"))
                {
                    // V스킬로 때렸을 때 점수 추가 (선택 사항)
                    GameManager.Instance.AddScore(10);
                    Debug.Log("⚡ V 스킬로 보스 타격! 점수 획득!");
                }
            }
            // 보스에게 대미지 전달
            boss.TakeDamage(currentDamage);
        }
    }
}