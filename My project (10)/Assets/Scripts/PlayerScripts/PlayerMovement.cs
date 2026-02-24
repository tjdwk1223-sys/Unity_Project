using UnityEngine;

public class KikiController : MonoBehaviour
{
    [Header("★ 능력치 데이터 연결")]
    public CharacterStats stats; // KikiStats 연결 확인!
    private float lastAttackTime;

    [Header("실시간 상태")]
    public float currentHp; // 데이터 시트의 Max HP 230 적용

    [Header("점프 및 중력")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    [Header("마우스 시선 회전 설정")]
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    private float xRotation = 0f;

    [Header("필수 연결 요소")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // 시작할 때 데이터 시트(Stats)에서 체력을 가져옵니다.
        if (stats != null) currentHp = stats.maxHp;
    }

    void Update()
    {
        // 1. 지면 체크 및 중력
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // 2. 마우스 시선 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 3. 이동 로직
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        float baseSpeed = (stats != null) ? stats.moveSpeed : 5.0f;
        float currentSpeed = (Input.GetKey(KeyCode.LeftShift) && isGrounded) ? baseSpeed * 1.5f : baseSpeed;
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        // 4. 애니메이션 파라미터
        if (anim != null)
        {
            anim.SetFloat("Horizontal", x, 0.1f, Time.deltaTime);
            anim.SetFloat("Vertical", z, 0.1f, Time.deltaTime);
            float inputMagnitude = new Vector2(x, z).magnitude;
            anim.SetFloat("Speed", inputMagnitude * currentSpeed);
        }

        // 5. 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 6. 전투 입력 (기획자님의 모든 스킬 복구!)
        HandleCombatInput();
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log($"앗! 키키가 {damage}만큼 맞았다! 남은 피: {currentHp}");
    }

    void HandleCombatInput()
    {
        if (anim == null || stats == null) return;

        // 공격 속도 체크
        if (Time.time >= lastAttackTime + stats.AttackCooldown)
        {
            bool isAttack = false;

            // 기획자님이 만드신 모든 스킬 단축키 100% 보존
            if (Input.GetMouseButtonDown(0)) { anim.SetTrigger("doSword"); isAttack = true; } // LMB
            if (Input.GetMouseButtonDown(1)) { anim.SetTrigger("doKick"); isAttack = true; }  // RMB
            if (Input.GetKeyDown(KeyCode.Z)) { anim.SetTrigger("doKick"); isAttack = true; }  // Z
            if (Input.GetKeyDown(KeyCode.X)) { anim.SetTrigger("doSkill"); isAttack = true; } // X
            if (Input.GetKeyDown(KeyCode.C)) { anim.SetTrigger("doSkill2"); isAttack = true; }// C
            if (Input.GetKeyDown(KeyCode.Q)) { anim.SetTrigger("doSkill3"); isAttack = true; }// Q
            if (Input.GetKeyDown(KeyCode.F)) { anim.SetTrigger("doSkill4"); isAttack = true; }// F
            if (Input.GetKeyDown(KeyCode.V)) { anim.SetTrigger("doSkill5"); isAttack = true; }// V

            if (isAttack) lastAttackTime = Time.time;
        }
    }
}