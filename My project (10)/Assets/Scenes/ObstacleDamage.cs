using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    public float damageAmount = 20f;
    public float knockbackForce = 5f; // 힘은 5 정도로 적당히!

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<KikiController>();

            if (player != null && !player.isInvincible)
            {
                player.TakeDamage(damageAmount);

                // ▼▼▼ [수정된 부분] 휀스 중심이 아니라, "부딪힌 표면"에서 수직으로 밀어냄! ▼▼▼
                Collider myCollider = GetComponent<Collider>();
                Vector3 hitPoint = myCollider.ClosestPoint(player.transform.position); // 가장 가까운 충돌 지점 찾기
                Vector3 pushDir = (player.transform.position - hitPoint).normalized;   // 거기서 바깥쪽으로 밀기

                // 혹시라도 방향 계산이 안 되면(0), 그냥 플레이어 뒤로 밀어버림 (안전장치)
                if (pushDir == Vector3.zero) pushDir = -player.transform.forward;

                pushDir.y = 0; // 위로는 붕 뜨지 않게
                // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

                player.ApplyKnockback(pushDir, knockbackForce);
                Debug.Log("정확한 방향으로 넉백!");
            }
        }
    }
}