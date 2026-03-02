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
        // ★ [로그 추가] 무기가 허공이 아닌 뭔가에 닿았을 때 무조건 이름 출력!
        Debug.Log($"[무기 충돌 테스트] 키키의 무기가 '{other.name}'(이)랑 부딪힘!");

        ZombieController zombie = other.GetComponent<ZombieController>();
        if (zombie != null) { zombie.OnHit(); zombie.TakeDamage(currentDamage); return; }

        MimiNPC mimi = other.GetComponent<MimiNPC>();
        if (mimi != null && mimi.isAttacking == true) { mimi.TakeDamage(currentDamage); return; }

        BossAI boss = other.GetComponent<BossAI>();
        if (boss != null)
        {
            // ★ [로그 추가] 마왕을 정확히 인식하고 딜을 넣기 직전!
            Debug.Log($"👉 [무기 타격 성공] 마왕에게 {currentDamage} 대미지 전송 시도!");
            boss.TakeDamage(currentDamage);
        }
    }
}