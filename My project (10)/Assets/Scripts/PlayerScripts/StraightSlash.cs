using UnityEngine;

public class StraightSlash : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 2f;
    private float timer = 0f;

    [Header("--- 대미지 설정 ---")]
    public GameObject damageTextPrefab; // 유니티에서 DamageText 프리팹을 연결하세요!

    [HideInInspector]
    public Vector3 shootDirection;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        transform.position += shootDirection * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    // 적과 부딪혔을 때 숫자를 띄우는 코드 추가
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            int damage = Random.Range(20, 40);
            bool isCrit = Random.Range(0, 100) < 20;
            if (isCrit) damage *= 2;

            // 적의 머리 위(약 2m)에 생성
            Vector3 spawnPos = other.transform.position + Vector3.up * 2.0f;
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

            // DamageText 스크립트의 Setup 호출
            textObj.GetComponent<DamageText>().Setup(damage, isCrit);

            // 적에게 맞았으니 검기를 사라지게 함 (관통을 원하면 아래 줄 삭제)
            gameObject.SetActive(false);
        }
    }
}