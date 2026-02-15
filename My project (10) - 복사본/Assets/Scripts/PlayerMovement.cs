using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10.0f; // 속도를 10으로 올렸습니다.
    private CharacterController controller;

    void Start()
    {
        // 내 몸에 붙은 부품을 찾습니다.
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 핵심: 부품이 없으면 아래 코드를 실행하지 않음 (에러 방지)
        if (controller == null)
        {
            controller = GetComponent<CharacterController>(); // 다시 한번 찾아보기
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection = transform.TransformDirection(moveDirection);

        // SimpleMove는 중력을 자동으로 적용합니다.
        controller.SimpleMove(moveDirection * speed);
    }
}