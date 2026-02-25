using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public int zombieCount = 30;    //
    public float spawnRange = 35f;  // 범위를 넉넉하게 늘렸습니다.

    [Header("울타리 금지 좌표 (X: 62.8, Z: 63.1)")]
    public float safeCenterX = 62.8f; //
    public float safeCenterZ = 63.1f; //
    public float safeHalfWidth = 10f;
    public float safeHalfDepth = 12f;

    void Start()
    {
        SpawnZombiesWithDebug();
    }

    void SpawnZombiesWithDebug()
    {
        int spawned = 0;
        int navFail = 0;
        int safeZoneFail = 0;

        for (int i = 0; i < zombieCount * 5; i++)
        { // 시도 횟수를 대폭 늘림
            if (spawned >= zombieCount) break;

            // 스포너(본인) 위치 기준으로 랜덤 좌표 생성
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0,
                Random.Range(-spawnRange, spawnRange)
            );

            // 1. 수학적 금지 구역 체크
            bool isInsideX = randomPos.x > (safeCenterX - safeHalfWidth) && randomPos.x < (safeCenterX + safeHalfWidth);
            bool isInsideZ = randomPos.z > (safeCenterZ - safeHalfDepth) && randomPos.z < (safeCenterZ + safeHalfDepth);

            if (isInsideX && isInsideZ)
            {
                safeZoneFail++;
                continue;
            }

            // 2. 내비메시 바닥 체크 (범위를 10.0f로 확장)
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 10.0f, NavMesh.AllAreas))
            {
                Instantiate(zombiePrefab, hit.position, Quaternion.identity);
                spawned++;
            }
            else
            {
                navFail++;
            }
        }
        Debug.Log($"[결과] 성공: {spawned} / 울타리안이라 실패: {safeZoneFail} / 바닥없어 실패: {navFail}");
    }

    // 에디터에서 소환 범위를 시각적으로 보여줌 (기즈모)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(safeCenterX, 0, safeCenterZ), new Vector3(safeHalfWidth * 2, 2, safeHalfDepth * 2));
    }
}