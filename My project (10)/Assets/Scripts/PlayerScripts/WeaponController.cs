using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public KikiController playerController;

    void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponentInParent<KikiController>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 데미지 가져오기
        int damage = (playerController != null && playerController.stats != null)
                     ? playerController.stats.damage : 20;

        // 1. 좀비는 무조건 때림
        ZombieController zombie = other.GetComponent<ZombieController>();
        if (zombie != null)
        {
            zombie.OnHit();
            zombie.TakeDamage(damage);
            return;
        }

        // 2. ★ 미미는 '공격 모드'일 때만 때림!
        MimiNPC mimi = other.GetComponent<MimiNPC>();
        if (mimi != null)
        {
            // 미미가 빡쳐서(isAttacking) 싸우는 중일 때만 데미지 들어감
            if (mimi.isAttacking == true)
            {
                mimi.TakeDamage(damage);
                Debug.Log("전투 모드 미미 타격 성공!");
            }
            else
            {
                Debug.Log("미미는 현재 아군이라 때릴 수 없습니다.");
            }
        }
    }
}