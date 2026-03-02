using UnityEngine;

public class StraightSlash : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 2f;
    private float timer = 0f;

    [Header("--- 대미지 설정 ---")]
    public int baseDamage = 30;
    public GameObject damageTextPrefab;

    [HideInInspector]
    public Vector3 shootDirection;

    void OnEnable() { timer = 0f; }

    void Update()
    {
        transform.position += shootDirection * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= lifeTime) gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        ZombieController zombie = other.GetComponent<ZombieController>();
        // ★ 보스(BossAI) 매개변수를 넘기기 위해 ApplyDamage 함수 형태를 바꿨습니다.
        if (zombie != null) { ApplyDamage(zombie.transform, zombie, null, null); return; }

        MimiNPC mimi = other.GetComponent<MimiNPC>();
        if (mimi != null && mimi.isAttacking) { ApplyDamage(mimi.transform, null, mimi, null); return; }

        // ▼▼▼ [추가됨] 보스와 부딪혔는지 확인! ▼▼▼
        BossAI boss = other.GetComponent<BossAI>();
        if (boss != null) { ApplyDamage(boss.transform, null, null, boss); return; }
    }

    // ★ BossAI boss 매개변수가 하나 더 추가되었습니다.
    void ApplyDamage(Transform targetTransform, ZombieController zombie, MimiNPC mimi, BossAI boss)
    {
        int finalDamage = Random.Range(baseDamage - 5, baseDamage + 5);
        bool isCrit = Random.Range(0, 100) < 20;
        if (isCrit) finalDamage *= 2;

        if (zombie != null) { zombie.OnHit(); zombie.TakeDamage(finalDamage); }
        if (mimi != null) { mimi.TakeDamage(finalDamage); }

        if (boss != null)
        {
            // ★ [로그 추가] 검기가 마왕에게 맞았을 때!
            Debug.Log($"👉 [검기 명중] 마왕에게 {finalDamage} 대미지 전송 시도! (크리티컬: {isCrit})");
            boss.TakeDamage(finalDamage);
        }

        if (damageTextPrefab != null) // ... (이하 텍스트 띄우는 코드 동일)
        {
            Vector3 spawnPos = targetTransform.position + Vector3.up * 2.0f;
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<DamageText>().Setup(finalDamage, isCrit);
        }
        gameObject.SetActive(false);
    }
}