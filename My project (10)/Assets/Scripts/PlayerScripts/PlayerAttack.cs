using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
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
    }
}