using UnityEngine;

public class StraightSlash : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 2f;
    private float timer = 0f;

    [HideInInspector]
    public Vector3 shootDirection; // ★ 밖에서 주입받을 '진짜 날아갈 방향'

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        // 내 각도가 어떻든 상관없이, 주입받은 방향(shootDirection)으로만 무식하게 직진!
        transform.position += shootDirection * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }
}