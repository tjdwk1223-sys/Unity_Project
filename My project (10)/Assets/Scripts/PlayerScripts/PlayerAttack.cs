using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [Header("--- 대미지 텍스트 ---")]
    public GameObject damageTextPrefab; // 여기에 DamageText 프리팹을 드래그하세요.
    [Header("--- 검기 설정 ---")]
    public bool hasSwordAura = false;
    public GameObject auraPrefab;
    public Transform firePoint;
    public float delayTime = 0.3f;

    [Header("--- 쿨타임 (연속 클릭 방지) ---")]
    public float attackCooldown = 1.0f;
    private bool isAttacking = false;

    [Header("--- 탄창(풀링) 설정 ---")]
    public int poolSize = 20;
    private List<GameObject> auraPool = new List<GameObject>();

    void Start()
    {
        // ▼▼▼ [수정됨: 고장난 PlayerPrefs를 지우고 게임 매니저와 연결!] ▼▼▼
        if (GameManager.Instance != null)
        {
            // 매니저가 "너 검기 써도 돼(true)" 하면 켜고, "안 돼(false)" 하면 끕니다.
            hasSwordAura = GameManager.Instance.isAuraUnlocked;
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // 이 아래쪽 오브젝트 풀링(검기 미리 만들어두기)은 그대로 둡니다!
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(auraPrefab);
            obj.SetActive(false);
            auraPool.Add(obj);
        }
    }
    void Update()
    {
        if (hasSwordAura && Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(ShootAuraWithDelay());
        }
    }

    IEnumerator ShootAuraWithDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(delayTime);
        ExecuteShoot();
        yield return new WaitForSeconds(attackCooldown - delayTime);
        isAttacking = false;
    }

    void ExecuteShoot()
    {
        if (firePoint == null) return;

        for (int i = 0; i < auraPool.Count; i++)
        {
            if (!auraPool[i].activeInHierarchy)
            {
                // 1. 위치는 총구 위치로 맞춤
                auraPool[i].transform.position = firePoint.position;

                // 2. ★ 핵심: 플레이어 방향(총구 회전)에다가 + 기획자님이 깎아둔 프리팹 원래 각도를 곱해서 유지!
                auraPool[i].transform.rotation = firePoint.rotation * auraPrefab.transform.rotation;

                // 3. ★ 핵심: 검기 스크립트를 가져와서 "총구 앞쪽(정면)으로 날아가라!" 고 방향 강제 주입
                StraightSlash slashScript = auraPool[i].GetComponent<StraightSlash>();
                if (slashScript != null)
                {
                    slashScript.shootDirection = firePoint.forward;
                }

                auraPool[i].SetActive(true);
                return;
            }
        }
    }

    public void UnlockAura()
    {
        hasSwordAura = true;

        // ★ [추가된 부분 2] NPC 퀘스트를 깨서 검기를 얻는 순간, 유니티 시스템에 영구 저장!
        PlayerPrefs.SetInt("SwordAuraUnlocked", 1);
        Debug.Log("검기 스킬 영구 해금 완료!");
    }
}