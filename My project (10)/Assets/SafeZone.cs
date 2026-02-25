using UnityEngine;
using System.Collections;

public class SafeZone : MonoBehaviour
{
    [Header("회복 설정")]
    public float healAmount = 5f; // 한 번에 차오르는 회복량
    public float healInterval = 1f; // 회복 간격 (1초마다)

    private Coroutine healCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어(키키)가 들어왔을 때
        if (other.CompareTag("Player"))
        {
            // 회복 코루틴 시작
            healCoroutine = StartCoroutine(HealOverTime(other.gameObject));
            Debug.Log("안전지대 진입: 마왕의 위협으로부터 안전합니다!");
        }

        // 마왕(보스)이 들어왔을 때
        if (other.CompareTag("Boss"))
        {
            PushBackBoss(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어(키키)가 나갔을 때
        if (other.CompareTag("Player"))
        {
            // 회복 코루틴 중지
            if (healCoroutine != null)
            {
                StopCoroutine(healCoroutine);
                healCoroutine = null;
            }
            Debug.Log("안전지대 이탈: 다시 전투가 시작됩니다!");
        }
    }

    IEnumerator HealOverTime(GameObject playerObj)
    {
        // KikiController 컴포넌트 가져오기
        KikiController kiki = playerObj.GetComponent<KikiController>();

        if (kiki != null && kiki.stats != null)
        {
            while (true)
            {
                // 현재 체력이 최대 체력보다 낮을 때만 회복
                if (kiki.currentHp < kiki.stats.maxHp)
                {
                    kiki.currentHp += healAmount;

                    // 최대 체력을 넘어가면 최대 체력으로 고정
                    if (kiki.currentHp > kiki.stats.maxHp)
                    {
                        kiki.currentHp = kiki.stats.maxHp;
                    }

                    Debug.Log($"체력 회복 됨! 현재 체력: {kiki.currentHp} / {kiki.stats.maxHp}");
                }

                // 설정한 시간(healInterval)만큼 대기
                yield return new WaitForSeconds(healInterval);
            }
        }
    }

    void PushBackBoss(Collider Boss)
    {
        // 보스가 못 들어오게 밀어내는 로직
        Vector3 pushDirection = (Boss.transform.position - transform.position).normalized;
        pushDirection.y = 0; // 위아래로 튀는 것 방지

        Boss.transform.position += pushDirection * 2f;
        Debug.Log("마왕은 이 성역에 발을 들일 수 없습니다!");
    }
}