using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public KikiController playerController;

    [Header("--- 무기 충돌체 ---")]
    public Collider damageCollider;

    // ★ [새로 추가] 이번 공격에 들어갈 진짜 대미지를 기억하는 변수!
    [HideInInspector] // (인스펙터 창에서는 안 보이게 숨김)
    public int currentDamage;

    void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponentInParent<KikiController>();
        }

        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        if (damageCollider != null) damageCollider.enabled = true;
    }

    public void DisableCollider()
    {
        if (damageCollider != null) damageCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // ★ [수정됨] 이제 스탯에서 무조건 가져오는 게 아니라, 키키가 정해준 currentDamage로 때립니다!

        // 1. 좀비는 무조건 때림
        ZombieController zombie = other.GetComponent<ZombieController>();
        if (zombie != null)
        {
            zombie.OnHit();
            zombie.TakeDamage(currentDamage); // 키키가 명령한 대미지 전달!
            return;
        }

        // 2. 미미는 '공격 모드'일 때만 때림
        MimiNPC mimi = other.GetComponent<MimiNPC>();
        if (mimi != null)
        {
            if (mimi.isAttacking == true)
            {
                mimi.TakeDamage(currentDamage); // 키키가 명령한 대미지 전달!
                Debug.Log("전투 모드 미미 타격 성공!");
            }
        }
    }
}
