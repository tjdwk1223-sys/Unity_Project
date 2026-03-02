using UnityEngine;

public class BossPunch : MonoBehaviour
{
    public float damage = 10f; // 펀치 대미지
    public float hitRadius = 1.0f; // 주먹의 판정 크기 (엄청 넉넉함!)

    private Collider myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // 애니메이션에서 주먹 스위치를 켰을 때만 레이더(탐색) 작동!
        if (myCollider != null && myCollider.enabled)
        {
            // 내 주먹(transform.position)을 기준으로 반경 1.0f 안에 있는 모든 물체를 싹 다 스캔합니다.
            Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);

            foreach (Collider hit in hits)
            {
                // 스캔된 물체 중에 'Player' 태그를 가진 녀석이 있다면?
                if (hit.CompareTag("Player"))
                {
                    // 부모에게서 키키 스크립트를 찾아냅니다.
                    KikiController kiki = hit.GetComponentInParent<KikiController>();
                    if (kiki != null)
                    {
                        // 1. 대미지를 즉시 박아버림!
                        kiki.TakeDamage(damage);

                        // 2. 로그 띄우기
                        Debug.Log($"🤜 {gameObject.name}가 레이더 판정으로 키키를 팼습니다! 퍽!");

                        // 3. 다단히트 방지: 한 대 때렸으니 내 주먹 스위치를 즉시 꺼버립니다.
                        myCollider.enabled = false;

                        // 더 이상 스캔할 필요 없으니 종료
                        return;
                    }
                }
            }
        }
    }

    // 씬(Scene) 창에서 마왕 주먹의 실제 판정 크기를 빨간색 원으로 볼 수 있게 해줍니다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}