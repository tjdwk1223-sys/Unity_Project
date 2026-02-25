using UnityEngine;

public class BossPunch : MonoBehaviour
{
    public float damage = 10f; // 마왕의 펀치 데미지

    private void OnTriggerEnter(Collider other)
    {
        // 닿은 물체가 키키(Player)라면?
        if (other.CompareTag("Player"))
        {
            // TODO: 나중에 키키 스크립트 완성되면 주석 풀고 연결!
            // other.GetComponent<PlayerController>().TakeDamage(damage);

            // 어느 손으로 때렸는지 콘솔창에 띄워줍니다.
            Debug.Log(gameObject.name + "가 맨손으로 키키를 팼습니다! 퍽!");

            // [다단히트 방지] 한 번 닿아서 데미지를 줬으면 즉시 내 판정을 꺼버림
            GetComponent<Collider>().enabled = false;
        }
    }
}